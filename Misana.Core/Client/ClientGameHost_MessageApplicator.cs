using System;
using Misana.Core.Communication.Messages;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Effects.BaseEffects;
using Misana.Core.Effects.Messages;
using Misana.Core.Network;
using Misana.Core.Systems;

namespace Misana.Core.Client
{
    public partial class ClientGameHost : IClientGameMessageApplicator
    {
        public void Apply(EntityHealthMessage message)
        {
            var entity = Simulation.Entities.GetEntityById(message.EntityId);
            if (entity != null)
            {
                var cmp = entity.Get<HealthComponent>();
                cmp.Current = message.Health;
            }
        }

        public void Apply(EntityPositionMessage message)
        {
            var entity = Simulation.Entities.GetEntityById(message.entityId);
            if (entity != null)
            {
                var transformCmp = entity.Get<TransformComponent>();
                transformCmp.Position = message.position;

                var facingCmp = entity.Get<FacingComponent>();
                if (facingCmp != null)
                    facingCmp.Facing = message.Facing;
            }
        }

        public void Apply(SpawnerTriggeredMessage message)
        {
            var simulation = Simulation;
            //simulation.Players.ReceiveMessage(ref message, header, client);

            if(Entities.GetEntityById(message.SpawnedEntityId) != null)
                return;

            var owner = simulation.Entities.GetEntityById(message.SpawnerOwnerId);
            if(owner == null)
                return;

            var spawnerComponent = owner.Get<SpawnerComponent>();

            var tf = ComponentRegistry<TransformComponent>.Take();
            tf.CurrentAreaId = message.AreaId;
            tf.Position = message.Position;
            tf.Radius = message.Radius;

            ProjectileComponent pc = null;
            if (message.Projectile)
            {
                pc = ComponentRegistry<ProjectileComponent>.Take();
                pc.Move = message.Move;
            }

            SpawnerSystem.SpawnRemote(spawnerComponent, message.SpawnedEntityId, tf, pc);
        }

        public void Apply(OnTeleportEffectMessage message)
        {
            var entity = Entities.GetEntityById(message.EntityId);

            if(entity == null)
                return;

            var transform = entity.Get<TransformComponent>();

            if(transform == null)
                return;

            TeleportEffect.ApplyFromRemote(message, entity, transform, Simulation);
        }

        public void Apply(OnDropWieldedEffectMessage message)
        {
            InputSystem.ApplyFromRemote(message, Entities);
        }

        public void Apply(OnPickupEffectMessage message)
        {
            var parent = Entities.GetEntityById(message.ParentEntityId);
            var wielded = Entities.GetEntityById(message.EntityId);

            if (parent != null && wielded != null)
            {
                InputSystem.ApplyFromRemote(parent, wielded, Entities);
            }
        }

        public void Apply(OnDamageEffectMessage message)
        {
            var entity = Entities.GetEntityById(message.EntityId);

            if(entity == null)
                return;

            var health = entity.Get<HealthComponent>();

            if(health == null)
                return;

            DamageEffect.ApplyFromRemote(message, entity, health, Simulation);
        }

    }
}