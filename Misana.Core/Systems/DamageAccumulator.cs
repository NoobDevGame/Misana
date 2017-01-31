using Misana.Core.Components;
using Misana.Core.Components.StatusComponents;
using Misana.Core.Ecs;

namespace Misana.Core.Systems
{
    public class DamageAccumulator : BaseSystemR1O1<HealthComponent,StatsComponent>
    {
        protected override void Update(Entity e, HealthComponent healthComponent,StatsComponent statsComponent)
        {
            var defense = statsComponent?.Defense ?? 0;

            var damage =  healthComponent.CurrentDamage - defense;

            if (damage > 0)
                healthComponent.Current -= damage;

            healthComponent.CurrentDamage = 0;
        }
    }
}