using System;
using System.Collections.Generic;
using System.Threading;

namespace Mogster.Core.Helpers
{
    public class EventManager
    {
        private static object _oLockEvents = new object();
        private static Dictionary<string, List<AutoResetEvent>> _dEvents = new Dictionary<string, List<AutoResetEvent>>();

        public static AutoResetEvent GenerateChildEvent(string eventName)
        {
            AutoResetEvent autoResetEventChild = new AutoResetEvent(false);

            lock (_oLockEvents)
            {
                if (!_dEvents.ContainsKey(eventName))
                {
                    _dEvents.Add(eventName, new List<AutoResetEvent>());
                }
                _dEvents[eventName].Add(autoResetEventChild);
            }

            return autoResetEventChild;
        }

        public static void ReleaseChildEvent(AutoResetEvent autoResetEventChild)
        {
            lock (_oLockEvents)
            {
                foreach (List<AutoResetEvent> lAutoResetEvents in _dEvents.Values)
                {
                    if (lAutoResetEvents.Contains(autoResetEventChild))
                    {
                        lAutoResetEvents.Remove(autoResetEventChild);
                        break;
                    }
                }
            }
        }

        public static void SetAll(string eventName)
        {
            lock (_oLockEvents)
            {
                if (_dEvents.ContainsKey(eventName))
                {
                    foreach (AutoResetEvent autoResetEventChild in _dEvents[eventName])
                    {
                        autoResetEventChild.Set();
                    }
                }
            }
        }
    }
}
