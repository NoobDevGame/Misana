namespace Misana.Core.Network {
    public interface IOutgoingMessageQueue
    {
        void Enqueue<T>(T msg) where T : NetworkMessage, IGameMessage;
    }
}