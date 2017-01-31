using Misana.Core.Ecs;
using Misana.Core.Ecs.Meta;

namespace Misana.Core.Components.StatusComponents
{
    public class StatsComponent : Component<StatsComponent>
    {
        [Copy,Reset(1)]
        public int Attack;

        [Copy,Reset(0)]
        public int Defense;
    }
}