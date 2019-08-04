using FFXIV_ACT_Plugin.Memory.Models;
using System;
using System.Runtime.InteropServices;

namespace Mogster.Core.Models
{
    // FFXIV_ACT_Plugin.Memory.Models.Combatant64Struct


    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Combatant64Struct
    {
        [FieldOffset(48)]
        public unsafe fixed byte Name[30];

        [FieldOffset(116)]
        public uint ID;

        [FieldOffset(128)]
        public uint BNpcID;

        [FieldOffset(132)]
        public uint OwnerID;

        [FieldOffset(140)]
        public byte Type;

        [FieldOffset(146)]
        public byte EffectiveDistance;

        [FieldOffset(160)]
        public float PosX;

        [FieldOffset(164)]
        public float PosZ;

        [FieldOffset(168)]
        public float PosY;

        [FieldOffset(176)]
        public float Heading;

        [FieldOffset(1000)]
        public uint PCTargetID;

        [FieldOffset(6176)]
        public uint NPCTargetID;

        [FieldOffset(6268)]
        public uint BNpcNameID;

        [FieldOffset(6296)]
        public ushort CurrentWorldID;

        [FieldOffset(6298)]
        public ushort HomeWorldID;

        [FieldOffset(6308)]
        public uint CurrentHP;

        [FieldOffset(6312)]
        public uint MaxHP;

        [FieldOffset(6316)]
        public uint CurrentMP;

        [FieldOffset(6320)]
        public uint MaxMP;

        [FieldOffset(6326)]
        public ushort CurrentGP;

        [FieldOffset(6328)]
        public ushort MaxGP;

        [FieldOffset(6330)]
        public ushort CurrentCP;

        [FieldOffset(6332)]
        public ushort MaxCP;

        [FieldOffset(6364)]
        public byte Job;

        [FieldOffset(6366)]
        public byte Level;

        [FieldOffset(6389)]
        public byte AgroFlags;

        [FieldOffset(6409)]
        public byte CombatFlags;

        [FieldOffset(6440)]
        public IntPtr FlyingTextPointer;

        [FieldOffset(6448)]
        public uint FlyingTextMaxLen;

        [FieldOffset(6464)]
        public uint FlyingTextStart;

        [FieldOffset(6488)]
        public uint FlyingTextEnd;

        [FieldOffset(6504)]
        public unsafe fixed byte Effects[540];

        [FieldOffset(7248)]
        public byte IsCasting1;

        [FieldOffset(7250)]
        public byte IsCasting2;

        [FieldOffset(7252)]
        public uint CastBuffID;

        [FieldOffset(7264)]
        public uint CastTargetID;

        [FieldOffset(7300)]
        public float CastDurationCurrent;

        [FieldOffset(7304)]
        public float CastDurationMax;

        [FieldOffset(7312)]
        public OutgoingAbilityStruct OutgoingAbility;
        [FieldOffset(7616)]
        public unsafe fixed byte IncomingAbilities[3600];

    }

}
