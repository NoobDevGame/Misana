using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Threading;
using Misana.Core.Communication.Components;
using Misana.Core.Communication.Messages;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Effects.Messages;
using Misana.Network;

namespace Misana.Core.Communication
{
    public abstract class NetworkEffectMessenger
    {
        protected readonly Simulation simulation;
        protected readonly INetworkSender sender;
        protected readonly INetworkReceiver receiver;

        private Dictionary<Type,Action<object>> localCallbacks = new Dictionary<Type ,Action<object>>();

        public NetworkEffectMessenger(Simulation simulation,INetworkSender sender,INetworkReceiver receiver)
        {
            this.simulation = simulation;
            this.sender = sender;
            this.receiver = receiver;


        }

        protected void RegisterCallback<T>(MessageReceiveCallback<T> callback)
            where T : struct
        {
            receiver.RegisterOnMessageCallback(callback);
            localCallbacks.Add(typeof(T),(o) => callback((T)o,default(MessageHeader),null));

        }

        public void SendMessage<T>(ref T message,bool self = false) where T : struct
        {
            if (self )
                ApplyEffectSelf(ref message);
            sender.SendMessage(ref message);
        }

        public bool TryGetMessage<T>(out T message) where T : struct
        {
            return receiver.TryGetMessage(out message);
        }

        public void ApplyEffectSelf<T>(ref T message)
            where T : struct
        {
            if (localCallbacks.ContainsKey(typeof(T)))
            {
                localCallbacks[typeof(T)].Invoke(message);
            }
        }
    }
}