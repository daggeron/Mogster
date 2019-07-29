using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Mogster.Core.IPC.NativeWebSocket;
using Mogster.Core.Models;
using Mogster.Core.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mogster.Core.IPC
{
    public class NativeWebsocketClient : IHostedService
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, string> options;
        private readonly IEventAggregator eventAggregator;

        public NativeWebsocketClient(IOptions<Dictionary<string, string>> options, IEventAggregator eventAggregator)
        {
            this.options = options.Value;
            this.eventAggregator = eventAggregator;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Client.ConnectAsync(options["uri"], cancellationToken, async (connection) =>
            {
                connection.OnReceive += async message =>
                {
                    switch (message)
                    {
                        case "FFSHUTDOWN":
                            eventAggregator.GetEvent<ShutdownEvent>().Publish();
                            break;
                        case "RESENDZONE":
                            eventAggregator.GetEvent<ZoneRefreshEvent>().Publish();
                            break;
                        default:
                            break;
                    }

                    await Task.CompletedTask;
                };

                connection.OnClose += (a, b) =>
                {
                    eventAggregator.GetEvent<IPCMessageEvent>().Unsubscribe((message) => sendMessage(message, connection));
                    return Task.CompletedTask;
                };

                eventAggregator.GetEvent<IPCMessageEvent>().Subscribe((message) => sendMessage(message, connection));

                await connection.SendAsync("Bob's your uncle");
            }, true, true, 1, (options) =>
            {
                options.KeepAliveInterval = TimeSpan.FromSeconds(30);
            });
        }

        private bool sendMessage(IPCMessage message, Connection connection)
        {
            return connection.SendAsync(message.ToString()).Result;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {

            return Task.CompletedTask;
        }
    }
}
