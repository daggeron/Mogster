using Mogster.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mogster.Core.Events.SystemEvents
{
    class UnknownOp : ISystemMessage
    {
        public MessageType MessageType => MessageType.DutyComplete;
    }
}
