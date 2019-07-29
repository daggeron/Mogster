using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mogster.Core.Models
{
    public enum MessageType
    {
        ZoneChanged,
        RawLogLine,
        SystemMessage,
        EntityHasDied,
        CombatantAdded,
        CombatantRemoved,
        CombatStatusChanged,

        StartCasting,

        EffectAdded,
        EffectRemoved,
        IncomingAbility,
        OutgoingAbility,

        PartyWipe,
        DutyWipe,
        DutyComplete,
        DutyLockoutUpdate,
        DutyStart,
        NoOp
    }
}
