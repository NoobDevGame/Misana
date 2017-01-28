using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Misana.Network
{
    internal class NetworkClient : INetworkClient
    {

        public int NetworkId { get; } = NetworkManager.GetNextId();


        private MessageHandleList _messageHandles = new MessageHandleList();

        private TcpClient _client;
        private NetworkStream stream;

        public bool CanConnect { get; private set; }
        public bool IsConnected { get; private set; }

        byte[] buffer = new byte[1024];

        public NetworkClient()
        {
            _client = new TcpClient();
            CanConnect = true;
        }

        internal NetworkClient(TcpClient tcpClient)
        {
            _client = tcpClient;
            stream = tcpClient.GetStream();
            CanConnect = false;
            IsConnected = true;

            StartRead();
        }

        private void StartRead()
        {
            stream.BeginRead(buffer, 0, 4, OnReadLenght, null);
        }

        private void OnReadLenght(IAsyncResult ar)
        {

            var dataCount = stream.EndRead(ar);
            var lenght = BitConverter.ToInt32(buffer, 0);

            stream.BeginRead(buffer, 0, lenght, OnReadData, null);
        }

        private void OnReadData(IAsyncResult ar)
        {
            var dataCount = stream.EndRead(ar);

            byte[] data = new byte[dataCount];
            Array.Copy(buffer,data,dataCount);
            ReceiveData(data);


            StartRead();
        }

        private void ReceiveData(byte[] data)
        {
            var header = MessageHandle.DeserializeHeader(ref data);
            var index = header.MessageTypeIndex;

            if (!_messageHandles.ExistHandle(index))
            {
                return;
            }

            var handle = _messageHandles.GetHandle(index);
            var message = handle.Derserialize(ref data);

            handle.SetMessage(message,header,this);
        }

        public void RegisterOnMessageCallback<T>(MessageReceiveCallback<T> callback)
            where T : struct
        {
            _messageHandles.GetHandle<T>().RegisterCallback(callback);
        }

        public void SendMessage<T>(ref T message) where T : struct
        {
            if (!IsConnected)
                throw new InvalidOperationException("Client is not connected");

            SendRequestMessage(ref message);
        }

        public MessageWaitObject SendRequestMessage<T>(ref T message) where T : struct
        {
            if (!IsConnected)
                throw new InvalidOperationException("Client is not connected");

            MessageWaitObject waitObject = null;

            var data = MessageHandle<T>.Serialize(ref message,out waitObject);

            waitObject?.Start();

            WriteData(ref data);

            return waitObject;
        }

        public void SendResponseMessage<T>(ref T message,byte messageid) where T : struct
        {
            if (!IsConnected)
                throw new InvalidOperationException("Client is not connected");

            var index = MessageHandle<T>.Index;

            var data = MessageHandle<T>.Serialize(ref message,messageid);

            WriteData(ref data);
        }

        private void WriteData( ref byte[] data)
        {
            byte[] sendData = new byte[data.Length + 4];
            var lengthBytes = BitConverter.GetBytes(data.Length);

            Array.Copy(lengthBytes,sendData,lengthBytes.Length);
            Array.Copy(data,0,sendData,lengthBytes.Length,data.Length);

            stream.BeginWrite(sendData, 0, sendData.Length, null, null);
        }

        public bool TryGetMessage<T>(out T message, out INetworkIdentifier senderClient)
            where T : struct
        {
            if (!IsConnected)
                throw new InvalidOperationException("Client is not connected");

            var index = MessageHandle<T>.Index;

            var handler = (MessageHandle<T>)_messageHandles.GetHandle(index.Value);
            if (handler == null)
            {
                senderClient = null;
                message = default(T);
                return false;
            }
            
            return handler.TryGetValue(out message,out senderClient);
        }

        public bool TryGetMessage<T>(out T message) where T : struct
        {
            if (!IsConnected)
                throw new InvalidOperationException("Client is not connected");

            INetworkIdentifier client;
            return TryGetMessage(out message, out client);
        }

        public async Task Connect(IPEndPoint endPoint)
        {
            if (IsConnected || !CanConnect)
                throw new InvalidOperationException("Client is connected");

            await _client.ConnectAsync(endPoint.Address, endPoint.Port);
            stream = _client.GetStream();
            StartRead();

            IsConnected = true;
        }

        public Task Connect(IPAddress addr)
        {
            return Connect(new IPEndPoint(addr, NetworkManager.TcpPort));
        }

        public void Disconnect()
        {
            if (!IsConnected)
                return;

            _client.Close();
        }



    }
}