
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Memory;
using Sharlayan;
using Sharlayan.Models;
using System;

namespace Mogster.Core.Harness
{
    public class SharlayanHarness
    {
        private readonly IProcessManager processManager;

        public SharlayanHarness(IProcessManager processManager)
        {
            this.processManager = processManager;

            this.processManager.ProcessChanged += new EventHandler(OnProcessChanged);
        }

        private void OnProcessChanged(object sender, EventArgs args)
        {
            Initialize();
        }

        private void Initialize()
        {
            if (processManager.Current.ClientMode == FFXIVClientMode.Unknown)
            {
                MemoryHandler.Instance.UnsetProcess();
            }
            else
            {
                ProcessModel processModel = new ProcessModel
                {
                    Process = processManager.Current.Process,
                    IsWin64 = true
                };
                MemoryHandler.Instance.SetProcess(processModel, "English", "latest", true);
            }
        }

        internal void Refresh()
        {

        }
    }
}
