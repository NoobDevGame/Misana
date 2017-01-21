using System;

namespace Misana.Network
{
    internal class HandleList
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