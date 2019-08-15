using Mogster.Core.Events;
using Mogster.Core.Events.SystemEvents;
using Mogster.Core.Models;
using System;

namespace Mogster.Core.Helpers
{
    public class BobParser
    {
        //actorId.ToString("X2") + "|" + actorName + "|" + skillId.ToString("X2") + "|" + skillName.ToProperCase() + "|" + targetId.ToString("X2") + "|" + targetName + "|";
        public static NetworkCast ProcessNetworkCastMessage(DateTime serverTime, string logLine)
        {
            String[] chunks = logLine.Split(new[] { '|' }, StringSplitOptions.None);

            if (chunks.Length < 6)
            {
                //throw an exceptions
                return new NetworkCast();
            }

            NetworkCast networkCast = new NetworkCast
            {
                targetID = UInt32.Parse(chunks[4], System.Globalization.NumberStyles.HexNumber),
                targetName = chunks[5],
                actorID = UInt32.Parse(chunks[0], System.Globalization.NumberStyles.HexNumber),
                actorName = chunks[1],
                skillId = UInt32.Parse(chunks[2], System.Globalization.NumberStyles.HexNumber),
                skillName = chunks[3],
                timeOfCast = serverTime
            };

            return networkCast;
        }

        public static CombatDeath ParseNetworkDeath(DateTime serverTime, string logLine)
        {
            String[] chunks = logLine.Split(new[] { '|' }, StringSplitOptions.None);

            if (chunks.Length < 5)
            {
                //throw an exceptions
                return new CombatDeath();
            }

            CombatDeath combatDeath = new CombatDeath
            {
                targetID = UInt32.Parse(chunks[0], System.Globalization.NumberStyles.HexNumber),
                targetName = chunks[1],
                actorID = UInt32.Parse(chunks[2], System.Globalization.NumberStyles.HexNumber),
                actorName = chunks[3],
                timeOfDeath = serverTime
            };

            return combatDeath;
        }

        public static NetworkBuff ParseNetworkEffect(DateTime serverTime, string logLine)
        {
            String[] chunks = logLine.Split(new[] { '|' }, StringSplitOptions.None);

            NetworkBuff networkBuff = new NetworkBuff()
            {
                BuffID = UInt16.Parse(chunks[0], System.Globalization.NumberStyles.HexNumber),
                BuffName = chunks[1],
                Duration = float.Parse(chunks[2]),
                ActorID = UInt32.Parse(chunks[3], System.Globalization.NumberStyles.HexNumber),
                ActorName = chunks[4],
                TargetID = UInt32.Parse(chunks[5], System.Globalization.NumberStyles.HexNumber),
                TargetName = chunks[5]
            };

            return networkBuff;
        }

        internal static ISystemMessage ParseNetwork6D(DateTime serverDate, string logLine)
        {

            String[] chunks = logLine.Split(new[] { '|' }, StringSplitOptions.None);
            String opCode = chunks[1];
            switch (opCode) {
                /*case "40000010":*/
                case "40000012":
                    return new DutyWipe();
                case "40000003":
                    return new DutyComplete();
                case "40000001":
                    return new DutyStart(Convert.ToInt32(chunks[2], 16));
                case "80000004":
                    return new DutyLockoutUpdate(Convert.ToInt32(chunks[2], 16));
                default:
                    return new UnknownOp();
            }

        }
    }
}
