using Misana.Core.Components;
using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misana.Core.Communication.Messages;
using Misana.Network;

namespace Misana.Core.Systems
{
    public class InputSystem : BaseSystemR3O1<PlayerInputComponent, MotionComponent, TransformComponent, FacingComponent>
    {
        private readonly PositionTrackingSystem _positionTrackingSystem;
        private readonly INetworkSender _sender;

        public InputSystem(PositionTrackingSystem positionTrackingSystem, INetworkSender sender)
        {
            _positionTrackingSystem = positionTrackingSystem;
            _sender = sender;
        }

        protected override void Update(Entity e, PlayerInputComponent r1, MotionComponent r2, TransformComponent r3, FacingComponent o1)
        {
            r2.Move += r1.Move * GameTime.ElapsedTime.TotalSeconds;

            if (o1 != null)
                o1.Facing = (r3.Position - r1.Facing).Normalize();

            var wielding = e.Get<WieldingComponent>();
            
            if (wielding != null)
            {
                if (r1.Drop)
                {
                    if (wielding.RightHandEntityId > 0)
                    {
                        var wieldedEntity = Manager.GetEntityById(wielding.RightHandEntityId);
                       
                        if (wieldedEntity != null)
                        {
                            var msg = new DropWieldedMessage {
                                OwnerId = e.Id,
                                WieldedId = wieldedEntity.Id,
                                TwoHanded = wieldedEntity.Id
                            };
                            _sender.SendMessage(ref msg);

                            wielding.RightHandEntityId = 0;
                        }
                    }
                }
                else
                {
                    wielding.Use = r1.Attacking;
                }

                if (r1.PickUp)
                {
                    var ids = _positionTrackingSystem.SlowCoarseQuery(r3.CurrentArea.Id - 1, r3.Position, 1).ToList();
                    foreach(var id in ids)
                    {
                        if(id == e.Id)
                            continue;

                        var entity = Manager.GetEntityById(id);
                        if(entity?.Get<DroppedItemComponent>() == null)
                            continue;

                        if (wielding.RightHandEntityId == 0)
                        {
                            var wieldedTransform = entity.Get<TransformComponent>();
                            wieldedTransform.ParentEntityId = e.Id;
                            wieldedTransform.Position = Vector2.Zero;
                            entity.Remove<DroppedItemComponent>();
                            wieldedTransform.Radius *= 2;

                            var w = ComponentRegistry<WieldedComponent>.Take();
                            Manager.Add(entity, w, false);

                            wielding.RightHandEntityId = entity.Id;
                            wielding.TwoHanded = true;
                        }
                    }
                }
            }

            var interacting = e.Get<EntityInteractableComponent>();

            if (interacting != null)
                interacting.Interacting = r1.Interact;
        }
    }

    
}
