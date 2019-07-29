using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using FFXIV_ACT_Plugin.Common;
using FFXIV_ACT_Plugin.Common.Models;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Memory;
using FFXIV_ACT_Plugin.Memory.MemoryProcessors;
using Mogster.Core.Helpers;

namespace Mogster.Core.Harness.FFXIVPlugin
{
    // Token: 0x0200000D RID: 13
    public class ScanHPPercent
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private CancellationTokenSource tokenSource;
        private AutoResetEvent loopResetEvent;

        // Token: 0x06000088 RID: 136 RVA: 0x0000414C File Offset: 0x0000234C
        public ScanHPPercent(ISettingsManager settingsManager, IServerTimeProcessor serverTime, ICombatantManager combatantManager)
        {
            settingsManager.MemoryScanSettingsChanged += this.OnMemoryScanSettingsChanged;
            this._serverTime = serverTime;
            this._combatantManager = combatantManager;
            loopResetEvent = EventManager.GenerateChildEvent("scan_event");
        }

        // Token: 0x06000089 RID: 137 RVA: 0x0000417C File Offset: 0x0000237C
        private void OnMemoryScanSettingsChanged(object sender, MemoryScanSettings config)
        {
            this._enableScanHPPercent = config.EnableScanHPPercent;
        }

        // Token: 0x0600008A RID: 138 RVA: 0x0000418A File Offset: 0x0000238A
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

        // Token: 0x0600008B RID: 139 RVA: 0x000041A3 File Offset: 0x000023A3
        public void StopScan()
        {
            tokenSource?.Cancel();
        }

        // Token: 0x0600008C RID: 140 RVA: 0x000041C0 File Offset: 0x000023C0
        private void RunScan(CancellationToken token)
        {
            Tuple<uint, int>[] array = null;

            while (true)
            {
                WaitHandle.WaitAny(new WaitHandle[] { loopResetEvent, token.WaitHandle });

                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                if (this._enableScanHPPercent)
                {
                    this._serverTime.Refresh();
                    DateTime serverTime = this._serverTime.ServerTime;
                    if (array == null)
                    {
                        array = new Tuple<uint, int>[this._combatantManager.Combatants.Count];
                        for (int i = 0; i < this._combatantManager.Combatants.Count; i++)
                        {
                            array[i] = new Tuple<uint, int>(this._combatantManager.Combatants[i].ID, (int)(100.0 * this._combatantManager.Combatants[i].CurrentHP / ((this._combatantManager.Combatants[i].MaxHP < 0u) ? 1u : this._combatantManager.Combatants[i].MaxHP)));
                        }
                    }
                    for (int j = 0; j < this._combatantManager.Combatants.Count; j++)
                    {
                        Combatant combatant = this._combatantManager.Combatants[j];
                        if (array[j].Item1 != combatant.ID)
                        {
                            array[j] = new Tuple<uint, int>(combatant.ID, 0);
                        }
                        if (combatant.type == 1 || combatant.type == 2)
                        {
                            Tuple<uint, int> tuple = array[j];
                            int num = (int)(100.0 * combatant.CurrentHP / ((combatant.MaxHP <= 0u) ? 1u : combatant.MaxHP));
                            int item = tuple.Item2;
                            array[j] = new Tuple<uint, int>(combatant.ID, num);
                            if (item > num)
                            {
                                for (int k = item - 1; k >= ((num > 0) ? num : 0); k--)
                                {
                                    string logLine = string.Concat(new string[]
                                    {
                                            13.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'),
                                            "|",
                                            serverTime.ToString("O"),
                                            "|",
                                            combatant.Name,
                                            " HP at ",
                                            k.ToString(CultureInfo.InvariantCulture),
                                            "%."
                                    });
                                    ACTWrapper.ParseRawLogLine(false, serverTime, logLine);
                                }
                            }
                            else if (item != num && num > 0)
                            {
                                string logLine2 = string.Concat(new string[]
                                {
                                        13.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'),
                                        "|",
                                        serverTime.ToString("O"),
                                        "|",
                                        combatant.Name,
                                        " HP at ",
                                        num.ToString(CultureInfo.InvariantCulture),
                                        "%."
                                });
                                ACTWrapper.ParseRawLogLine(false, serverTime, logLine2);
                            }
                        }
                    }
                }
            }
        }

        // Token: 0x0400004B RID: 75
        private IServerTimeProcessor _serverTime;

        // Token: 0x0400004C RID: 76
        private ICombatantManager _combatantManager;

        // Token: 0x0400004E RID: 78
        private bool _enableScanHPPercent;
    }
}
