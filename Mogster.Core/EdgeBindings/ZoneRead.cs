using FFXIV_ACT_Plugin.Memory;
using FFXIV_ACT_Plugin.Memory.MemoryProcessors;
using FFXIV_ACT_Plugin.Memory.MemoryReader;
using FFXIV_ACT_Plugin.Resource;
using System.Threading.Tasks;
using TinyIoC;
using Mogster.Core.Models;
using FFXIV_ACT_Plugin.Config;
using System;

namespace Mogster.Core.EdgeBindings
{
    public class ZoneRead
    {
        public TinyIoCContainer IoCContainer { get; private set; }
        public IProcessManager ProcessManager { get; private set; }
        public IZoneProcessor ZoneProcessor { get; private set; }

        private System.Timers.Timer processTimer = new System.Timers.Timer(200);

        public ZoneRead()
        {
            this.IoCContainer = new TinyIoCContainer();

            this.IoCContainer.Register<IReadZoneId, ReadZoneId>().AsSingleton();
            this.IoCContainer.Register<IZoneList, ZoneList>().AsSingleton();
            this.IoCContainer.Register<DataEvent>().AsSingleton();
            this.IoCContainer.Register<ISignatureManager, SignatureManager>().AsSingleton();
            this.IoCContainer.Register<IReadMemory, ReadMemory>().AsSingleton();
            this.IoCContainer.Register<IProcessManager, ProcessManager>().AsSingleton();
            this.IoCContainer.Register<ISettingsManager, SettingsManager>().AsSingleton();
            this.IoCContainer.Register<IReadProcesses, ReadProcesses>().AsSingleton();
            this.IoCContainer.Register<IZoneProcessor, ZoneProcessor>().AsSingleton();

            MemoryScanSettings memoryScanSettings = new MemoryScanSettings()
            {
                ProcessID = 0
            };
            this.ProcessManager = IoCContainer.Resolve<IProcessManager>();
            this.ZoneProcessor = IoCContainer.Resolve<IZoneProcessor>();

            this.IoCContainer.Resolve<ISettingsManager>().UpdateMemoryScanSettings(memoryScanSettings);
            
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

        public async Task<object> Invoke(object input)
        {
            if (!ProcessManager.Verify())
            {
                return 0;
            } else
            {
                ZoneProcessor.Refresh();
                return new Zone()
                {
                    ZoneID = ZoneProcessor.ZoneID,
                    ZoneName = ZoneProcessor.ZoneName
                };
            }
        }
    }
}
