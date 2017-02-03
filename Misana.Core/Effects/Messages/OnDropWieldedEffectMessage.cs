using Misana.Core.Network;

namespace Misana.Core.Effects.Messages
{
    public class OnDropWieldedEffectMessage : TcpGameMessage
    {
        public int OwnerId;
        public int WieldedId;
        public int TwoHanded;

        public OnDropWieldedEffectMessage(){}

        public override void ApplyOnClient(IClientGameMessageApplicator a)
        {
           a.Apply(this);
        }

        public override void ApplyOnServer(IServerGameMessageApplicator a, IClientOnServer client)
        {
            a.Apply(this, client);
        }
    }
}