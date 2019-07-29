using Mogster.Core.Models;
using System;
using System.Threading.Tasks;

namespace Mogster.Core.IPC
{
    public interface IIPCSystem: IDisposable
    {

        void SendMessage(IPCMessage message);
        void SendMessage<T>(MessageType messageType, T message);

        Task Start();
        void Stop();
    }
}
