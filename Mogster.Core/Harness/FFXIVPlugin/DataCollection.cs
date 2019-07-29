using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Logfile;

using FFXIV_ACT_Plugin.Network;

using FFXIV_ACT_Plugin.Memory;
using FFXIV_ACT_Plugin.Overlay;
using System.Threading;

namespace Mogster
{
    public class DataCollection
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly ISettingsManager settingsManager;
        private readonly IProcessManager processManager;
        private readonly ILogOutput logOutput;
        private readonly IScanMemory scanMemory;
        private ScanPackets scanPackets;
        private readonly Mogster.Core.Harness.FFXIVPlugin.ScanHPPercent scanHPPercent;
        private readonly DataEventProcessor dataEventProcessor;


        public DataCollection(
            ISettingsManager settingsManager,
            IProcessManager processManager,
            ILogOutput logOutput,
            IScanMemory scanMemory,
            ScanPackets scanPackets,
            Mogster.Core.Harness.FFXIVPlugin.ScanHPPercent scanHPPercent,
            DataEventProcessor dataEventProcessor)
        {
            this.settingsManager = settingsManager;
            this.processManager = processManager;
            this.logOutput = logOutput;
            this.scanMemory = scanMemory;
            this.scanPackets = scanPackets;
            this.scanHPPercent = scanHPPercent;
            this.dataEventProcessor = dataEventProcessor;
        }

        public void Start()
        {
            //scanMemory.StartScan();
            scanPackets.StartScan();
            scanHPPercent.StartScan();
        }

        public void Stop()
        {
            //scanMemory.StopScan();
            scanPackets.StopScan();
            scanHPPercent.StopScan();
        }
    }
}
