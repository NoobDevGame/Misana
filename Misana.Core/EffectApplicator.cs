using System;
using System.Threading;
using Misana.Core.Communication;
using Misana.Core.Communication.Components;
using Misana.Core.Communication.Messages;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Effects.BaseEffects;
using Misana.Core.Effects.Messages;
using Misana.Core.Events.Entities;
using Misana.Network;

namespace Misana.Core
{
    public class EffectApplicator : NetworkEffectMessenger
    {
        private int _dummyId = 0;

        public EffectApplicator(Simulation simulation, INetworkSender sender, INetworkReceiver receiver)
            : base(simulation, sender, receiver)
        {
            RegisterCallback<OnCreateProjectileEffectMessage>(OnCreateProjectileEffect);
            RegisterCallback<OnDropWieldedEffectMessage>(OnDropWielded);
            RegisterCallback<OnPickupEffectMessage>(OnEffectPickup);
            RegisterCallback<OnTeleportEffectMessage>(OnTeleport);
            RegisterCallback<OnDamageEffectMessage>(OnDamageEffect);
        }

        private void OnDamageEffect(OnDamageEffectMessage message, MessageHeader header, NetworkClient client)
        {
            var entity = simulation.Entities.GetEntityById(message.EntityId);
            var healthComponet = entity.Get<HealthComponent>();

            if (healthComponet != null)
                healthComponet.Current -= message.Damage;
        }

        private void OnTeleport(OnTeleportEffectMessage message, MessageHeader header, NetworkClient client)
        {
            var entity = simulation.Entities.GetEntityById(message.EntityId);
            var positionComponent = entity.Get<TransformComponent>();

            if (positionComponent != null)
            {
                var area = simulation.CurrentMap.GetAreaById( message.AreaId);
                positionComponent.CurrentArea = area;
                positionComponent.Position = message.Position;
            }
        }

        private void OnEffectPickup(OnPickupEffectMessage effectMessage, MessageHeader header, NetworkClient client)
        {
            var parentEntity = simulation.Entities.GetEntityById(effectMessage.ParentEntityId);
            var entity = simulation.Entities.GetEntityById(effectMessage.EntityId);

            var wieldedTransform = entity.Get<TransformComponent>();
            wieldedTransform.ParentEntityId = parentEntity.Id;
            wieldedTransform.Position = Vector2.Zero;
            entity.Remove<DroppedItemComponent>();
            wieldedTransform.Radius *= 2;

            var w = ComponentRegistry<WieldedComponent>.Take();
            simulation.Entities.Add(entity, w, false);

            var wielding = parentEntity.Get<WieldingComponent>();

            wielding.RightHandEntityId = entity.Id;
            wielding.TwoHanded = true;
        }

        private void OnDropWielded(OnDropWieldedEffectMessage effectMessage, MessageHeader header, NetworkClient client)
        {
            var em = simulation.Entities;
            var owner = em.GetEntityById(effectMessage.OwnerId);
            var wielded = em.GetEntityById(effectMessage.WieldedId);

            if (wielded == null || owner == null)
                return;

            var ownerWielding = owner.Get<WieldingComponent>();
            var ownerTransform = owner.Get<TransformComponent>();

            if (ownerWielding == null || ownerTransform == null)
                return;

            if (ownerWielding.RightHandEntityId != effectMessage.WieldedId)
                return;

            var wieldedTransform = wielded.Get<TransformComponent>();

            if (wieldedTransform == null)
                return;

            wielded.Remove<WieldedComponent>().Add<DroppedItemComponent>();

            ownerWielding.TwoHanded = false;
            ownerWielding.RightHandEntityId = 0;

            wieldedTransform.Radius /= 2;
            wieldedTransform.ParentEntityId = 0;
            wieldedTransform.Position = ownerTransform.Position;
        }

        private void OnCreateProjectileEffect(OnCreateProjectileEffectMessage message, MessageHeader header, NetworkClient client)
        {
            var id = Interlocked.Increment(ref _dummyId) % ushort.MaxValue + ushort.MaxValue;
            EntityBuilder builder = new EntityBuilder();
            builder.Add<MotionComponent>(x => x.Move = message.move )
                .Add<ProjectileComponent>(x => x.Move = message.move)
                .Add<OnLocalSimulationComponent>()
                .Add<EntityColliderComponent>(a =>
                {
                    a.OnCollisionEvents.Add(new ApplyEffectEvent(new DamageEffect(message.Damage)) {ApplyTo = ApplicableTo.Other});
                    a.OnCollisionEvents.Add(new ApplyEffectEvent(new RemoveSelfEffect()) {ApplyTo = ApplicableTo.Self});
                })
                .Add<TransformComponent>(t => {
                    t.CurrentArea = simulation.CurrentMap.GetAreaById(message.area);
                    t.Radius = message.Radius;
                    t.Position = message.position;
                    t.Radius = 0.2f;
                })
                ;

            if (message.Expiration > 0)
                builder.Add<ExpiringComponent>(x => x.TimeLeft = TimeSpan.FromMilliseconds(message.Expiration));

            if (simulation.Mode != SimulationMode.Server)
                builder.Add<SpriteInfoComponent>(x => x.TilePosition = new Index2(37,0));

            builder.Commit(simulation.Entities,id);
        }
    }
}