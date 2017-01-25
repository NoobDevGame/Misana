using System.Threading;
using System.Threading.Tasks;

namespace Misana.Network
{
    public class MessageWaitObject
    {
        public SemaphoreSlim Semaphore = new SemaphoreSlim(0,1);

        public bool IsLocked { get; private set; }

        private object message;

        internal void Start()
        {
            IsLocked = true;
        }

        public async Task<object> Wait()
        {
            await Semaphore.WaitAsync();
            Semaphore.Release();
            return message;
        }

        public async Task<T> Wait<T>()
            where T : struct
        {
            var message = (T)await Wait();
            return message;
        }

        internal void Release(object message)
        {
            this.message = message;
            Semaphore.Release();
            IsLocked = false;
        }
    }
}