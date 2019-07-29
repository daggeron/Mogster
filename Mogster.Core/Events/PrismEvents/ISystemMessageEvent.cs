using Mogster.Core.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mogster.Core.Events
{
    public class ISystemMessageEvent : PubSubEvent<ISystemMessage>
    {
    }
}
