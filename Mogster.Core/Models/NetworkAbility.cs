using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mogster.Core.Models
{
    class NetworkAbility
    {
        public uint actorId;
        public string actorName;
        public uint skillId;
        public string skillName;
        public uint targetId;
        public string targetName;
        public uint? actorCurrentHp;
        public uint? actorMaxHp;
        public uint? actorCurrentMp;
        public uint? actorMaxMp;
        public uint? actorCurrentTp;
        public uint? actorMaxTp;
        public float? actorPosX;
        public float? actorPosY;
        public float? actorPosZ;
        public float? actorHeading;
        public uint? targetCurrentHp;
        public uint? targetMaxHp;
        public uint? targetCurrentMp;
        public uint? targetMaxMp;
        public uint? targetCurrentTp;
        public uint? targetMaxTp;
        public float? targetPosX;
        public float? targetPosY;
        public float? targetPosZ;
        public float? targetHeading;
        public ulong effectData1;
        public ulong effectData2;
        public ulong effectData3;
        public ulong effectData4;
        public ulong effectData5;
        public ulong effectData6;
        public ulong effectData7;
        public ulong effectData8;
    }
}
