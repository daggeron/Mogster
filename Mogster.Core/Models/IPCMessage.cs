using FFXIV_ACT_Plugin.Logfile;
using Newtonsoft.Json.Linq;
using Mogster.Core.Events;
using System;

namespace Mogster.Core.Models
{
    public class IPCMessage
    {

        public IPCMessage(MessageType messageType, String message)
        {
            MessageType = messageType;
            this.message = message;
        }

        public static IPCMessage Get<TObject>(MessageType messageType, TObject value)
        {
            JObject message = new JObject
            {
                ["event"] = messageType.ToString(),
                ["payload"] = JToken.FromObject(value)
            };

            return new IPCMessage(messageType, message.ToString());
        }

        public MessageType MessageType { get; set; }

        private readonly String message;

        public override string ToString()
        {
            return message.ToString();
        }

    }
}
