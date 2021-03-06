﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Misana.Network
{
    internal class UdpListnerClient : INetworkClient
    {
        private readonly NetworkListener _listener;
        public int NetworkId { get; } = NetworkManager.GetNextId();

        public bool IsConnected { get; private set; }
        public bool CanSend { get; } = true;

        public UdpClient UdpClient { get; private set; }

        public UdpListnerClient(NetworkListener listener)
        {
            _listener = listener;
        }

        public Task Connect(IPAddress addr)
        {
            if (addr != IPAddress.Any)
                throw new ArgumentException();

            UdpClient = new UdpClient(new IPEndPoint(IPAddress.Any, NetworkManager.ServerUdpPort));
            UdpClient.BeginReceive(OnReadUdpData, null);
            IsConnected = true;

            return Task.CompletedTask;
        }

        public void Disconnect()
        {
            IsConnected = false;
        }


        private void OnReadUdpData(IAsyncResult ar)
        {
            byte[] data;
            IPEndPoint sender = null;

            try
            {
                data = UdpClient.EndReceive(ar, ref sender);
            }
            catch (Exception e)
            {
                data = null;
                Console.WriteLine(e);
                throw;
            }

            try
            {
                UdpClient.BeginReceive(OnReadUdpData, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            if(data != null)
                ReceiveData(data,_listener.GetClientByIp(sender.Address));
        }

        private void ReceiveData(byte[] data,NetworkClient client)
        {
            client.ReceiveData(data);
        }

        public void RegisterOnMessageCallback<T>(MessageReceiveCallback<T> callback)
            where T : struct
        {
        }


        public void SendMessage<T>(ref T message) where T : struct
        {
            throw new NotSupportedException();
        }

        public MessageWaitObject SendRequestMessage<T>(ref T message) where T : struct
        {
            throw new NotSupportedException();
        }

        public void SendResponseMessage<T>(ref T message, byte messageid) where T : struct
        {
            throw new NotSupportedException();
        }

        public bool TryGetMessage<T>(out T message, out INetworkIdentifier senderClient) where T : struct
        {
            message = default(T);
            senderClient = null;
            return false;
        }

        public bool TryGetMessage<T>(out T message) where T : struct
        {
            message = default(T);
            return false;
        }

        public void SendTcpBytes(byte[] bytes)
        {
            throw new NotSupportedException();
        }

        public void SendUdpBytes(byte[] bytes)
        {
            throw new NotSupportedException();
        }
    }
}