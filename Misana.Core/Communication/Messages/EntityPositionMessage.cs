﻿using Misana.Core.Components;
using Misana.Network;

namespace Misana.Core.Communication.Messages
{
    [MessageDefinition]
    public struct EntityPositionMessage
    {
        public int entityId;

        public Vector2 position;
        public Vector2 Facing;

        public EntityPositionMessage(int entityId,TransformComponent component)
        {
            this.entityId = entityId;
            position = component.Position;
            Facing = Vector2.Zero;
        }
    }
}