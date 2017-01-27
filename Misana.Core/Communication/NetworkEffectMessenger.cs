using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Threading;
using Misana.Core.Communication.Messages;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Effects.Messages;
using Misana.Network;

namespace Misana.Core.Communication
{
    public class NetworkEffectMessenger
    {
        private readonly Simulation simulation;
        private readonly INetworkSender sender;
        private readonly INetworkReceiver receiver;

        private Dictionary<Type,Action<object>> localCallbacks = new Dictionary<Type ,Action<object>>();

        private int _dummyId = 0;

        public NetworkEffectMessenger(Simulation simulation,INetworkSender sender,INetworkReceiver receiver)
        {
            this.simulation = simulation;
            this.sender = sender;
            this.receiver = receiver;

            RegisterCallback<OnCreateProjectileEffectMessage>(OnCreateProjectileEffect);
            RegisterCallback<OnDropWieldedEffectMessage>(OnDropWielded);
            RegisterCallback<OnPickupEffectMessage>(OnEffectPickup);
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
                .Add<TransformComponent>(t => {
                    t.CurrentArea = simulation.CurrentMap.GetAreaById(message.area);
                    t.Radius = message.Radius;
                    t.Position = message.position;
                })
                ;

            if (message.Expiration > 0)
                builder.Add<ExpiringComponent>(x => x.TimeLeft = TimeSpan.FromMilliseconds(message.Expiration));

            if (simulation.Mode != SimulationMode.Server)
                builder.Add<SpriteInfoComponent>(x => x.TilePosition = new Index2(37,0));

            builder.Commit(simulation.Entities,id);
        }

        private void RegisterCallback<T>(MessageReceiveCallback<T> callback)
            where T : struct
        {
            receiver.RegisterOnMessageCallback(callback);
            localCallbacks.Add(typeof(T),(o) => callback((T)o,default(MessageHeader),null));

        }

        public void SendMessage<T>(ref T message,bool self = false) where T : struct
        {
            if (self && localCallbacks.ContainsKey(typeof(T)))
            {
                localCallbacks[typeof(T)].Invoke(message);
            }
            sender.SendMessage(ref message);
        }

        public bool TryGetMessage<T>(out T message) where T : struct
        {
            return receiver.TryGetMessage(out message);
        }
    }
}