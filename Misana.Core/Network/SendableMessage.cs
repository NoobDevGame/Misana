using Misana.Serialization;

namespace Misana.Core.Network {
    public class SendableMessage
    {
        public NetworkMessage Message;
        public bool Tcp;
        public byte MessageId;
        public int MessageType;
        public Serialize<NetworkMessage> Serialize;
    }

    public class SendableServerMessage
    {
        public NetworkMessage Message;
        public bool Tcp;
        public byte MessageId;
        public int MessageType;
        public Serialize<NetworkMessage> Serialize;
        public IClientOnServer Client;
    }
}