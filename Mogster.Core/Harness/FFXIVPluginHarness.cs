using FFXIV_ACT_Plugin.Common;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Logfile;
using FFXIV_ACT_Plugin.Memory;
using FFXIV_ACT_Plugin.Memory.MemoryProcessors;
using FFXIV_ACT_Plugin.Memory.MemoryReader;
using FFXIV_ACT_Plugin.Network;
using FFXIV_ACT_Plugin.Overlay;
using FFXIV_ACT_Plugin.Parse;
using FFXIV_ACT_Plugin.Resource;
using Mogster.Core.Harness.FFXIVPlugin;
using TinyIoC;

namespace Mogster.Core.Harness
{
    public class FFXIVPluginHarness
    {
        public IZoneProcessor ZoneProcessor { get; private set; }

//        private DataCollection DataCollection;

        public TinyIoCContainer IoCContainer { get; private set; }
        public IProcessManager ProcessManager { get; private set; }

        public FFXIVPluginHarness()
        {
            this.IoCContainer = new TinyIoCContainer();
            ConfigureIOC();

            this.ProcessManager = IoCContainer.Resolve<IProcessManager>();
            this.ZoneProcessor = IoCContainer.Resolve<IZoneProcessor>();
        }

        public void UpdateMemoryScanSettings(MemoryScanSettings memoryScanSettings)
        {
            this.IoCContainer.Resolve<ISettingsManager>().UpdateMemoryScanSettings(memoryScanSettings);
        }
        public void UpdateParseScanSettings(ParseSettings parseSettings)
        {
            this.IoCContainer.Resolve<ISettingsManager>().UpdateParseSettings(parseSettings);
        }

        #region IOC Configuration
        private void ConfigureIOC()
        {
            this.IoCContainer = new TinyIoCContainer();
            this.IoCContainer.Register<ISettingsManager, SettingsManager>().AsSingleton();

            this.IoCContainer.Register<IACTEvent, ACT_Event>().AsSingleton();
            this.IoCContainer.Register<IResourceData, ResourceData>().AsSingleton();
            this.IoCContainer.Register<IAbilityResource, Abilities>().AsSingleton();
            this.IoCContainer.Register<IBuffList, BuffList>().AsSingleton();
            this.IoCContainer.Register<INameResource, NameList>().AsSingleton();
            this.IoCContainer.Register<ISkillList, SkillList>().AsSingleton();
            this.IoCContainer.Register<IWorldList, WorldList>().AsSingleton();
            this.IoCContainer.Register<IZoneList, ZoneList>().AsSingleton();
            this.IoCContainer.Register<ICombatantManager, Mogster.Core.Harness.FFXIVPlugin.CombatantManager>().AsSingleton();
            this.IoCContainer.Register<DataCollection>().AsSingleton();
            this.IoCContainer.Register<DataEvent>().AsSingleton();
            this.IoCContainer.Register<IDataSubscription, FFXIV_ACT_Plugin.Memory.DataSubscription>().AsSingleton();
            this.IoCContainer.Register<IDataRepository, FFXIV_ACT_Plugin.Memory.DataRepository>().AsSingleton();
            this.IoCContainer.Register<ILogOutput, BlankLogOutput>().AsSingleton();
            this.IoCContainer.Register<ILogFormat, CLF>().AsSingleton();
            this.IoCContainer.Register<IProcessManager, ProcessManager>().AsSingleton();
            this.IoCContainer.Register<ISignatureManager, SignatureManager>().AsSingleton();
            this.IoCContainer.Register<IScanMemory, Mogster.Core.Harness.FFXIVPlugin.ScanMemory>().AsSingleton();
            this.IoCContainer.Register<Mogster.Core.Harness.FFXIVPlugin.ScanHPPercent>().AsSingleton();
            this.IoCContainer.Register<ScanPackets>().AsSingleton();
            this.IoCContainer.Register<DataEventProcessor>().AsSingleton();
            this.IoCContainer.Register<IReadCombatant, ReadCombatant>().AsSingleton();
            this.IoCContainer.Register<IReadFlyingText, ReadFlyingText>().AsSingleton();
            this.IoCContainer.Register<IReadLog, ReadLog>().AsSingleton();
            this.IoCContainer.Register<IReadMemory, ReadMemory>().AsSingleton();
            this.IoCContainer.Register<IReadMobArray, ReadMobArray>().AsSingleton();
            this.IoCContainer.Register<IReadParty, ReadParty>().AsSingleton();
            this.IoCContainer.Register<IReadPlayer, ReadPlayer>().AsSingleton();
            this.IoCContainer.Register<IReadProcesses, ReadProcesses>().AsSingleton();
            this.IoCContainer.Register<IReadServerTime, ReadServerTime>().AsSingleton();
            this.IoCContainer.Register<IReadZoneId, ReadZoneId>().AsSingleton();
            this.IoCContainer.Register<IReadTarget, ReadTarget>().AsSingleton();
            this.IoCContainer.Register<ICombatantAbilityProcessor, CombatantAbilityProcessor>().AsSingleton();
            this.IoCContainer.Register<ICombatantEffectProcessor, CombatantEffectProcessor>().AsSingleton();
            this.IoCContainer.Register<ICombatantFlyingTextProcessor, CombatantFlyingTextProcessor>().AsSingleton();
            this.IoCContainer.Register<ICombatantProcessor, CombatantProcessor>().AsSingleton();
            this.IoCContainer.Register<ILogProcessor, LogProcessor>().AsSingleton();
            this.IoCContainer.Register<IMobArrayProcessor, MobArrayProcessor>().AsSingleton();
            this.IoCContainer.Register<IPartyProcessor, PartyProcessor>().AsSingleton();
            this.IoCContainer.Register<IPlayerProcessor, PlayerProcessor>().AsSingleton();
            this.IoCContainer.Register<IServerTimeProcessor, ServerTimeProcessor>().AsSingleton();
            this.IoCContainer.Register<IZoneProcessor, ZoneProcessor>().AsSingleton();
            this.IoCContainer.Register<ITargetProcessor, TargetProcessor>().AsSingleton();
            this.IoCContainer.Register<INetworkBuffManager, NetworkBuffManager>().AsSingleton();
            this.IoCContainer.Register<ProcessAbility>().AsSingleton();
            this.IoCContainer.Register<ProcessActorCast>().AsSingleton();
            this.IoCContainer.Register<ProcessActorControl142>().AsSingleton();
            this.IoCContainer.Register<ProcessActorControl143>().AsSingleton();
            this.IoCContainer.Register<ProcessActorControl144>().AsSingleton();
            this.IoCContainer.Register<ProcessActorGauge>().AsSingleton();
            this.IoCContainer.Register<ProcessAddStatusEffect>().AsSingleton();
            this.IoCContainer.Register<ProcessStatusEffectList>().AsSingleton();
            this.IoCContainer.Register<LogParse>().AsSingleton();
            this.IoCContainer.Register<LogParseBuff>().AsSingleton();
            this.IoCContainer.Register<LogParseData>().AsSingleton();
            this.IoCContainer.Register<LogParseChatBufferLine>().AsSingleton();
            this.IoCContainer.Register<LogParseFlyingText>().AsSingleton();
            this.IoCContainer.Register<IDoTSimulator, DoTSimulator>().AsSingleton();
            this.IoCContainer.Register<ICombatantHistory, CombatantHistory>().AsSingleton();
            this.IoCContainer.Register<IBuffHistory, BuffHistory>().AsSingleton();
            this.IoCContainer.Register<ISkillHistory, SkillHistory>().AsSingleton();
        }
        #endregion
    }
}
