using Misana.Core.Components;
using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misana.Core.Communication.Messages;
using Misana.Core.Effects.Messages;
using Misana.Network;

namespace Misana.Core.Systems
{
    public class InputSystem : BaseSystemR3O1<PlayerInputComponent, MotionComponent, TransformComponent, FacingComponent>
    {
        private readonly PositionTrackingSystem _positionTrackingSystem;
        private readonly Simulation _simulation;

        public InputSystem(PositionTrackingSystem positionTrackingSystem, Simulation simulation)
        {
            _positionTrackingSystem = positionTrackingSystem;
            _simulation = simulation;
        }

        protected override void Update(Entity e, PlayerInputComponent r1, MotionComponent r2, TransformComponent r3, FacingComponent o1)
        {
            r2.Move += r1.Move * 3 *  GameTime.ElapsedTime.TotalSeconds;

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
                            var msg = new OnDropWieldedEffectMessage {
                                OwnerId = e.Id,
                                WieldedId = wieldedEntity.Id,
                                TwoHanded = wieldedEntity.Id
                            };
                            _simulation.EffectMessenger.SendMessage(ref msg,true);

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
                           OnPickupEffectMessage message = new OnPickupEffectMessage(e.Id,id);
                           _simulation.EffectMessenger.SendMessage(ref message,true);
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
