using Misana.Core.Communication.Messages;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Effects.BaseEffects;
using Misana.Core.Effects.Messages;
using Misana.Core.Network;
using Misana.Core.Systems;

namespace Misana.Core.Server
{
    public partial class ServerGameHost : IServerGameMessageApplicator
    {
        public void Apply(SpawnerTriggeredMessage message, IClientOnServer client)
        {
            if (!players.ContainsKey(client.NetworkId))
                return;

            var owner = simulation.BaseSimulation.Entities.GetEntityById(message.SpawnerOwnerId);
            var spawnerComponent = owner.Get<SpawnerComponent>();

            var tf = ComponentRegistry<TransformComponent>.Take();
            tf.CurrentArea = simulation.BaseSimulation.CurrentMap.GetAreaById(message.AreaId);
            tf.Position = message.Position;
            tf.Radius = message.Radius;


            if (simulation.BaseSimulation.SpawnerSystem.CanSpawn(spawnerComponent, owner.Get<TransformComponent>()))
            {
                ProjectileComponent pc = null;
                if (message.Projectile)
                {
                    pc = ComponentRegistry<ProjectileComponent>.Take();
                    pc.Move = message.Move;
                }

                simulation.BaseSimulation.SpawnerSystem.SpawnRemote(spawnerComponent, message.SpawnedEntityId, tf, pc);
                Enqueue(message, client.NetworkId);
            }
        }

        public void Apply(OnTeleportEffectMessage message, IClientOnServer client)
        {
            var entity = Entities.GetEntityById(message.EntityId);

            var transform = entity?.Get<TransformComponent>();

            if(transform == null)
                return;

            if (TeleportEffect.CanApply(message, entity, transform, simulation.BaseSimulation))
            {
                TeleportEffect.ApplyFromRemote(message, entity, transform, simulation.BaseSimulation);
                Enqueue(message, client.NetworkId);
            }
        }

        public void Apply(EntityHealthMessage message, IClientOnServer client)
        {
            throw new System.NotSupportedException();
        }

        public void Apply(EntityPositionMessage message, IClientOnServer client)
        {
            var e = Entities.GetEntityById(message.entityId);

            if (e == null)
                return;

            var transform = e.Get<TransformComponent>();
            var facing = e.Get<FacingComponent>();

            if (transform != null)
                transform.Position = message.position;

            if (facing != null)
                facing.Facing = message.Facing;

            Enqueue(message, client.NetworkId);
        }

        public void Apply(OnDropWieldedEffectMessage message, IClientOnServer client)
        {
            InputSystem.ApplyFromRemote(message, Entities);
            Enqueue(message, client.NetworkId);
        }

        public void Apply(OnPickupEffectMessage message, IClientOnServer client)
        {
            InputSystem.ApplyFromRemote(message, Entities);
            Enqueue(message, client.NetworkId);
        }

        public void Apply(OnDamageEffectMessage message, IClientOnServer client)
        {
            throw new System.NotSupportedException();

//            var entity = Entities.GetEntityById(message.EntityId);
//
//            if(entity == null)
//                return;
//
//            var health = entity.Get<HealthComponent>();
//
//            if(health == null)
//                return;
//
//            DamageEffect.ApplyFromRemote(message, entity, health, simulation.BaseSimulation);
//            simulation.Players.Enqueue(message, client.NetworkId);
        }

        public void Apply(OnCreateEntityMessage message, IClientOnServer client)
        {
            throw new System.NotImplementedException();
        }
    }
}