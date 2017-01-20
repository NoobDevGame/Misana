using Misana.Core.Ecs;

namespace Misana.Core.Components
{
    public class WieldingComponent : Component<WieldingComponent>
    {
        public bool TwoHanded;
        public int LeftHandEntityId;
        public int RightHandEntityId;
        public bool Use;

        public override void CopyTo(WieldingComponent other)
        {
            other.TwoHanded = TwoHanded;
            other.LeftHandEntityId = LeftHandEntityId;
            other.RightHandEntityId = RightHandEntityId;
        }
    }
}