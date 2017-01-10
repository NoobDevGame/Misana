using System.Collections.Generic;

namespace Misana.Core.Ecs
{
    public abstract class BaseSystem
    {
        protected const int InitialSize = 16;

        protected readonly EntityManager Manager;
        protected BaseSystem(EntityManager manager)
        {
            Manager = manager;
        }

        protected int Capacity = InitialSize;
        protected int Count = 0;
        protected Entity[] Entities = new Entity[InitialSize];

        protected readonly Dictionary<Entity, int> IndexMap = new Dictionary<Entity, int>(InitialSize);

        protected List<int> RequiredIndexes;
        protected List<int> OptionalIndexes;
        protected List<int> NegativeIndexes;

        public void EntityAdded(Entity e)
        {
            if (!Matches(e))
                return;

            if (Count == Capacity)
            {
                var newCapacity = Capacity * 2;
                Grow(ref Entities, newCapacity);
                Grow(newCapacity);
                Capacity = newCapacity;
            }

            var idx = Count++;
            Entities[idx] = e;
            IndexMap[e] = idx;
            Add(e, idx);
        }

        public void EntityRemoved(Entity e)
        {
            int idx;
            if (!IndexMap.TryGetValue(e, out idx))
                return;

            if (idx == Count - 1)
            {
                Entities[idx] = null;
                Remove(idx, null);
            }
            else
            {
                var swapIndex = Count - 1;
                Entities[idx] = Entities[swapIndex];
                Entities[swapIndex] = null;
                IndexMap.Remove(e);
                IndexMap[Entities[idx]] = idx;
                Remove(idx, swapIndex);
            }

            Count--;
        }

        public void EntityChanged(Entity e)
        {
            int idx;
            var existing = IndexMap.TryGetValue(e, out idx);

            if (Matches(e))
            {
                if (existing)
                {
                    Update(e, idx);
                    return;
                }

                if (Count == Capacity)
                {
                    var newCapacity = Capacity * 2;
                    Grow(ref Entities, newCapacity);
                    Grow(newCapacity);
                }

                idx = Count++;
                Entities[idx] = e;
                IndexMap[e] = idx;
                Add(e, idx);
            }
            else
            {
                if (!existing)
                    return;

                if (idx == Count - 1)
                {
                    Entities[idx] = null;
                    Remove(idx, null);
                }
                else
                {
                    var swapIndex = Count - 1;
                    Entities[idx] = Entities[swapIndex];
                    Entities[swapIndex] = null;
                    IndexMap.Remove(e);
                    IndexMap[Entities[idx]] = idx;
                    Remove(idx, swapIndex);
                }

                Count--;
            }
        }

        protected bool Matches(Entity e)
        {
            for (int i = 0; i < RequiredIndexes.Count; i++)
            {
                if (e.Components[RequiredIndexes[i]] == null)
                    return false;
            }

            if (NegativeIndexes != null)
            {
                for (int i = 0; i < NegativeIndexes.Count; i++)
                {
                    if (e.Components[NegativeIndexes[i]] != null)
                        return false;
                }
            }

            return true;
        }

        protected virtual void Update(Entity entity, int idx) { }

        public abstract void Tick();

        protected abstract void Remove(int index, int? swapWith);
        protected abstract void Add(Entity e, int index);

        protected abstract void Grow(int to);

        protected void Grow<T>(ref T[] arr, int to)
        {
            var nt = new T[to];
            arr.CopyTo(nt, 0);
            arr = nt;
        }
    }

    public abstract class BaseSystemR1<TR1> : BaseSystem
        where TR1 : Component, new()
    {
        protected TR1[] R1S = new TR1[InitialSize];

        protected override void Grow(int to)
        {
            Grow(ref R1S, to);
        }

        protected override void Add(Entity e, int index)
        {
            R1S[index] = e.Get<TR1>();
        }

        protected override void Remove(int index, int? swapWith)
        {
            if (swapWith.HasValue)
            {
                var swap = swapWith.Value;

                R1S[index] = R1S[swap];
                R1S[swap] = null;
            }
            else
            {
                R1S[index] = null;
            }
        }

        public override void Tick()
        {
            for (int i = 0; i < Count; i++)
            {
                Update(Entities[i], R1S[i]);
            }
        }

        protected abstract void Update(Entity e, TR1 r1);

        protected BaseSystemR1(EntityManager manager) : base(manager)
        {
            ComponentRegistry<TR1>.InterestedSystems[manager.Index].Add(this);
            RequiredIndexes = new List<int> { ComponentRegistry<TR1>.Index };
        }
    }

    public abstract class BaseSystemR2<TR1, TR2> : BaseSystem
        where TR1 : Component, new()
        where TR2 : Component, new()
    {
        protected TR1[] R1S = new TR1[InitialSize];
        protected TR2[] R2S = new TR2[InitialSize];

        protected override void Grow(int to)
        {
            Grow(ref R1S, to);
            Grow(ref R2S, to);
        }

        protected override void Add(Entity e, int index)
        {
            R1S[index] = e.Get<TR1>();
            R2S[index] = e.Get<TR2>();
        }

        protected override void Remove(int index, int? swapWith)
        {
            if (swapWith.HasValue)
            {
                var swap = swapWith.Value;

                R1S[index] = R1S[swap];
                R1S[swap] = null;

                R2S[index] = R2S[swap];
                R2S[swap] = null;
            }
            else
            {
                R1S[index] = null;
                R2S[index] = null;
            }
        }

        public override void Tick()
        {
            for (int i = 0; i < Count; i++)
            {
                Update(Entities[i], R1S[i], R2S[i]);
            }
        }

        protected abstract void Update(Entity e, TR1 r1, TR2 r2);

        protected BaseSystemR2(EntityManager manager) : base(manager)
        {
            ComponentRegistry<TR1>.InterestedSystems[manager.Index].Add(this);
            ComponentRegistry<TR2>.InterestedSystems[manager.Index].Add(this);
            RequiredIndexes = new List<int> { ComponentRegistry<TR1>.Index, ComponentRegistry<TR2>.Index };
        }

    }

    public abstract class BaseSystemR3<TR1, TR2, TR3> : BaseSystem
        where TR1 : Component, new()
        where TR2 : Component, new()
        where TR3 : Component, new()
    {
        protected TR1[] R1S = new TR1[InitialSize];
        protected TR2[] R2S = new TR2[InitialSize];
        protected TR3[] R3S = new TR3[InitialSize];

        protected override void Grow(int to)
        {
            Grow(ref R1S, to);
            Grow(ref R2S, to);
            Grow(ref R3S, to);
        }

        protected override void Add(Entity e, int index)
        {
            R1S[index] = e.Get<TR1>();
            R2S[index] = e.Get<TR2>();
            R3S[index] = e.Get<TR3>();
        }

        protected override void Remove(int index, int? swapWith)
        {
            if (swapWith.HasValue)
            {
                var swap = swapWith.Value;

                R1S[index] = R1S[swap];
                R1S[swap] = null;

                R2S[index] = R2S[swap];
                R2S[swap] = null;

                R3S[index] = R3S[swap];
                R3S[swap] = null;
            }
            else
            {
                R1S[index] = null;
                R2S[index] = null;
                R3S[index] = null;
            }
        }

        public override void Tick()
        {
            for (int i = 0; i < Count; i++)
            {
                Update(Entities[i], R1S[i], R2S[i], R3S[i]);
            }
        }

        protected abstract void Update(Entity e, TR1 r1, TR2 r2, TR3 r3);

        protected BaseSystemR3(EntityManager manager) : base(manager)
        {
            ComponentRegistry<TR1>.InterestedSystems[manager.Index].Add(this);
            ComponentRegistry<TR2>.InterestedSystems[manager.Index].Add(this);
            ComponentRegistry<TR3>.InterestedSystems[manager.Index].Add(this);
            RequiredIndexes = new List<int> { ComponentRegistry<TR1>.Index, ComponentRegistry<TR2>.Index, ComponentRegistry<TR3>.Index };
        }

    }

    public abstract class BaseSystemR2N1<TR1, TR2, TN1> : BaseSystem
        where TR1 : Component, new()
        where TR2 : Component, new()
        where TN1 : Component, new()
    {
        protected TR1[] R1S = new TR1[InitialSize];
        protected TR2[] R2S = new TR2[InitialSize];

        protected override void Grow(int to)
        {
            Grow(ref R1S, to);
            Grow(ref R2S, to);
        }

        protected override void Add(Entity e, int index)
        {
            R1S[index] = e.Get<TR1>();
            R2S[index] = e.Get<TR2>();
        }

        protected override void Remove(int index, int? swapWith)
        {
            if (swapWith.HasValue)
            {
                var swap = swapWith.Value;

                R1S[index] = R1S[swap];
                R1S[swap] = null;

                R2S[index] = R2S[swap];
                R2S[swap] = null;
            }
            else
            {
                R1S[index] = null;
                R2S[index] = null;
            }
        }

        public override void Tick()
        {
            for (int i = 0; i < Count; i++)
            {
                Update(Entities[i], R1S[i], R2S[i]);
            }
        }

        protected abstract void Update(Entity e, TR1 r1, TR2 r2);

        protected BaseSystemR2N1(EntityManager manager) : base(manager)
        {
            ComponentRegistry<TR1>.InterestedSystems[manager.Index].Add(this);
            ComponentRegistry<TR2>.InterestedSystems[manager.Index].Add(this);
            ComponentRegistry<TN1>.InterestedSystems[manager.Index].Add(this);
            RequiredIndexes = new List<int> { ComponentRegistry<TR1>.Index, ComponentRegistry<TR2>.Index };
            NegativeIndexes = new List<int> { ComponentRegistry<TN1>.Index };
        }
    }

    public abstract class BaseSystemR1N1<TR1, TN1> : BaseSystemR1<TR1>
        where TR1 : Component, new()
        where TN1 : Component, new()
    {

        protected BaseSystemR1N1(EntityManager manager) : base(manager)
        {
            ComponentRegistry<TN1>.InterestedSystems[manager.Index].Add(this);
            NegativeIndexes = new List<int> { ComponentRegistry<TN1>.Index };
        }

    }

    public abstract class BaseSystemR1N2<TR1, TN1, TN2> : BaseSystemR1<TR1>
       where TR1 : Component, new()
       where TN1 : Component, new()
       where TN2 : Component, new()
    {
        protected BaseSystemR1N2(EntityManager manager) : base(manager)
        {
            ComponentRegistry<TN1>.InterestedSystems[manager.Index].Add(this);
            ComponentRegistry<TN2>.InterestedSystems[manager.Index].Add(this);
            NegativeIndexes = new List<int> { ComponentRegistry<TN1>.Index, ComponentRegistry<TN2>.Index };
        }
    }

    public abstract class BaseSystemR1O1<TR1, TO1> : BaseSystem
       where TR1 : Component, new()
       where TO1 : Component, new()
    {

        protected TR1[] R1S = new TR1[InitialSize];
        protected TO1[] O1S = new TO1[InitialSize];

        protected override void Grow(int to)
        {
            Grow(ref R1S, to);
            Grow(ref O1S, to);
        }

        protected override void Add(Entity e, int index)
        {
            R1S[index] = e.Get<TR1>();
            O1S[index] = e.Get<TO1>();
        }

        protected override void Remove(int index, int? swapWith)
        {
            if (swapWith.HasValue)
            {
                var swap = swapWith.Value;

                R1S[index] = R1S[swap];
                R1S[swap] = null;

                O1S[index] = O1S[swap];
                O1S[swap] = null;
            }
            else
            {
                R1S[index] = null;
                O1S[index] = null;
            }
        }

        public override void Tick()
        {
            for (int i = 0; i < Count; i++)
            {
                Update(Entities[i], R1S[i], O1S[i]);
            }
        }

        protected abstract void Update(Entity e, TR1 r1, TO1 o1);


        protected BaseSystemR1O1(EntityManager manager) : base(manager)
        {
            ComponentRegistry<TR1>.InterestedSystems[manager.Index].Add(this);
            ComponentRegistry<TO1>.InterestedSystems[manager.Index].Add(this);
            RequiredIndexes = new List<int> { ComponentRegistry<TR1>.Index };
            OptionalIndexes = new List<int> { ComponentRegistry<TO1>.Index };
        }

        protected override void Update(Entity entity, int idx)
        {
            O1S[idx] = entity.Get<TO1>();
        }
    }

    public abstract class BaseSystemR1O1N1<TR1, TO1, TN1> : BaseSystemR1O1<TR1, TO1>
       where TR1 : Component, new()
       where TO1 : Component, new()
       where TN1 : Component, new()
    {
        protected BaseSystemR1O1N1(EntityManager manager) : base(manager)
        {
            ComponentRegistry<TN1>.InterestedSystems[manager.Index].Add(this);
            NegativeIndexes = new List<int> { ComponentRegistry<TN1>.Index };
        }

    }

    public abstract class BaseSystemR2O1N1<TR1, TR2, TO1, TN1> : BaseSystemR2O1<TR1, TR2, TO1>
       where TR1 : Component, new()
       where TR2 : Component, new()
       where TO1 : Component, new()
       where TN1 : Component, new()
    {
        protected BaseSystemR2O1N1(EntityManager manager) : base(manager)
        {
            ComponentRegistry<TN1>.InterestedSystems[manager.Index].Add(this);
            NegativeIndexes = new List<int> { ComponentRegistry<TN1>.Index };
        }

    }

    public abstract class BaseSystemR1O2<TR1, TO1, TO2> : BaseSystem
       where TR1 : Component, new()
       where TO1 : Component, new()
       where TO2 : Component, new()
    {

        protected TR1[] R1S = new TR1[InitialSize];
        protected TO1[] O1S = new TO1[InitialSize];
        protected TO2[] O2S = new TO2[InitialSize];

        protected override void Grow(int to)
        {
            Grow(ref R1S, to);
            Grow(ref O1S, to);
            Grow(ref O2S, to);
        }

        protected override void Add(Entity e, int index)
        {
            R1S[index] = e.Get<TR1>();
            O1S[index] = e.Get<TO1>();
            O2S[index] = e.Get<TO2>();
        }

        protected override void Remove(int index, int? swapWith)
        {
            if (swapWith.HasValue)
            {
                var swap = swapWith.Value;

                R1S[index] = R1S[swap];
                R1S[swap] = null;

                O1S[index] = O1S[swap];
                O1S[swap] = null;


                O2S[index] = O2S[swap];
                O2S[swap] = null;
            }
            else
            {
                R1S[index] = null;
                O1S[index] = null;
                O2S[index] = null;
            }
        }

        public override void Tick()
        {
            for (int i = 0; i < Count; i++)
            {
                Update(Entities[i], R1S[i], O1S[i], O2S[i]);
            }
        }

        protected abstract void Update(Entity e, TR1 r1, TO1 o1, TO2 o2);


        protected BaseSystemR1O2(EntityManager manager) : base(manager)
        {
            ComponentRegistry<TR1>.InterestedSystems[manager.Index].Add(this);
            ComponentRegistry<TO1>.InterestedSystems[manager.Index].Add(this);
            ComponentRegistry<TO2>.InterestedSystems[manager.Index].Add(this);
            
            RequiredIndexes = new List<int> { ComponentRegistry<TR1>.Index };
            OptionalIndexes = new List<int> { ComponentRegistry<TO1>.Index, ComponentRegistry<TO2>.Index };
        }

        protected override void Update(Entity entity, int idx)
        {
            O1S[idx] = entity.Get<TO1>();
            O2S[idx] = entity.Get<TO2>();
        }
    }

    public abstract class BaseSystemR2O1<TR1, TR2, TO1> : BaseSystem
       where TR1 : Component, new()
       where TR2 : Component, new()
       where TO1 : Component, new()
    {

        protected TR1[] R1S = new TR1[InitialSize];
        protected TR2[] R2S = new TR2[InitialSize];
        protected TO1[] O1S = new TO1[InitialSize];

        protected override void Grow(int to)
        {
            Grow(ref R1S, to);
            Grow(ref R2S, to);
            Grow(ref O1S, to);
        }

        protected override void Add(Entity e, int index)
        {
            R1S[index] = e.Get<TR1>();
            R2S[index] = e.Get<TR2>();
            O1S[index] = e.Get<TO1>();
        }

        protected override void Remove(int index, int? swapWith)
        {
            if (swapWith.HasValue)
            {
                var swap = swapWith.Value;

                R1S[index] = R1S[swap];
                R1S[swap] = null;

                R2S[index] = R2S[swap];
                R2S[swap] = null;

                O1S[index] = O1S[swap];
                O1S[swap] = null;
            }
            else
            {
                R1S[index] = null;
                R2S[index] = null;
                O1S[index] = null;
            }
        }

        public override void Tick()
        {
            for (int i = 0; i < Count; i++)
            {
                Update(Entities[i], R1S[i], R2S[i], O1S[i]);
            }
        }

        protected abstract void Update(Entity e, TR1 r1, TR2 r2, TO1 o1);


        protected BaseSystemR2O1(EntityManager manager) : base(manager)
        {
            ComponentRegistry<TR1>.InterestedSystems[manager.Index].Add(this);
            ComponentRegistry<TR2>.InterestedSystems[manager.Index].Add(this);
            ComponentRegistry<TO1>.InterestedSystems[manager.Index].Add(this);
            RequiredIndexes = new List<int> { ComponentRegistry<TR1>.Index, ComponentRegistry<TR2>.Index, };
            OptionalIndexes = new List<int> { ComponentRegistry<TO1>.Index };
        }

        protected override void Update(Entity entity, int idx)
        {
            O1S[idx] = entity.Get<TO1>();
        }
    }

    public abstract class BaseSystemR3O1<TR1, TR2, TR3, TO1> : BaseSystem
       where TR1 : Component, new()
       where TR2 : Component, new()
       where TR3 : Component, new()
       where TO1 : Component, new()
    {

        protected TR1[] R1S = new TR1[InitialSize];
        protected TR2[] R2S = new TR2[InitialSize];
        protected TR3[] R3S = new TR3[InitialSize];
        protected TO1[] O1S = new TO1[InitialSize];

        protected override void Grow(int to)
        {
            Grow(ref R1S, to);
            Grow(ref R2S, to);
            Grow(ref R3S, to);
            Grow(ref O1S, to);
        }

        protected override void Add(Entity e, int index)
        {
            R1S[index] = e.Get<TR1>();
            R2S[index] = e.Get<TR2>();
            R3S[index] = e.Get<TR3>();
            O1S[index] = e.Get<TO1>();
        }

        protected override void Remove(int index, int? swapWith)
        {
            if (swapWith.HasValue)
            {
                var swap = swapWith.Value;

                R1S[index] = R1S[swap];
                R1S[swap] = null;

                R2S[index] = R2S[swap];
                R2S[swap] = null;

                R3S[index] = R3S[swap];
                R3S[swap] = null;

                O1S[index] = O1S[swap];
                O1S[swap] = null;
            }
            else
            {
                R1S[index] = null;
                R2S[index] = null;
                R3S[index] = null;
                O1S[index] = null;
            }
        }

        public override void Tick()
        {
            for (int i = 0; i < Count; i++)
            {
                Update(Entities[i], R1S[i], R2S[i], R3S[i], O1S[i]);
            }
        }

        protected abstract void Update(Entity e, TR1 r1, TR2 r2, TR3 r3, TO1 o1);


        protected BaseSystemR3O1(EntityManager manager) : base(manager)
        {
            ComponentRegistry<TR1>.InterestedSystems[manager.Index].Add(this);
            ComponentRegistry<TR2>.InterestedSystems[manager.Index].Add(this);
            ComponentRegistry<TR3>.InterestedSystems[manager.Index].Add(this);
            ComponentRegistry<TO1>.InterestedSystems[manager.Index].Add(this);

            RequiredIndexes = new List<int> { ComponentRegistry<TR1>.Index, ComponentRegistry<TR2>.Index, };
            OptionalIndexes = new List<int> { ComponentRegistry<TO1>.Index };
        }

        protected override void Update(Entity entity, int idx)
        {
            O1S[idx] = entity.Get<TO1>();
        }
    }


    public abstract class BaseSystemR2O2<TR1, TR2, TO1, TO2> : BaseSystem
       where TR1 : Component, new()
       where TR2 : Component, new()
       where TO1 : Component, new()
       where TO2 : Component, new()
    {

        protected TR1[] R1S = new TR1[InitialSize];
        protected TR2[] R2S = new TR2[InitialSize];
        protected TO1[] O1S = new TO1[InitialSize];
        protected TO2[] O2S = new TO2[InitialSize];

        protected override void Grow(int to)
        {
            Grow(ref R1S, to);
            Grow(ref R2S, to);
            Grow(ref O1S, to);
            Grow(ref O2S, to);
        }

        protected override void Add(Entity e, int index)
        {
            R1S[index] = e.Get<TR1>();
            R2S[index] = e.Get<TR2>();
            O1S[index] = e.Get<TO1>();
            O2S[index] = e.Get<TO2>();
        }

        protected override void Remove(int index, int? swapWith)
        {
            if (swapWith.HasValue)
            {
                var swap = swapWith.Value;

                R1S[index] = R1S[swap];
                R1S[swap] = null;

                R2S[index] = R2S[swap];
                R2S[swap] = null;

                O1S[index] = O1S[swap];
                O1S[swap] = null;

                O2S[index] = O2S[swap];
                O2S[swap] = null;
            }
            else
            {
                R1S[index] = null;
                R2S[index] = null;
                O1S[index] = null;
                O2S[index] = null;
            }
        }

        public override void Tick()
        {
            for (int i = 0; i < Count; i++)
            {
                Update(Entities[i], R1S[i], R2S[i], O1S[i], O2S[i]);
            }
        }

        protected abstract void Update(Entity e, TR1 r1, TR2 r2, TO1 o1, TO2 o2);


        protected BaseSystemR2O2(EntityManager manager) : base(manager)
        {
            ComponentRegistry<TR1>.InterestedSystems[manager.Index].Add(this);
            ComponentRegistry<TR2>.InterestedSystems[manager.Index].Add(this);
            ComponentRegistry<TO1>.InterestedSystems[manager.Index].Add(this);
            ComponentRegistry<TO2>.InterestedSystems[manager.Index].Add(this);

            RequiredIndexes = new List<int> { ComponentRegistry<TR1>.Index, ComponentRegistry<TR2>.Index, };
            OptionalIndexes = new List<int> { ComponentRegistry<TO1>.Index, ComponentRegistry<TO2>.Index };
        }

        protected override void Update(Entity entity, int idx)
        {
            O1S[idx] = entity.Get<TO1>();
            O2S[idx] = entity.Get<TO2>();
        }
    }

    public abstract class BaseSystemR3N1<TR1, TR2, TR3, TN1> : BaseSystemR1O2<TR1, TR2, TR3>
       where TR1 : Component, new()
       where TR2 : Component, new()
       where TR3 : Component, new()
       where TN1 : Component, new()
    {
        protected BaseSystemR3N1(EntityManager manager) : base(manager)
        {
            ComponentRegistry<TN1>.InterestedSystems[manager.Index].Add(this);
            NegativeIndexes = new List<int> { ComponentRegistry<TN1>.Index };
        }
    }


    public abstract class BaseSystemR1O2N1<TR1, TO1, TO2, TN1> : BaseSystemR1O2<TR1, TO1, TO2>
        where TR1 : Component, new()
        where TO1 : Component, new()
        where TO2 : Component, new()
        where TN1 : Component, new()
    {
        protected BaseSystemR1O2N1(EntityManager manager) : base(manager)
        {
            ComponentRegistry<TN1>.InterestedSystems[manager.Index].Add(this);
            NegativeIndexes = new List<int> { ComponentRegistry<TN1>.Index };
        }
    }

    public abstract class BaseSystemR1O2N2<TR1, TO1, TO2, TN1, TN2> : BaseSystemR1O2<TR1, TO1, TO2>
       where TR1 : Component, new()
       where TO1 : Component, new()
       where TO2 : Component, new()
       where TN1 : Component, new()
       where TN2 : Component, new()
    {
        protected BaseSystemR1O2N2(EntityManager manager) : base(manager)
        {
            ComponentRegistry<TN1>.InterestedSystems[manager.Index].Add(this);
            ComponentRegistry<TN2>.InterestedSystems[manager.Index].Add(this);
            NegativeIndexes = new List<int> { ComponentRegistry<TN1>.Index, ComponentRegistry<TN2>.Index };
        }
    }
}