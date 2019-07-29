using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mogster.Core.IPC.NativeWebSocket
{
    public static class Client
    {
        /// <summary>
        /// Creates and starts a new websocket listening client.
        /// </summary>
        /// <param name="uri">The target URI of the format: "ws://(address)/[path]".</param>
        /// <param name="token">Cancellation token.</param>
        /// <param name="onConnect">Action executed when connection is established.</param>
        /// <param name="reconnectOnError">True to reconnect on error, false otherwise.
        /// <para>If true, exceptions will not be thrown. Set to false when debugging.</para>
        /// </param>
        /// <param name="reconnectOnClose">True to reconnect on normal close request, false otherwise.</param>
        /// <param name="secondsBetweenReconnect">The number of seconds between two reconnect attempts.</param>
        /// <param name="setOptions">Websocket option set method.</param>
        /// <returns>Client task.</returns>
        /// <exception cref="Exception">Socket connection exception thrown in case when <paramref name="reconnectOnError"/> and <paramref name="reconnectOnClose"/> is set to false.</exception>
        public static async Task ConnectAsync(string uri, CancellationToken token, Action<Connection> onConnect, bool reconnectOnError = false,
                                              bool reconnectOnClose = false, int secondsBetweenReconnect = 0, Action<ClientWebSocketOptions> setOptions = null)
        {
            var isClosedSuccessfully = true;
            var shouldReconnect = false;

            do
            {
                try
                {
                    isClosedSuccessfully = await connectAsync(uri, token, onConnect, setOptions);
                }
                catch (Exception)
                {
                    isClosedSuccessfully = false;
                    if (!reconnectOnError && !reconnectOnClose) throw;
                }

                if (token.IsCancellationRequested)
                    break;

                shouldReconnect = (!isClosedSuccessfully && reconnectOnError) || reconnectOnClose;
                if (shouldReconnect)
                    await Task.Delay(TimeSpan.FromSeconds(secondsBetweenReconnect));
            }
            while (shouldReconnect);
        }

        static async Task<bool> connectAsync(string uri, CancellationToken token, Action<Connection> onConnection, Action<ClientWebSocketOptions> setOptions = null)
        {
            ClientWebSocket webSocket = null;
            var isClosedSuccessfully = true;

            try
            {
                webSocket = new ClientWebSocket();
                
                setOptions?.Invoke(webSocket.Options);
                await webSocket.ConnectAsync(new Uri(uri), token);
            }
            catch (Exception)
            {
                webSocket?.Dispose();
                throw;
            }

            var connection = new Connection(webSocket);
            try
            {
                onConnection(connection);
                await connection.ListenReceiveAsync(token);
            }
            finally
            {
                isClosedSuccessfully = webSocket.State != WebSocketState.Aborted;
                webSocket?.Dispose();
            }

            return isClosedSuccessfully;
        }

    }
}
