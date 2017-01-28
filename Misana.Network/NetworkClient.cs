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

        private TcpClient _tcpClient;
        private UdpClient _udpSendClient;
        private UdpClient _udpReceiveClient;


        private NetworkStream stream;

        public bool CanConnect { get; private set; }
        public bool IsConnected { get; private set; }

        private IPAddress remoteIp;
        private int sendPort;

        byte[] tcpBuffer = new byte[1024];
        byte[] udpBuffer = new byte[1024];


        public NetworkClient()
        {
            sendPort = NetworkManager.LocalUdpPort;
            CanConnect = true;
        }

        internal NetworkClient(TcpClient tcpClient)
        {
            remoteIp = ((IPEndPoint) tcpClient.Client.RemoteEndPoint).Address;
            sendPort = NetworkManager.ServerUdpPort;

            _tcpClient = tcpClient;
            stream = tcpClient.GetStream();
            _udpSendClient = new UdpClient();
            _udpReceiveClient = new UdpClient(new IPEndPoint(remoteIp,NetworkManager.LocalUdpPort));

            CanConnect = false;
            IsConnected = true;

            StartRead();
        }

        private void StartRead()
        {
            stream.BeginRead(tcpBuffer, 0, 4, OnReadtcpLenght, null);
            _udpReceiveClient.BeginReceive(onReadUdp,null);
        }

        private void onReadUdp(IAsyncResult ar)
        {
            IPEndPoint sender = null;
            var data = _udpReceiveClient.EndReceive(ar,ref sender);


            ReceiveData(data);
            _udpReceiveClient.BeginReceive(onReadUdp,null);
        }

        private void OnReadtcpLenght(IAsyncResult ar)
        {

            var dataCount = stream.EndRead(ar);
            var lenght = BitConverter.ToInt32(tcpBuffer, 0);

            stream.BeginRead(tcpBuffer, 0, lenght, OnReadTcpData, null);
        }

        private void OnReadTcpData(IAsyncResult ar)
        {
            var dataCount = stream.EndRead(ar);

            byte[] data = new byte[dataCount];
            Array.Copy(tcpBuffer,data,dataCount);
            ReceiveData(data);


            stream.BeginRead(tcpBuffer, 0, 4, OnReadtcpLenght, null);
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
            if (MessageHandle<T>.IsUDPMessage)
            {
                WriteUDPData(ref data);
            }
            else
            {
                WriteTCPData(ref data);
            }


            return waitObject;
        }

        public void SendResponseMessage<T>(ref T message,byte messageid) where T : struct
        {
            if (!IsConnected)
                throw new InvalidOperationException("Client is not connected");

            var index = MessageHandle<T>.Index;

            var data = MessageHandle<T>.Serialize(ref message,messageid);

            WriteTCPData(ref data);
        }

        private void WriteTCPData( ref byte[] data)
        {
            byte[] sendData = new byte[data.Length + 4];
            var lengthBytes = BitConverter.GetBytes(data.Length);

            Array.Copy(lengthBytes,sendData,lengthBytes.Length);
            Array.Copy(data,0,sendData,lengthBytes.Length,data.Length);

            stream.BeginWrite(sendData, 0, sendData.Length, null, null);
        }

        private void WriteUDPData( ref byte[] data)
        {

            _udpSendClient.BeginSend(data, data.Length, new IPEndPoint(remoteIp, sendPort), null, null);
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

        private async Task Connect(IPEndPoint endPoint)
        {
            if (IsConnected || !CanConnect)
                throw new InvalidOperationException("Client is connected");

            remoteIp = endPoint.Address;

            _tcpClient = new TcpClient();

            await _tcpClient.ConnectAsync(endPoint.Address, endPoint.Port);
            stream = _tcpClient.GetStream();

            _udpSendClient = new UdpClient(new IPEndPoint(endPoint.Address,NetworkManager.ServerUdpPort));
            _udpReceiveClient = new UdpClient();

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

            _tcpClient.Close();
        }



    }
}