using System;
using System.Collections.Generic;
using Misana.Core.Effects.Messages;
using Misana.Core.Entities.BaseDefinition;
using Misana.Core.Systems;

namespace Misana.Core.Ecs
{
    public class EntityBuilder
    {
        public Component[] Components;
        public List<AttachedEntity> AttachedEntities = new List<AttachedEntity>();

        public EntityBuilder()
        {
            Components = ComponentArrayPool.Take();
        }

        public EntityBuilder(Component[] components)
        {
            Components = components;
        }

        public EntityBuilder Add<T>() where T : Component, new() => Add<T>((Action<T>) null);

        public EntityBuilder Add<T>(Action<T> a) where T : Component, new()
        {
            var c = ComponentRegistry<T>.Take();
            a?.Invoke(c);

            return Add(c);
        }

        public EntityBuilder Add<T>(T component) where T : Component, new()
        {
            if (Components == null)
                throw new InvalidOperationException();

            Components[ComponentRegistry<T>.Index] = component;
            return this;
        }

        public EntityBuilder Remove<T>() where T : Component, new()
        {
            if (Components == null)
                throw new InvalidOperationException();

            Components[ComponentRegistry<T>.Index] = null;
            return this;
        }

        public Entity Commit(EntityManager manager)
        {
            if (Components == null)
                throw new InvalidOperationException();

            return Commit(manager, manager.NextId());
        }

        public Entity Commit(EntityManager manager, params int[] entityIds)
        {
            if (Components == null)
                throw new InvalidOperationException();

            var e = new Entity(entityIds[0], Components, manager);
            manager.AddEntity(e);

            Components = null;

            for (var i = 0; i < AttachedEntities.Count; i++)
            {
                var ae = AttachedEntities[i];
                var r = ae.Builder.Commit(manager, i + 1 < entityIds.Length ? entityIds[1] : manager.NextId());

                switch (ae.AttachmentType)
                {
                    case AttachmentType.None:
                        break;
                    case AttachmentType.Wielded:
                        InputSystem.ApplyFromRemote(e, r, manager);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return e;
        }

        public EntityBuilder CommitAndReturnCopy(EntityManager manager)
        {
            int id;
            return CommitAndReturnCopy(manager, out id);
        }

        public EntityBuilder CommitAndReturnCopy(EntityManager manager, out int id)
        {
            var copy = Copy();
            var entity = Commit(manager);
            id = entity.Id;
            return copy;
        }

        public EntityBuilder Copy()
        {
            var eb = new EntityBuilder();

            for (int i = 0; i < Components.Length; i++)
            {
                if (Components[i] != null)
                {
                    eb.Components[i] = ComponentRegistry.Take[i]();
                    ComponentRegistry.Copy[i](Components[i], eb.Components[i]);
                }
            }

            return eb;
        }


        public T Get<T>()
            where T :Component, new ()
        {
            return (T) Components[ComponentRegistry<T>.Index];
        }
    }

    public enum AttachmentType : byte
    {
        None = 0,
        Wielded = 1
    }

    public class AttachedEntity
    {
        public EntityBuilder Builder;
        public AttachmentType AttachmentType;

        public AttachedEntity(EntityBuilder builder, AttachmentType at)
        {
            Builder = builder;
            AttachmentType = at;
        }

        public AttachedEntity(){}
    }
}