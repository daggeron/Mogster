using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mogster.Core.Models
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Combatant
    {
        [FieldOffset(6057)]
        public byte AgroFlags;
        [FieldOffset(6077)]
        public byte CombatFlags;
    }
}
