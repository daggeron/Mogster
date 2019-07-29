using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mogster.Core.Models;

namespace Mogster.Core.Events.SystemEvents
{
    class DutyLockoutUpdate : ISystemMessage
    {
        public DutyLockoutUpdate(int time)
        {
            Time = time;
        }

        public MessageType MessageType => MessageType.DutyLockoutUpdate;

        public int Time { get; }
    }
}
