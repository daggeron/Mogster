using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mogster.Core.Models
{
    public class NetworkCast
    {
        public uint targetID;
        public String targetName;

        public uint actorID;
        public String actorName;

        public uint skillId;
        public String skillName;

        public DateTime timeOfCast;
    }
}
