﻿using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Maps;

namespace Misana.Core.Entities.BaseDefinition
{
    public class BlockColliderDefinition : ComponentDefinition<BlockColliderComponent>
    {
        public override void OnApplyDefinition(EntityBuilder entity, Map map, BlockColliderComponent component, ISimulation sim)
        {

        }
    }
}