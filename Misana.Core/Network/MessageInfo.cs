using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Misana.Serialization;

namespace Misana.Core.Network
{
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    public static class MessageInfo<T> where T : NetworkMessage
    {
        public static int Index;
        public static bool IsRpcMessage;
        public static bool IsUdp;
        public static Serialize<NetworkMessage> SerializeBase;
        public static Serialize<T> Serialize;
        public static Deserialize<NetworkMessage> DeserializeBase;
        public static Deserialize<T> Deserialize;

        private static int _lastMessageId;

        public static byte NextMessageId()
        {
            return (byte)(Interlocked.Increment(ref _lastMessageId) & 255);
        }
    }

    public static class MessageInfo
    {
        public static bool[] IsUdp;
        public static Serialize<NetworkMessage>[] Serializers;
        public static Deserialize<NetworkMessage>[] Deserializers;
        public static bool[] IsRpcMessage;
    }
}