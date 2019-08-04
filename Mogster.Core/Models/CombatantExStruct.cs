using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mogster.Core.Models
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct CombatantExStruct
    {
        [FieldOffset(6389)]
        public byte AgroFlags;
        [FieldOffset(6409)]
        public byte CombatFlags;
    }
}
