using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mogster.Core.Harness.FFXIVPlugin
{
    public struct Server_MessageHeader
    {
        public uint MessageLength;
        public uint ActorID;
        public uint LoginUserID;
        public uint Unknown1;
        public ushort Unknown2;
        public Server_MessageType MessageType;
        public uint Unknown3;
        public uint Seconds;
        public uint Unknown4;
    }
}
