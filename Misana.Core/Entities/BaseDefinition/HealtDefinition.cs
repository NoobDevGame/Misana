using Misana.Core.Components;
using Misana.Core.Ecs;

namespace Misana.Core.Entities.BaseDefinition
{
    public class HealtDefinition : ComponentDefinition<HealthComponent>
    {
        public float Current { get; set; }
        public float Max { get; set; }

        public HealtDefinition()
        {
            Current = Max = 100;
        }

        public HealtDefinition(float max)
        {
            Current = Max = max;
        }

        public HealtDefinition(float current, float max)
        {
            Current = current;
            Max = max;
        }

        public override void OnApplyDefinition(Entity entity, HealthComponent component)
        {
            component.Current = Current;
            component.Max = Max;
        }
    }
}