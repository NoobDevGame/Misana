using Misana.Network;

namespace Misana.Core.Communication.Messages
{
    [MessageDefinition]
    public struct OnDropWieldedEffectMessage
    {
        public int OwnerId;
        public int WieldedId;
        public int TwoHanded;
    }
}