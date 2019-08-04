using FFXIV_ACT_Plugin.Common.Models;
using FFXIV_ACT_Plugin.Config;
using FFXIV_ACT_Plugin.Memory;
using FFXIV_ACT_Plugin.Memory.MemoryProcessors;
using FFXIV_ACT_Plugin.Memory.MemoryReader;
using FFXIV_ACT_Plugin.Memory.Models;
using FFXIV_ACT_Plugin.Resource;
using Mogster.Core.Events;
using Mogster.Core.Models;
using Prism.Events;
using System;
using System.Text;

namespace Mogster.Core.Harness.FFXIVPlugin
{
    public class CombatantProcessor : ICombatantProcessor
    {
        private bool _disableNetworkScan;
        private readonly DataEvent _dataEvent;
        private readonly IProcessManager _processManager;
        private readonly IReadCombatant _readCombatant;
        private readonly ICombatantAbilityProcessor _combatantAbilityReader;
        private readonly ICombatantEffectProcessor _combatantEffectReader;
        private readonly ICombatantFlyingTextProcessor _combatantFlyingTextReader;
        private readonly IWorldList _worldList;
        private readonly IEventAggregator eventAggregator;

        public CombatantProcessor(
          ISettingsManager settingsManager,
          IReadCombatant readCombatant,
          IProcessManager processManager,
          DataEvent dataEvent,
          ICombatantAbilityProcessor combatantAbilityReader,
          ICombatantEffectProcessor combatantEffectReader,
          ICombatantFlyingTextProcessor combatantFlyingTextReader,
          IWorldList worldList,
          IEventAggregator eventAggregator)
        {
            settingsManager.MemoryScanSettingsChanged += new EventHandler<MemoryScanSettings>(this.OnMemoryScanSettingsChanged);
            this._processManager = processManager;
            this._readCombatant = readCombatant;
            this._dataEvent = dataEvent;
            this._combatantAbilityReader = combatantAbilityReader;
            this._combatantEffectReader = combatantEffectReader;
            this._combatantFlyingTextReader = combatantFlyingTextReader;
            this._worldList = worldList;
            this.eventAggregator = eventAggregator;
        }

        private void OnMemoryScanSettingsChanged(object sender, MemoryScanSettings config)
        {
            this._disableNetworkScan = config.DisableNetworkScan;
        }

        public void RefreshCombatant(Combatant combatant, IntPtr pointer, bool refresh)
        {
            if (pointer != combatant.Pointer)
            {
                if (combatant.Pointer != IntPtr.Zero)
                {
                    eventAggregator.GetEvent<IPCMessageEvent>().Publish(IPCMessage.Get(MessageType.CombatantRemoved, combatant));
                    this.ClearCombatant(combatant);
                }
                if (!(pointer != IntPtr.Zero))
                    return;
                this.RefreshCombatantInternal(combatant, pointer, refresh);
                eventAggregator.GetEvent<IPCMessageEvent>().Publish(IPCMessage.Get(MessageType.CombatantAdded, combatant));
            }
            else
            {
                if (!(pointer != IntPtr.Zero))
                    return;
                int job1 = combatant.Job;
                int level = combatant.Level;
                uint id = combatant.ID;
                string name = combatant.Name;
                this.RefreshCombatantInternal(combatant, pointer, refresh);
                int job2 = combatant.Job;
                if (job1 == job2 && level == combatant.Level && ((int)id == (int)combatant.ID && !(name != combatant.Name)))
                    return;
                eventAggregator.GetEvent<IPCMessageEvent>().Publish(IPCMessage.Get(MessageType.CombatantAdded, combatant));
            }
        }

        private unsafe void RefreshCombatantInternal(
          Combatant oldCombatant,
          IntPtr combatantPointer,
          bool refresh)
        {
            if (this._disableNetworkScan)
                refresh = true;
            if (oldCombatant.Pointer == combatantPointer && !refresh)
                return;
            IntPtr pointer = oldCombatant.Pointer;
            bool flag = false;
            IncomingAbilityStruct* incomingAbilities;
            OutgoingAbilityStruct* outgoingAbilities;
            EffectStruct* effects;
            IntPtr flyingTextPointer;
            uint flyingTextStart;
            uint flyingTextEnd;
            uint flyingTextMaxLen;
            if (this._processManager.Current.ClientMode == FFXIVClientMode.FFXIV_64)
            {
                Mogster.Core.Models.Combatant64Struct* combatant64StructPtr = (Mogster.Core.Models.Combatant64Struct*)(void*)this._readCombatant.Read(combatantPointer);
                string combatantName = this.GetCombatantName(combatant64StructPtr->Name);
                if (combatantName != oldCombatant.Name || (int)oldCombatant.ID != (int)combatant64StructPtr->ID)
                    flag = true;
                oldCombatant.BNpcID = combatant64StructPtr->BNpcID;
                oldCombatant.BNpcNameID = combatant64StructPtr->BNpcNameID;
                oldCombatant.CastBuffID = combatant64StructPtr->CastBuffID;
                oldCombatant.CastDurationCurrent = combatant64StructPtr->CastDurationCurrent;
                oldCombatant.CastDurationMax = combatant64StructPtr->CastDurationMax;
                oldCombatant.CastTargetID = combatant64StructPtr->CastTargetID;
                oldCombatant.CurrentCP = (uint)combatant64StructPtr->CurrentCP;
                oldCombatant.CurrentGP = (uint)combatant64StructPtr->CurrentGP;
                oldCombatant.CurrentHP = combatant64StructPtr->CurrentHP;
                oldCombatant.CurrentMP = combatant64StructPtr->CurrentMP;
                oldCombatant.ID = combatant64StructPtr->ID;
                oldCombatant.IsCasting = combatant64StructPtr->IsCasting1 == (byte)1 && combatant64StructPtr->IsCasting2 == (byte)1;
                oldCombatant.Job = (int)combatant64StructPtr->Job;
                oldCombatant.Level = (int)combatant64StructPtr->Level;
                oldCombatant.MaxCP = (uint)combatant64StructPtr->MaxCP;
                oldCombatant.MaxGP = (uint)combatant64StructPtr->MaxGP;
                oldCombatant.MaxHP = combatant64StructPtr->MaxHP;
                oldCombatant.MaxMP = combatant64StructPtr->MaxMP;
                oldCombatant.Name = combatantName;
                oldCombatant.OwnerID = combatant64StructPtr->OwnerID;
                oldCombatant.PosX = combatant64StructPtr->PosX;
                oldCombatant.PosY = combatant64StructPtr->PosY;
                oldCombatant.PosZ = combatant64StructPtr->PosZ;
                oldCombatant.Heading = combatant64StructPtr->Heading;
                oldCombatant.EffectiveDistance = combatant64StructPtr->EffectiveDistance;
                oldCombatant.TargetID = combatant64StructPtr->PCTargetID == 3758096384U || combatant64StructPtr->PCTargetID == 0U ? (combatant64StructPtr->NPCTargetID == 3758096384U || combatant64StructPtr->NPCTargetID == 0U ? 0U : combatant64StructPtr->NPCTargetID) : combatant64StructPtr->PCTargetID;
                oldCombatant.type = combatant64StructPtr->Type;
                oldCombatant.WorldID = (uint)combatant64StructPtr->HomeWorldID;
                oldCombatant.CurrentWorldID = (uint)combatant64StructPtr->CurrentWorldID;
                incomingAbilities = (IncomingAbilityStruct*)combatant64StructPtr->IncomingAbilities;
                outgoingAbilities = &combatant64StructPtr->OutgoingAbility;
                effects = (EffectStruct*)combatant64StructPtr->Effects;
                flyingTextPointer = combatant64StructPtr->FlyingTextPointer;
                flyingTextStart = combatant64StructPtr->FlyingTextStart;
                flyingTextEnd = combatant64StructPtr->FlyingTextEnd;
                flyingTextMaxLen = combatant64StructPtr->FlyingTextMaxLen;
            }
            else
            {
                Combatant32Struct* combatant32StructPtr = (Combatant32Struct*)(void*)this._readCombatant.Read(combatantPointer);
                string combatantName = this.GetCombatantName(combatant32StructPtr->Name);
                if (combatantName != oldCombatant.Name || (int)oldCombatant.ID != (int)combatant32StructPtr->ID)
                    flag = true;
                oldCombatant.BNpcID = combatant32StructPtr->BNpcID;
                oldCombatant.BNpcNameID = combatant32StructPtr->BNpcNameID;
                oldCombatant.CastBuffID = combatant32StructPtr->CastBuffID;
                oldCombatant.CastDurationCurrent = combatant32StructPtr->CastDurationCurrent;
                oldCombatant.CastDurationMax = combatant32StructPtr->CastDurationMax;
                oldCombatant.CastTargetID = combatant32StructPtr->CastTargetID;
                oldCombatant.CurrentCP = (uint)combatant32StructPtr->CurrentCP;
                oldCombatant.CurrentGP = (uint)combatant32StructPtr->CurrentGP;
                oldCombatant.CurrentHP = combatant32StructPtr->CurrentHP;
                oldCombatant.CurrentMP = combatant32StructPtr->CurrentMP;
                oldCombatant.ID = combatant32StructPtr->ID;
                oldCombatant.IsCasting = combatant32StructPtr->IsCasting1 == (byte)1 && combatant32StructPtr->IsCasting2 == (byte)1;
                oldCombatant.Job = (int)combatant32StructPtr->Job;
                oldCombatant.Level = (int)combatant32StructPtr->Level;
                oldCombatant.MaxCP = (uint)combatant32StructPtr->MaxCP;
                oldCombatant.MaxGP = (uint)combatant32StructPtr->MaxGP;
                oldCombatant.MaxHP = combatant32StructPtr->MaxHP;
                oldCombatant.MaxMP = combatant32StructPtr->MaxMP;
                oldCombatant.Name = combatantName;
                oldCombatant.OwnerID = combatant32StructPtr->OwnerID;
                oldCombatant.PosX = combatant32StructPtr->PosX;
                oldCombatant.PosY = combatant32StructPtr->PosY;
                oldCombatant.PosZ = combatant32StructPtr->PosZ;
                oldCombatant.Heading = combatant32StructPtr->Heading;
                oldCombatant.EffectiveDistance = combatant32StructPtr->EffectiveDistance;
                oldCombatant.TargetID = combatant32StructPtr->PCTargetID == 3758096384U || combatant32StructPtr->PCTargetID == 0U ? (combatant32StructPtr->NPCTargetID == 3758096384U || combatant32StructPtr->NPCTargetID == 0U ? 0U : combatant32StructPtr->NPCTargetID) : combatant32StructPtr->PCTargetID;
                oldCombatant.type = combatant32StructPtr->Type;
                oldCombatant.WorldID = (uint)combatant32StructPtr->HomeWorldID;
                oldCombatant.CurrentWorldID = (uint)combatant32StructPtr->CurrentWorldID;




                incomingAbilities = (IncomingAbilityStruct*)combatant32StructPtr->IncomingAbilities;
                outgoingAbilities = &combatant32StructPtr->OutgoingAbility;
                effects = (EffectStruct*)combatant32StructPtr->Effects;
                flyingTextPointer = new IntPtr((long)combatant32StructPtr->FlyingTextPointer);
                flyingTextStart = combatant32StructPtr->FlyingTextStart;
                flyingTextEnd = combatant32StructPtr->FlyingTextEnd;
                flyingTextMaxLen = combatant32StructPtr->FlyingTextMaxLen;
            }
            oldCombatant.Pointer = combatantPointer;
            oldCombatant.MaxTP = 1000U;
            if (oldCombatant.WorldID == (uint)ushort.MaxValue)
                oldCombatant.WorldID = 0U;
            if (oldCombatant.CurrentWorldID == (uint)ushort.MaxValue)
                oldCombatant.CurrentWorldID = 0U;
            if (oldCombatant.OwnerID == 3758096384U)
                oldCombatant.OwnerID = 0U;
            if (flag)
            {
                oldCombatant.NetworkBuffs.Clear();
                oldCombatant.UnknownNetworkBuffs.Clear();
            }
            if (oldCombatant.type == (byte)1 || oldCombatant.type == (byte)2)
            {
                oldCombatant.WorldName = this._worldList.GetWorldNameById(oldCombatant.WorldID);
                if (!this._disableNetworkScan)
                    return;
                this._combatantAbilityReader.RefreshIncomingAbilities(oldCombatant, incomingAbilities);
                this._combatantAbilityReader.RefreshOutgoingAbilities(oldCombatant, outgoingAbilities);
                this._combatantEffectReader.RefreshForCombatant(oldCombatant, effects);
                this._combatantFlyingTextReader.Process(oldCombatant, flyingTextPointer, flyingTextStart, flyingTextEnd, flyingTextMaxLen);
            }
            else
                oldCombatant.WorldName = "";
        }

        private void ClearCombatant(Combatant combatant)
        {
            combatant.PartyType = PartyTypeEnum.None;
            combatant.BNpcID = 0U;
            combatant.BNpcNameID = 0U;
            combatant.CastBuffID = 0U;
            combatant.CastDurationCurrent = 0.0f;
            combatant.CastDurationMax = 0.0f;
            combatant.CastTargetID = 0U;
            combatant.CurrentCP = 0U;
            combatant.CurrentGP = 0U;
            combatant.CurrentHP = 0U;
            combatant.CurrentMP = 0U;
            combatant.CurrentTP = 0U;
            combatant.ID = 0U;
            combatant.IsCasting = false;
            combatant.Job = 0;
            combatant.Level = 0;
            combatant.MaxCP = 0U;
            combatant.MaxGP = 0U;
            combatant.MaxHP = 0U;
            combatant.MaxMP = 0U;
            combatant.MaxTP = 0U;
            combatant.Name = (string)null;
            combatant.OwnerID = 0U;
            combatant.Pointer = IntPtr.Zero;
            combatant.PosX = 0.0f;
            combatant.PosY = 0.0f;
            combatant.PosZ = 0.0f;
            combatant.Heading = 0.0f;
            combatant.EffectiveDistance = (byte)0;
            combatant.TargetID = 0U;
            combatant.type = (byte)0;
            combatant.WorldID = 0U;
            combatant.CurrentWorldID = 0U;
            combatant.NetworkBuffs.Clear();
            combatant.UnknownNetworkBuffs.Clear();
            combatant.OutgoingAbility.ActorID = 0U;
            combatant.OutgoingAbility.SequenceID = 0U;
            combatant.OutgoingAbility.SkillID = 0U;
            combatant.OutgoingAbility.TargetID = 0U;
        }

        private unsafe string GetCombatantName(byte* data)
        {
            byte[] bytes = new byte[40];
            for (int count = 0; count < bytes.Length; ++count)
            {
                if (data[count] == (byte)0)
                    return Encoding.UTF8.GetString(bytes, 0, count);
                bytes[count] = data[count];
            }
            return (string)null;
        }
    }
}
