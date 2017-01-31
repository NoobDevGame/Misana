using Misana.Core.Ecs;
using Misana.Core.Ecs.Meta;

namespace Misana.Core.Components.StatusComponents
{
    public class HealthComponent : Component<HealthComponent>
    {
        [Copy, Reset]
        public bool IsDeath;

        [Copy, Reset]
        public float Current;

        [Copy, Reset]
        public float Max;

        [Reset]
        public float CurrentDamage;

        public float Ratio => Current / Max;
    }
}