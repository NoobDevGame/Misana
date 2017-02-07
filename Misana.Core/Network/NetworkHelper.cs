using System;
using System.Net;
using System.Net.Sockets;
using Misana.Serialization;

namespace Misana.Core.Network
{
    public delegate void DataHandler(byte[] buffer, ref int processed);
    public class NetworkHelper
    {
        public const int InitialTcpBufferSize = 1024 * 1024;
        public const int InitialUdpBufferSize = 1536 * 2;

        public static void Serialize(SendableMessage msg, ref byte[] buffer, ref int index)
        {
            Serializer.EnsureSize(ref buffer, index + 128 + 1);
            var writeLengthTo = index;
            index += 4;
            Serializer.WriteByte(msg.MessageId, ref buffer, ref index);
            Serializer.WriteInt32(msg.MessageType, ref buffer, ref index);
            msg.Serialize(msg.Message, ref buffer, ref index);
            Serializer.WriteInt32(index - writeLengthTo + 1 - 4, ref buffer, ref writeLengthTo);
        }

        public const int UdpCutoff = 1408;

        public static void MaybeSendUdp(Socket socket, ref byte[] buffer, ref int index, ref int lastMessageEnd, EndPoint endPoint)
        {
            if (index >= UdpCutoff)
            {
                if (lastMessageEnd == -1)
                {
                    socket.SendTo(buffer, 0, index + 1, SocketFlags.None, endPoint);
                    index = 0;
                }
                else
                {
                    socket.SendTo(buffer, 0, lastMessageEnd + 1, SocketFlags.None, endPoint);
                    Buffer.BlockCopy(buffer, lastMessageEnd, buffer, 0, index - lastMessageEnd);

                    index = index - lastMessageEnd;
                    lastMessageEnd = index;
                }
            }
            else
            {
                lastMessageEnd = index;
            }
        }

        public static void ProcessData(DataHandler handler, byte[] buffer, int read, ref bool overflown, ref int expectedLength, byte[] overflow, ref int overflowIndex)
        {
            if (overflown)
            {
                Buffer.BlockCopy(buffer, 0, overflow, overflowIndex, read);
                overflowIndex += read;

                if (overflowIndex < expectedLength)
                {
                    return;
                }

                overflown = false;
                read = overflowIndex;
                overflowIndex = 0;
                expectedLength = 0;
                buffer = overflow;
            }

            var processed = 0;

            while (true)
            {
                var len = Deserializer.ReadInt32(buffer, ref processed);

                if (read - processed < len)
                {
                    overflown = true;
                    expectedLength = len + 4;
                    Buffer.BlockCopy(buffer, processed - 4, overflow, 0, read - processed + 4);
                    overflowIndex = read - processed + 4;
                    return;
                }

                handler(buffer, ref processed);

                if (processed >= len)
                {
                    break;
                }
            }
        }
    }
}