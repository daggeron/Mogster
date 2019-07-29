using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mogster.Core.Models
{
    public class CombatDeath
    {
        public uint targetID;
        public String targetName;

        public uint actorID;
        public String actorName;

        public DateTime timeOfDeath;
    }
}
