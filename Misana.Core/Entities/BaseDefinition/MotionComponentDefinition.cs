﻿using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Maps;

namespace Misana.Core.Entities.BaseDefinition
{
    public class MotionComponentDefinition : ComponentDefinition<MotionComponent>
    {

        public override void OnApplyDefinition(EntityBuilder entity, Map map, MotionComponent component, ISimulation sim)
        {
        }
    }
}