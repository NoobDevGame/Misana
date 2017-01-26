using System;
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
            RegisterCallback<DropWieldedMessage>(OnDropWielded);
        }

        private void OnDropWielded(DropWieldedMessage message, MessageHeader header, NetworkClient client)
        {
            var em = simulation.Entities;
            var owner = em.GetEntityById(message.OwnerId);
            var wielded = em.GetEntityById(message.WieldedId);

            if (wielded == null || owner == null)
                return;

            var ownerWielding = owner.Get<WieldingComponent>();
            var ownerTransform = owner.Get<TransformComponent>();

            if (ownerWielding == null || ownerTransform == null)
                return;

            if (ownerWielding.RightHandEntityId != message.WieldedId)
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
                builder.Add<SpriteInfoComponent>();

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