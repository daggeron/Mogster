
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

using System.IO;
using FFXIV_ACT_Plugin.Memory.MemoryProcessors;
using System.Diagnostics;
using Machina.FFXIV;
using Machina;
using Mogster.Core.Harness.FFXIVPlugin;
using FFXIV_ACT_Plugin.Memory.MemoryReader;
using Mogster.Core.Models;
using NLog.Config;
using NLog.Targets;
using NLog;

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
        private DataEvent dataEvent;
        private IReadCombatant readCombatant;

        private System.Timers.Timer Timer = new System.Timers.Timer();

        public Program()
        {


            var config = new LoggingConfiguration();

            // Step 2. Create targets
            var consoleTarget = new ColoredConsoleTarget("target1")
            {
                Layout = @"${date:format=HH\:mm\:ss} [${logger}] ${level:uppercase=false}: ${message} ${exception}"
            };
            config.AddTarget(consoleTarget);
            config.AddRuleForAllLevels(consoleTarget); // all to console
            LogManager.Configuration = config;


            Logger.Info("FFXIV Harness has been constructed");

            ffxivPlugin = new FFXIVPluginHarness();
            ffxivPlugin.IoCContainer.Register<IEventAggregator, EventAggregator>().AsSingleton();
            eventAggregator = ffxivPlugin.IoCContainer.Resolve<IEventAggregator>();
            processManager = ffxivPlugin.IoCContainer.Resolve<IProcessManager>();
            eventAggregator.GetEvent<CombatantRefreshEvent>().Subscribe(() =>
            {
                var a = 1;
            });

            Initialize();


            Timer.Interval = 50;

            Timer.Elapsed += new System.Timers.ElapsedEventHandler((e, d) =>
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
            dataEvent = ffxivPlugin.IoCContainer.Resolve<DataEvent>();

            ffxivPlugin.IoCContainer.Resolve<FFXIVEventBindings>();
            combatantManager = ffxivPlugin.IoCContainer.Resolve<ICombatantManager>();
            mobArrayProcessor = ffxivPlugin.IoCContainer.Resolve<IMobArrayProcessor>();
            
            readCombatant = ffxivPlugin.IoCContainer.Resolve<IReadCombatant>();
            //sharlayanHarness = ffxivPlugin.IoCContainer.Resolve<SharlayanHarness>();

            SetShutdownRequestHook();
        }

        FFXIVNetworkMonitor _monitor = new FFXIVNetworkMonitor();

        public void StopScan()
        {
            if (this._monitor == null)
                return;
            this._monitor.MessageReceived = (FFXIVNetworkMonitor.MessageReceivedDelegate)null;
            this._monitor.MessageSent = (FFXIVNetworkMonitor.MessageSentDelegate)null;
            this._monitor.Stop();
            this._monitor = (FFXIVNetworkMonitor)null;
        }

        public unsafe void Monitor_MessageReceived(string id, long epoch, byte[] message)
        {
            try
            {
                DateTime packetDate;
                if (epoch > 0L)
                {
                    packetDate = Utility.EpochToDateTime(epoch).ToLocalTime();
                }
                else
                {
                    packetDate = DateTime.MinValue;
                }
                if (message.Length >= sizeof(Server_MessageHeader))
                {
                    fixed (byte* buffer = message)
                    {
                        Server_MessageHeader* messageHeader = (Server_MessageHeader*)buffer;

                        Server_MessageType messageType = ((Server_MessageHeader*)buffer)->MessageType;
                        //                        if (messageType == Server_MessageType.)
                        //{
                        Logger.Debug(messageHeader->LoginUserID + " " + messageType);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void Monitor_MessageSent(string id, long epoch, byte[] message)
        {
        }
        public void testNW()
        {
            System.Timers.Timer testTimer = new System.Timers.Timer(2000);
            testTimer.Elapsed += (sender, args) =>
            {
                FFXIV_ACT_Plugin.Common.Models.Combatant combatant;

                try
                {
                    combatant = combatantManager.GetCombatantByPointer(mobArrayProcessor.PrimaryPlayerPointer);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    throw ex;
                }

                unsafe
                {
                    if (combatant.TargetID != 0)
                    {
                        var tc = combatantManager.GetCombatantById(combatant.TargetID);
                        CombatantExStruct* ptr = (CombatantExStruct*)(void*)readCombatant.Read(tc.Pointer);

                        bool InCombat = (ptr->CombatFlags & (1 << 1)) != 0;

                        bool IsAggressive = (ptr->CombatFlags & (1 << 0)) != 0;

                        bool IsAgroed = (ptr->AgroFlags & 1) > 0;
                        Logger.Debug(IsAggressive + " " + IsAgroed + " " + InCombat);
                    }
                }
            };
            testTimer.Start();
            /*_monitor.MessageReceived = delegate (string id, long epoch, byte[] message)
            {
                this.Monitor_MessageReceived(id, epoch, message);
            };
            dataEvent.NetworkReceived = (DataEvent.NetworkReceivedDelegate)((id, epoch, message) => this.Monitor_MessageReceived(id, epoch, message));*/
        }
        public void testNW2()
        {
            CurrentProcess currentProcess = this.processManager.Current;
            int? num;
            if (currentProcess == null)
            {
                num = null;
            }
            else
            {
                Process process = currentProcess.Process;
                num = ((process != null) ? new int?(process.Id) : null);
            }
            int num2 = num ?? 0;

            _monitor = new FFXIVNetworkMonitor();
            _monitor.MonitorType = TCPNetworkMonitor.NetworkMonitorType.RawSocket;
            _monitor.ProcessID = (uint)num2;

            _monitor.MessageReceived = delegate (string id, long epoch, byte[] message)
            {
                this.Monitor_MessageReceived(id, epoch, message);
            };

            _monitor.MessageSent = delegate (string id, long epoch, byte[] message)
            {
                this.Monitor_MessageSent(id, epoch, message);
            };

            _monitor.Start();
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
            //testNW();

            callBack = input;

            test();

            eventAggregator.GetEvent<IPCMessageEvent>().Subscribe((message) =>
            {
                this.callBack.Invoke(message.ToString());
            }, ThreadOption.BackgroundThread);

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

            FFXIV_ACT_Plugin.Common.Models.Combatant combatant;

            try
            {
                combatant = combatantManager.GetCombatantByPointer(mobArrayProcessor.PrimaryPlayerPointer);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
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

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

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
