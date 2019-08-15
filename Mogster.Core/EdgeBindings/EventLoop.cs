using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thaliak.Network.Utilities;
using WindowsFirewallHelper;

namespace Mogster.Core.EdgeBindings
{
    class EventLoop
    {
        private Func<object, Task<object>> callBack;


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "CS1998:This async method lacks 'await' operators and will run synchronously.", Justification = "Required by EdgeJS")]
        public async Task<object> Invoke(Func<object, Task<object>> input)
        {
            CheckFirewall();

            callBack = input;



            return true;
        }

        static void CheckFirewall()
        {
            FirewallManager.Instance.Rules
                .Where(item => item.Name.Equals("Mogster", StringComparison.OrdinalIgnoreCase))
                .ToList()
                .ForEach(rule => FirewallManager.Instance.Rules.Remove(rule));

            FirewallRegister.RegisterToFirewall();
        }
    }
}
