using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Misana.Core.Ecs.Changes
{
    internal class EntitesWithChanges
    {
        private readonly EntityManager _manager;
        private static readonly ConcurrentStack<List<EntityChange>> ChangeStack = new ConcurrentStack<List<EntityChange>>();
        
        static EntitesWithChanges()
        {
            for (int i = 0; i < 128; i++)
            {
                ChangeStack.Push(new List<EntityChange>(16));
            }
        }
        const int ThreadCount = 0;
        internal EntitesWithChanges(EntityManager manager)
        {
            _manager = manager;

            _threads = new Thread[ThreadCount];
            _workAvailable = new AutoResetEvent[ThreadCount];


            for (int i = 0; i < ThreadCount; i++)
            {
                _threads[i] = new Thread(CommitWorker) { IsBackground = true };
                _workAvailable[i] = new AutoResetEvent(false);
                _threads[i].Start(i);
            }

            _workDone = new AutoResetEvent(false);
        }

        private void CommitWorker(object o)
        {
            var i = (int)o;

            while (true)
            {
                _workAvailable[i].WaitOne();
                CommitWork(i);
                if (Interlocked.Increment(ref _workDoneCounter) >= ThreadCount)
                    _workDone.Set();
            }
        }

        private void CommitWork(int number)
        {
            foreach (var changes in _changeLists) {
                if (changes[0].EntityId % (ThreadCount + 1) == number)
                {
                    for (int j = 0; j < changes.Count; j++)
                    {
                        for (int i = j + 1; i < changes.Count; i++)
                        {
                            if (changes[i].Index == changes[j].Index)
                            {
                                if (!changes[j].Addition || !changes[i].Addition)
                                {
                                    changes[j].Release();
                                    changes[j] = changes[i];
                                }
                                else
                                {
                                    changes[j].Reconcile(changes[i]);
                                }

                                changes.RemoveAt(i--);
                            }
                        }

                        var e = _manager.GetEntityById(changes[j].EntityId);
                        if (e != null)
                            changes[j].ApplyTo(_manager, e);
                    }

                    changes.Clear();
                    ChangeStack.Push(changes);
                }
            }
        }

        private readonly Thread[] _threads;
        private readonly AutoResetEvent[] _workAvailable;
        private readonly AutoResetEvent _workDone;
        private int _workDoneCounter = 0;


        private readonly IntMap<int> _indexMap = new IntMap<int>(128);
        private readonly List<List<EntityChange>> _changeLists = new List<List<EntityChange>>(64);
        
        public bool HasChanges;
        private readonly object _dicLock = new object();
        public void Add(EntityChange change)
        {
            List<EntityChange> changes;
            lock (_dicLock)
            {
                int idx;
                if (!_indexMap.TryGetValue(change.EntityId, out idx))
                {
                    if (!ChangeStack.TryPop(out changes))
                        changes = new List<EntityChange>();

                    changes.Add(change);
                    _changeLists.Add(changes);
                    HasChanges = true;
                    return;
                }

                changes = _changeLists[idx];
            }

            lock (changes)
            {
                HasChanges = true;
                changes.Add(change);
            }
        }

        public void Commit()
        {
            for (int i = 0; i < ThreadCount; i++)
            {
                _workAvailable[i].Set();
            }

            CommitWork(ThreadCount);

            if (ThreadCount > 0)
            {
                _workDone.WaitOne();

                _workDoneCounter = 0;
            }

            _indexMap.Clear();
            _changeLists.Clear();
            HasChanges = false;
        }
    }
}