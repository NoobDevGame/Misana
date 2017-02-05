using Misana.Core.Components;
using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misana.Core.Communication.Messages;
using Misana.Core.Effects.Messages;

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
                            ApplyLocally(new OnDropWieldedEffectMessage {
                                OwnerId = e.Id,
                                WieldedId = wieldedEntity.Id,
                                TwoHanded = wieldedEntity.Id
                            }, Manager);
                        }
                    }
                }
                else
                {
                    wielding.Use = r1.Attacking;
                }

                if (r1.PickUp)
                {
                    var ids = _positionTrackingSystem.SlowCoarseQuery(r3.CurrentAreaId - 1, r3.Position, 1).ToList();
                    foreach(var id in ids)
                    {
                        if(id == e.Id)
                            continue;

                        var entity = Manager.GetEntityById(id);
                        if(entity?.Get<DroppedItemComponent>() == null)
                            continue;

                        if (wielding.RightHandEntityId == 0)
                        {
                            ApplyLocally(e, entity, Manager);
                        }
                    }
                }
            }

            var interacting = e.Get<EntityInteractableComponent>();

            if (interacting != null)
                interacting.Interacting = r1.Interact;
        }

        private void ApplyLocally(Entity wieldingEntity, Entity wieldedEntity, EntityManager manager)
        {
            ApplyFromRemote(wieldingEntity, wieldedEntity, manager);
            manager.NoteForSend(new OnPickupEffectMessage(wieldingEntity.Id, wieldedEntity.Id));
        }

        public static void ApplyFromRemote(Entity wieldingEntity, Entity wieldedEntity,  EntityManager manager)
        {


            var wieldedTransform = wieldedEntity.Get<TransformComponent>();
            wieldedTransform.ParentEntityId = wieldingEntity.Id;
            wieldedTransform.Position = Vector2.Zero;


            if (wieldedEntity.Get<DroppedItemComponent>() != null)
            {
                wieldedEntity.Remove<DroppedItemComponent>();
                wieldedTransform.Radius *= 2;
            }

            var w = ComponentRegistry<WieldedComponent>.Take();
            manager.Add(wieldedEntity, w, false);

            var wielding = wieldingEntity.Get<WieldingComponent>();

            wielding.RightHandEntityId = wieldedEntity.Id;
            wielding.TwoHanded = true;
        }

        private static void ApplyLocally(OnDropWieldedEffectMessage message, EntityManager manager)
        {
            ApplyFromRemote(message, manager);
            manager.NoteForSend(message);
        }

        public static void ApplyFromRemote(OnDropWieldedEffectMessage message, EntityManager manager)
        {
            var em = manager;
            var owner = em.GetEntityById(message.OwnerId);
            var wielded = em.GetEntityById(message.WieldedId);

            if (wielded == null || owner == null)
                return;

            var ownerWielding = owner.Get<WieldingComponent>();
            var ownerTransform = owner.Get<TransformComponent>();

            if (ownerWielding == null || ownerTransform == null)
                return;

            if (ownerWielding.RightHandEntityId != message.WieldedId)
                return;

            var wieldedTransform = wielded.Get<TransformComponent>();

            if (wieldedTransform == null)
                return;

            wielded.Remove<WieldedComponent>().Add<DroppedItemComponent>();

            ownerWielding.TwoHanded = false;
            ownerWielding.RightHandEntityId = 0;

            wieldedTransform.Radius /= 2;
            wieldedTransform.ParentEntityId = 0;
            wieldedTransform.Position = ownerTransform.Position;
        }
    }
}
