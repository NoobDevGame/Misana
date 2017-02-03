using Misana.Core.Communication.Messages;
using Misana.Core.Effects.Messages;

namespace Misana.Core.Network {
    public interface IServerGameMessageApplicator
    {
        void Apply(SpawnerTriggeredMessage message, IClientOnServer client);
        void Apply(OnTeleportEffectMessage message, IClientOnServer client);
        void Apply(EntityHealthMessage message, IClientOnServer client);
        void Apply(EntityPositionMessage message, IClientOnServer client);
        void Apply(OnDropWieldedEffectMessage message, IClientOnServer client);
        void Apply(OnPickupEffectMessage message, IClientOnServer client);
        void Apply(OnDamageEffectMessage message, IClientOnServer client);
        void Apply(OnCreateEntityMessage message, IClientOnServer client);
    }
}