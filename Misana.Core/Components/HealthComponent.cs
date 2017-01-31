using Misana.Core.Ecs;
using Misana.Core.Ecs.Meta;

namespace Misana.Core.Components
{
    public class HealthComponent : Component<HealthComponent>
    {
        [Copy, Reset]
        public float Current;

        [Copy, Reset]
        public float Max;

        public float Ratio => Current / Max;
    }
}