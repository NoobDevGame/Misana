using Misana.Core.Ecs;

namespace Misana.Core.Components
{
    public class HealthComponent : Component<HealthComponent>
    {
        public float Current;
        public float Max;
        public float Ratio => Current / Max;

        public override void CopyTo(HealthComponent other)
        {
            other.Current = Current;
            other.Max = Max;
        }
    }
}