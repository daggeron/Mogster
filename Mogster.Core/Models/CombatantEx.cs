using FFXIV_ACT_Plugin.Common.Models;
using System.Collections.Generic;

namespace Mogster.Core.Models
{
    /// <summary>
    /// Extended Combatant class that uses some Sharlayan structures.
    /// https://github.com/FFXIVAPP/sharlayan/blob/2ae237b805f25e4943c4c601376f8109766de06d/Sharlayan/Core/ActorItem.cs
    /// </summary>
    public class CombatantEx : Combatant
    {
        public byte AgroFlags { get; set; }
        public byte CombatFlags { get; set; }

        public bool InCombat => (this.CombatFlags & (1 << 1)) != 0;
        public bool IsAggressive => (this.CombatFlags & (1 << 0)) != 0;
        public bool IsAgroed => (this.AgroFlags & 1) > 0;

        public new IncomingAbility[] IncomingAbilities { get; set; }
        public new OutgoingAbility OutgoingAbility { get; set; }
        public new Effect[] Effects { get; set; }
        public new FlyingText FlyingText { get; set; }
        public new List<FFXIV_ACT_Plugin.Common.Models.NetworkBuff> NetworkBuffs { get; set; }
        public new List<FFXIV_ACT_Plugin.Common.Models.NetworkBuff> UnknownNetworkBuffs { get; set; }

        public static CombatantEx Ingest(Combatant combatant)
        {
            CombatantEx combatantEx = new CombatantEx
            {
                PosY = combatant.PosY,
                PosZ = combatant.PosZ,
                Heading = combatant.Heading,
                CurrentWorldID = combatant.CurrentWorldID,
                WorldID = combatant.WorldID,
                WorldName = combatant.WorldName,
                BNpcNameID = combatant.BNpcNameID,
                BNpcID = combatant.BNpcID,
                TargetID = combatant.TargetID,
                EffectiveDistance = combatant.EffectiveDistance,
                PartyType = combatant.PartyType,
                Order = combatant.Order,
                IncomingAbilities = combatant.IncomingAbilities,
                OutgoingAbility = combatant.OutgoingAbility,
                Effects = combatant.Effects,
                FlyingText = combatant.FlyingText,
                PosX = combatant.PosX,
                CastDurationMax = combatant.CastDurationMax,
                CastDurationCurrent = combatant.CastDurationCurrent,
                CastTargetID = combatant.CastTargetID,
                ID = combatant.ID,
                OwnerID = combatant.OwnerID,
                type = combatant.type,
                Job = combatant.Job,
                Level = combatant.Level,
                Name = combatant.Name,
                CurrentHP = combatant.CurrentHP,
                MaxHP = combatant.MaxHP,
                NetworkBuffs = combatant.NetworkBuffs,
                CurrentMP = combatant.CurrentMP,
                CurrentCP = combatant.CurrentCP,
                MaxCP = combatant.MaxCP,
                CurrentGP = combatant.CurrentGP,
                MaxGP = combatant.MaxGP,
                IsCasting = combatant.IsCasting,
                CastBuffID = combatant.CastBuffID,
                MaxMP = combatant.MaxMP,
                UnknownNetworkBuffs = combatant.UnknownNetworkBuffs
            };

            return combatantEx;
        }
    }
}
