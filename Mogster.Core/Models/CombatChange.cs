using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mogster.Core.Models
{
    public class CombatChange
    {
        public CombatChange(bool inCombat)
        {
            this.InCombat = inCombat;
        }
        public bool InCombat;
    }
}
