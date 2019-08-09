using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mogster.Core.Models;

namespace Mogster.Core.Events.SystemEvents
{
    class DutyWipe : ISystemMessage
    {
        public MessageType MessageType => MessageType.DutyWipe;
    }
}
