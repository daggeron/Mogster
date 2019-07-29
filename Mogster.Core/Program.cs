
using System;
using System.Threading;
using System.Threading.Tasks;

using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Memory;

using Mogster.Core.Events;
using Mogster.Core.Harness;
using Mogster.Core.Helpers;
using Prism.Events;
using System.Linq;
using Thaliak.Network.Utilities;
using WindowsFirewallHelper;
using Newtonsoft.Json;
using FFXIV_ACT_Plugin.Common.Models;
using System.IO;
using FFXIV_ACT_Plugin.Memory.MemoryProcessors;

namespace Mogster.Core
{

    /***
     * Important note about TinyIOC:
     * Everything registered is considered as lazyloaded.
     */
    class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private CancellationTokenSource cancellationToken = new CancellationTokenSource();
        private DataCollection DataCollection;
        private IEventAggregator eventAggregator;
        private IProcessManager processManager;
        private IMobArrayProcessor mobArrayProcessor;
        private ICombatantManager combatantManager;
        private FFXIVPluginHarness ffxivPlugin;

        private System.Timers.Timer Timer = new System.Timers.Timer();

        public Program()
        {
            Logger.Info("FFXIV Harness has been constructed");
            Console.WriteLine("FFXIV Harness has been constructed");

            ffxivPlugin = new FFXIVPluginHarness();
            ffxivPlugin.IoCContainer.Register<IEventAggregator, EventAggregator>().AsSingleton();
            eventAggregator = ffxivPlugin.IoCContainer.Resolve<IEventAggregator>();
            processManager = ffxivPlugin.IoCContainer.Resolve<IProcessManager>();
            eventAggregator.GetEvent<CombatantRefreshEvent>().Subscribe(() => {
                var a = 1;
            });

            Initialize();
            
            
            Timer.Interval = 50;

            Timer.Elapsed += new System.Timers.ElapsedEventHandler((e,d) =>
            {
                if (processManager.Verify())
                    EventManager.SetAll("scan_event");
            });
            Timer.Start();
        }

        private void Initialize()
        {
            DataCollection = ffxivPlugin.IoCContainer.Resolve<DataCollection>();
            DataCollection.Start();

            MemoryScanSettings memoryScanSettings = new MemoryScanSettings()
            {
                ProcessID = 0,
                UseWinPCap = false
            };

            ParseSettings parseSettings = new ParseSettings()
            {
                LanguageID = 99
            };


            ffxivPlugin.UpdateMemoryScanSettings(memoryScanSettings);
            ffxivPlugin.UpdateParseScanSettings(parseSettings);

            Logger.Debug("FFXIVEVENTBINDINGS");
            ffxivPlugin.IoCContainer.Resolve<FFXIVEventBindings>();
            combatantManager = ffxivPlugin.IoCContainer.Resolve<ICombatantManager>();
            mobArrayProcessor = ffxivPlugin.IoCContainer.Resolve<IMobArrayProcessor>();
            
            //sharlayanHarness = ffxivPlugin.IoCContainer.Resolve<SharlayanHarness>();

            SetShutdownRequestHook();
        }

        public void SetShutdownRequestHook()
        {
            
            eventAggregator.GetEvent<ShutdownEvent>().Subscribe(() =>
            {
                Cleanup();
            });
        }

        public void Cleanup()
        {
            //ffxivPlugin.IoCContainer.Resolve<IScanMemory>().StopScan();
            cancellationToken.Cancel();
        }

        private Func<object, Task<object>> callBack;

        public async Task<object> BindEventLoop(Func<object, Task<object>> input)
        {
            callBack = input;

            test();

            eventAggregator.GetEvent<IPCMessageEvent>().Subscribe((message) => this.callBack.Invoke(message.ToString()));

            callBack.Invoke("{'hello':'world'}");

            return 0;
        }


        public async Task<object> GetCombatant(dynamic input)
        {
            return combatantManager.GetCombatantById((uint)input);
        }

        public async Task<object> GetCurrentPlayer(dynamic input)
        {
            if (mobArrayProcessor.PrimaryPlayerPointer == null)
            {
                throw new Exception("Couldn't find you!");
            }

            Combatant combatant;

            try
            {
                combatant = combatantManager.GetCombatantByPointer(mobArrayProcessor.PrimaryPlayerPointer);
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }

            return combatant;
        }
        public async Task<object> GetCombatantList(dynamic input)
        {
            
            //var cl = combatantManager.Combatants.Where(mc => mc.ID != 0).GroupBy(mc => mc.ID).Select(mc => mc.First()).ToDictionary(mc => mc.ID, mc => mc.Name);
            try
            {
                var cl = combatantManager.Combatants;
                var pr = combatantManager.Combatants.Where(mc => mc.Name.StartsWith("Elise", StringComparison.OrdinalIgnoreCase));

                string json = JsonConvert.SerializeObject(cl, Formatting.Indented);
                using (StreamWriter outputFile = new StreamWriter(Path.Combine("E:\\tmp", "dump.json")))
                {
                    outputFile.Write(json);
                }

                Console.WriteLine(json);
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            //string json = JsonConvert.SerializeObject(cl, Formatting.Indented);
            //Console.WriteLine(json);
            return null;
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            FirewallManager.Instance.Rules
                .Where(item => item.Name.Equals("Mogster", StringComparison.OrdinalIgnoreCase))
                .ToList()
                .ForEach(rule => FirewallManager.Instance.Rules.Remove(rule));
        }

        static void test()
        {
            FirewallManager.Instance.Rules
                .Where(item => item.Name.Equals("Mogster", StringComparison.OrdinalIgnoreCase))
                .ToList()
                .ForEach(rule => FirewallManager.Instance.Rules.Remove(rule));

            FirewallRegister.RegisterToFirewall();
        }
    }
}
