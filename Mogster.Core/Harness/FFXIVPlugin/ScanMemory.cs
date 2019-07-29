using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Memory;
using FFXIV_ACT_Plugin.Memory.MemoryProcessors;
using Mogster.Core.Events;
using Mogster.Core.Harness;
using Mogster.Core.Helpers;
using Prism.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mogster.Core.Harness.FFXIVPlugin
{
    public class ScanMemory : IScanMemory
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private IServerTimeProcessor serverTimeProcessor;
        private IZoneProcessor zoneProcessor;
        private IMobArrayProcessor mobArrayProcessor;
        private ICombatantManager combatantManager;
        private IPlayerProcessor playerProcessor;
        private IPartyProcessor partyProcessor;
        private ILogProcessor logProcessor;
        private ITargetProcessor targetProcessor;
        private readonly IEventAggregator eventAggregator;

        //private readonly SharlayanHarness sharlayanHarness;

        private CancellationTokenSource tokenSource;
        private AutoResetEvent loopResetEvent;
        private bool disableNetworkScan;

        public Thread ScanThread => throw new NotImplementedException();

        public ScanMemory(
          ISettingsManager settingsManager,
          IServerTimeProcessor serverTime,
          IZoneProcessor zoneReader,
          IMobArrayProcessor mobArrayReader,
          ICombatantManager combatantManager,
          IPlayerProcessor playerProcessor,
          IPartyProcessor partyProcessor,
          ILogProcessor logProcessor,
          ITargetProcessor targetProcessor,
          IEventAggregator eventAggregator
          /*SharlayanHarness sharlayanHarness*/)
        {
            settingsManager.MemoryScanSettingsChanged += new EventHandler<MemoryScanSettings>(this.OnMemoryScanSettingsChanged);
            this.serverTimeProcessor = serverTime;
            this.zoneProcessor = zoneReader;
            this.mobArrayProcessor = mobArrayReader;
            this.combatantManager = combatantManager;
            this.playerProcessor = playerProcessor;
            this.partyProcessor = partyProcessor;
            this.logProcessor = logProcessor;
            this.targetProcessor = targetProcessor;
            this.eventAggregator = eventAggregator;
            //this.sharlayanHarness = sharlayanHarness;

            loopResetEvent = EventManager.GenerateChildEvent("scan_event");
            StartScan();
        }

        private void OnMemoryScanSettingsChanged(object sender, MemoryScanSettings config)
        {
            this.disableNetworkScan = config.DisableNetworkScan;
        }

        public void StartScan()
        {
            tokenSource?.Dispose();
            tokenSource = new CancellationTokenSource();
            Task.Factory
                .StartNew(() => RunScan(tokenSource.Token), TaskCreationOptions.LongRunning)
                .ContinueWith(tsk =>
                {
                    Logger.Trace(tsk.Exception);
                }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void StopScan()
        {
            tokenSource?.Cancel();
        }

        private void RunScan(CancellationToken token)
        {
            while (true)
            {
                if (this.disableNetworkScan)
                    MicroTimer.Instance.MicroSleepSimple(1);
                else
                {
                    //Our outside service handles verifying everything is attached and verified
                    //Also can be used for verifying that the mainform is visible
                    WaitHandle.WaitAny(new WaitHandle[] { loopResetEvent, token.WaitHandle });
                }
                
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                //this.sharlayanHarness.Refresh();
                this.serverTimeProcessor.Refresh();
                this.zoneProcessor.Refresh();

                if (this.zoneProcessor.ZoneID != 0U)
                    this.mobArrayProcessor.Refresh();
                if (this.zoneProcessor.ZoneID != 0U && this.mobArrayProcessor.PrimaryPlayerPointer != IntPtr.Zero)
                {
                    this.combatantManager.Refresh();
                    eventAggregator.GetEvent<CombatantRefreshEvent>().Publish();

                    this.playerProcessor.Refresh();
                    this.partyProcessor.Refresh();
                    this.logProcessor.Refresh();
                    this.targetProcessor.Refresh();
                }

            }

        }
    }
}

