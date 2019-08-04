using FFXIV_ACT_Plugin.Common.Models;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Memory;
using FFXIV_ACT_Plugin.Memory.MemoryProcessors;
using FFXIV_ACT_Plugin.Memory.MemoryReader;
using FFXIV_ACT_Plugin.Resource;
using Mogster.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TinyIoC;

namespace Mogster.Core.EdgeBindings
{
    public class CombatantManagerReader
    {
        public TinyIoCContainer IoCContainer { get; private set; }
        public IProcessManager ProcessManager { get; private set; }
        public IZoneProcessor ZoneProcessor { get; private set; }

        public ICombatantManager combatantManager;
        public IMobArrayProcessor mobArrayProcessor;
        private IReadCombatant readCombatant;

        private System.Timers.Timer processTimer = new System.Timers.Timer(200);

        private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        public CombatantManagerReader()
        {
            this.IoCContainer = new TinyIoCContainer();

            this.IoCContainer.Register<DataEvent>().AsSingleton();

            this.IoCContainer.Register<ICombatantAbilityProcessor, CombatantAbilityProcessor>().AsSingleton();
            this.IoCContainer.Register<ICombatantEffectProcessor, CombatantEffectProcessor>().AsSingleton();
            this.IoCContainer.Register<ICombatantFlyingTextProcessor, CombatantFlyingTextProcessor>().AsSingleton();
            this.IoCContainer.Register<ICombatantManager, CombatantManager>().AsSingleton();
            this.IoCContainer.Register<ICombatantProcessor, CombatantProcessor>().AsSingleton();
            this.IoCContainer.Register<IMobArrayProcessor, MobArrayProcessor>().AsSingleton();
            this.IoCContainer.Register<IProcessManager, ProcessManager>().AsSingleton();
            this.IoCContainer.Register<IReadCombatant, ReadCombatant>().AsSingleton();
            this.IoCContainer.Register<IReadFlyingText, ReadFlyingText>().AsSingleton();
            this.IoCContainer.Register<IReadMemory, ReadMemory>().AsSingleton();
            this.IoCContainer.Register<IReadMobArray, ReadMobArray>().AsSingleton();
            this.IoCContainer.Register<IReadProcesses, ReadProcesses>().AsSingleton();
            this.IoCContainer.Register<ISettingsManager, SettingsManager>().AsSingleton();
            this.IoCContainer.Register<ISignatureManager, SignatureManager>().AsSingleton();
            this.IoCContainer.Register<IWorldList, WorldList>().AsSingleton();

            MemoryScanSettings memoryScanSettings = new MemoryScanSettings()
            {
                ProcessID = 0
            };

            this.ProcessManager = IoCContainer.Resolve<IProcessManager>();

            this.IoCContainer.Resolve<ISettingsManager>().UpdateMemoryScanSettings(memoryScanSettings);
            combatantManager = this.IoCContainer.Resolve<ICombatantManager>();
            mobArrayProcessor = this.IoCContainer.Resolve<IMobArrayProcessor>();
            readCombatant = this.IoCContainer.Resolve<IReadCombatant>();
            processTimer.Elapsed += ProcessTimer_Elapsed;
            processTimer.Start();
        }

        private void ProcessTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.ProcessManager.Verify();
        }

        public void UpdateMemoryScanSettings(MemoryScanSettings memoryScanSettings)
        {
            this.IoCContainer.Resolve<ISettingsManager>().UpdateMemoryScanSettings(memoryScanSettings);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "CS1998:This async method lacks 'await' operators and will run synchronously.", Justification = "Required by EdgeJS")]
        public async Task<object> GetCombatant(dynamic input)
        {
            cacheLock.EnterReadLock();
            try
            {
                mobArrayProcessor.Refresh();
                combatantManager.Refresh();

                Combatant combatant = combatantManager.GetCombatantById((uint)input);

                CombatantEx combatantEx = CombatantEx.Ingest(combatant);

                unsafe
                {
                    CombatantExStruct* combatantExStruct = (CombatantExStruct*)(void*)readCombatant.Read(combatant.Pointer);
                    combatantEx.AgroFlags = combatantExStruct->AgroFlags;
                    combatantEx.CombatFlags = combatantExStruct->CombatFlags;
                }

                return combatantEx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required by EdgeJS")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "CS1998:This async method lacks 'await' operators and will run synchronously.", Justification = "Required by EdgeJS")]
        public async Task<object> GetCurrentPlayer(dynamic input)
        {
            cacheLock.EnterReadLock();
            try
            {
                mobArrayProcessor.Refresh();
                combatantManager.Refresh();

                if (mobArrayProcessor.PrimaryPlayerPointer == null
                    || mobArrayProcessor.PrimaryPlayerPointer == IntPtr.Zero)
                {
                    throw new Exception("Could not find you!");
                }

                Combatant combatant = combatantManager.GetCombatantByPointer(mobArrayProcessor.PrimaryPlayerPointer);

                CombatantEx combatantEx = CombatantEx.Ingest(combatant);

                unsafe
                {
                    CombatantExStruct* combatantExStruct = (CombatantExStruct*)(void*)readCombatant.Read(combatant.Pointer);
                    combatantEx.AgroFlags = combatantExStruct->AgroFlags;
                    combatantEx.CombatFlags = combatantExStruct->CombatFlags;
                }


                return combatantEx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required by EdgeJS")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "CS1998:This async method lacks 'await' operators and will run synchronously.", Justification = "Required by EdgeJS")]
        public async Task<object> GetCombatantList(dynamic input)
        {
            cacheLock.EnterReadLock();
            try
            {
                mobArrayProcessor.Refresh();
                combatantManager.Refresh();

                List<Combatant> combatants = new List<Combatant>(combatantManager.Combatants);
                combatants.Where(c => c.Pointer != IntPtr.Zero && c.ID != 0);
                return combatantManager.GetCombatantByPointer(mobArrayProcessor.PrimaryPlayerPointer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }
    }
}
