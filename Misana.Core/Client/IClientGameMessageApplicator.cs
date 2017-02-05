using Misana.Core.Communication.Messages;
using Misana.Core.Effects.Messages;

namespace Misana.Core.Network {
    public interface IClientGameMessageApplicator {
        void Apply(EntityHealthMessage message);
        void Apply(EntityPositionMessage message);
        void Apply(SpawnerTriggeredMessage message);
        void Apply(OnTeleportEffectMessage message);
        void Apply(OnDropWieldedEffectMessage message);
        void Apply(OnPickupEffectMessage message);
        void Apply(OnDamageEffectMessage message);
    }
}