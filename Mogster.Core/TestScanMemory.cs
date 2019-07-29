using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Memory;
using FFXIV_ACT_Plugin.Memory.MemoryProcessors;
using FFXIV_ACT_Plugin.Memory.MemoryReader;
using Microsoft.Extensions.Hosting;
using Mogster.Core.Events;
using Mogster.Core.Helpers;
using Mogster.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Prism.Events;
using Sharlayan;
using Sharlayan.Core;
using Sharlayan.Models;
using Sharlayan.Utilities;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mogster.Core
{

    public class SharlayanService: BackgroundService
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly object _playerLock = new object();
        private readonly IReadCombatant readCombatant;
        private readonly IMobArrayProcessor mobArrayProcessor;
        private readonly IEventAggregator eventAggregator;
        private bool cinCombat = false;

        public static CancellationTokenSource source = new CancellationTokenSource();

        public SharlayanService(IReadCombatant readCombatant, 
            IMobArrayProcessor mobArrayProcessor, 
            IEventAggregator eventAggregator)
        {
            this.readCombatant = readCombatant;
            this.mobArrayProcessor = mobArrayProcessor;
            this.eventAggregator = eventAggregator;
        }


        //private bool RefreshCharacter
        private bool testx()
        {
            var agroCount = MemoryHandler.Instance.GetInt16(Scanner.Instance.Locations[Signatures.AgroCountKey]);
            IntPtr primaryPlayer = mobArrayProcessor.PrimaryPlayerPointer;
            var sourceSize = MemoryHandler.Instance.Structures.ActorItem.SourceSize;
            byte[] source = MemoryHandler.Instance.GetByteArray(primaryPlayer, sourceSize);

            ActorItem entry = ActorItemResolver.ResolveActorFromBytes(source, true);

            var list = mobArrayProcessor.MobArray.Where(p=>p!=IntPtr.Zero).ToActor().Where(p => p.NPCID1 == 0 && p.NPCID2 == 0);
            var max = list.Max(p => p.Distance);
            var result = list.Select(m => new
            {
                Name = m.Name,
                Distance = m.Distance,
                ID = m.ID
            }).Where(a => a.ID != 0);
            var resultt = result.First(p => p.Distance == max);

            return entry.InCombat || agroCount > 0;
        }

        private bool InCombat()
        {
            var agroCount = MemoryHandler.Instance.GetInt16(Scanner.Instance.Locations[Signatures.AgroCountKey]);
            IntPtr primaryPlayer = mobArrayProcessor.PrimaryPlayerPointer;
            var sourceSize = MemoryHandler.Instance.Structures.ActorItem.SourceSize;
            byte[] source = MemoryHandler.Instance.GetByteArray(primaryPlayer, sourceSize);

            ActorItem entry = ActorItemResolver.ResolveActorFromBytes(source, true);

            return entry.InCombat || agroCount > 0;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!(MemoryHandler.Instance.IsAttached &&
                Scanner.Instance.Locations.ContainsKey(Signatures.CharacterMapKey) &&
                Scanner.Instance.Locations.ContainsKey(Signatures.TargetKey)) && 
                !stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(100, stoppingToken);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(25), stoppingToken);

                if (!(MemoryHandler.Instance.IsAttached &&
                Scanner.Instance.Locations.ContainsKey(Signatures.CharacterMapKey) &&
                Scanner.Instance.Locations.ContainsKey(Signatures.TargetKey)))
                    continue;

                bool inCombat = InCombat();
                if (cinCombat != inCombat)
                {
                    eventAggregator.GetEvent<GenericEvent<CombatChange>>().Publish(new CombatChange(inCombat));
                    cinCombat = inCombat;
                }

            }
        }
    }
}


