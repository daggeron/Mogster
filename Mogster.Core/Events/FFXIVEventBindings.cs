
using FFXIV_ACT_Plugin.Common.Models;
using FFXIV_ACT_Plugin.Memory;
using FFXIV_ACT_Plugin.Memory.MemoryProcessors;
using Mogster.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Events;
using System;
using System.Collections.ObjectModel;

namespace Mogster.Core.Events
{
    public class FFXIVEventBindings : IDisposable
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator eventAggregator;
        private readonly DataEvent dataEvent;
        private readonly IZoneProcessor zoneProcessor;

        public FFXIVEventBindings(IEventAggregator eventAggregator, 
                                    DataEvent dataEvent, 
                                    IZoneProcessor zoneProcessor)
        {
            this.eventAggregator = eventAggregator;
            this.dataEvent = dataEvent;
            this.zoneProcessor = zoneProcessor;

            this.BindEvents();
        }

        private void BindEvents()
        {

            Logger.Debug("BindingEvents");

            dataEvent.ZoneChanged += new DataEvent.ZoneChangedDelegate(OnZoneChange);
            dataEvent.PartyListChanged += new DataEvent.PartyListChangedDelegate(OnPartyListChanged);
            dataEvent.CombatantAdded += new DataEvent.CombatantAddedDelegate(OnCombatantAdded);
            dataEvent.CombatantRemoved += new DataEvent.CombatantRemovedDelegate(OnCombatantRemoved);
            dataEvent.OutgoingAbility += new DataEvent.OutgoingAbilityDelegate(OnOutgoingAbility);
            dataEvent.IncomingAbility += new DataEvent.IncomingAbilityDelegate(OnIncomingAbility);

            dataEvent.EffectAdded += new DataEvent.EffectAddedDelegate(OnEffectAdded);

            eventAggregator.GetEvent<EntityHasDiedEvent>().Subscribe(OnEntityDeath);
        }

        private void OnEffectAdded(object effect)
        {
            string json = JsonConvert.SerializeObject(effect, Formatting.Indented);
            Logger.Debug(json);

            eventAggregator.GetEvent<IPCMessageEvent>().Publish(IPCMessage.Get(MessageType.EffectAdded, (Effect)effect));
        }

        private void OnEffectRemoved(object effect)
        {
            eventAggregator.GetEvent<IPCMessageEvent>().Publish(IPCMessage.Get(MessageType.EffectRemoved, (Effect)effect));
        }

        private void OnOutgoingAbility(object ability)
        {
            eventAggregator.GetEvent<IPCMessageEvent>().Publish(IPCMessage.Get(MessageType.OutgoingAbility, (OutgoingAbility)ability));
        }

        private void OnIncomingAbility(object ability)
        {
            eventAggregator.GetEvent<IPCMessageEvent>().Publish(IPCMessage.Get(MessageType.IncomingAbility, (IncomingAbility)ability));
        }

        private void OnCombatantAdded(object combatant)
        {
            FFXIV_ACT_Plugin.Common.Models.Combatant addedCombatant = (FFXIV_ACT_Plugin.Common.Models.Combatant)combatant;
            eventAggregator.GetEvent<IPCMessageEvent>().Publish(IPCMessage.Get(MessageType.CombatantAdded, addedCombatant));
        }
        private void OnCombatantRemoved(object combatant)
        {
            FFXIV_ACT_Plugin.Common.Models.Combatant addedCombatant = (FFXIV_ACT_Plugin.Common.Models.Combatant)combatant;
            eventAggregator.GetEvent<IPCMessageEvent>().Publish(IPCMessage.Get(MessageType.CombatantRemoved, addedCombatant));
        }

        private void OnEntityDeath(CombatDeath combatDeath)
        {
            Logger.Debug("OnEntityDeath");
            //iPCSystem.SendMessage(MessageType.EntityHasDied, combatDeath);
        }


        //This event does not seem to be firing
        private void OnPartyListChanged(ReadOnlyCollection<uint> partyList, int partySize)
        {
            
            Logger.Debug("Partysize: {0}", partySize);
        }
        

        private void OnZoneRefreshRequest()
        {
            Logger.Debug("OnZoneRefreshRequest");
            OnZoneChange(zoneProcessor.ZoneID, zoneProcessor.ZoneName);
        }

        private void OnZoneChange(uint zoneID, string zoneName)
        {
            Logger.Debug("OnZoneChange");

            eventAggregator.GetEvent<IPCMessageEvent>().Publish(IPCMessage.Get(MessageType.ZoneChanged, new Zone()
            {
                ZoneID = zoneID,
                ZoneName = zoneName
            }));
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                dataEvent.ZoneChanged -= new DataEvent.ZoneChangedDelegate(OnZoneChange);
                dataEvent.PartyListChanged -= new DataEvent.PartyListChangedDelegate(OnPartyListChanged);
            }
        }
    }
}
