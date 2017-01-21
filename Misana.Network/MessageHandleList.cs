using System;

namespace Misana.Network
{
    internal class MessageHandleList
    {
        private MessageHandle[] handles = MessageHandleManager.CreateHandleArray();

        public bool ExistHandle(int index)
        {
            return !(index >= handles.Length || handles[index] == null);
        }

        public MessageHandle GetHandle(int index)
        {
            return handles[index];
        }

        public MessageHandle<T> GetHandle<T>()
            where T : struct
        {
            return (MessageHandle<T>) GetHandle(MessageHandle<T>.Index.Value);
        }

        public void CreateHandle(Type type)
        {
            var id = MessageHandleManager.GetId(type);
            if (id.HasValue)
            {
                if (id >= handles.Length)
                    Array.Resize(ref handles,id.Value+1);

                handles[id.Value] = MessageHandleManager.CreateMessageHandle(type);
            }
        }
    }
}