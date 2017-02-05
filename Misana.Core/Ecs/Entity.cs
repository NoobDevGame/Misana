using System;

namespace Misana.Core.Ecs
{
    public class Entity
    {
        public EntityManager Manager;
        public Component[] Components;
        public int Id;

        public T Get<T>() where T : Component, new() => (T)Components[ComponentRegistry<T>.Index];

        public Entity Add<T>() where T : Component, new()
        {
            Manager.Add<T>(this);
            return this;
        }

        public Entity Add<T>(T c) where T : Component, new()
        {
            Manager.Add(this, c);
            return this;
        }

        public Entity Remove<T>() where T : Component, new()
        {
            Manager.Remove<T>(this);
            return this;
        }

        public Entity()
        {
            Components = ComponentArrayPool.Take();
        }
        
        public Entity(int id, Component[] components, EntityManager manager)
        {
            Id = id;
            Components = components;
            Manager = manager;
        }

        public bool Contains<T>() where T : Component, new()
        {
            return Get<T>() != null;
        }
    }
}