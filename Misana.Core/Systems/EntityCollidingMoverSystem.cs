using Misana.Core.Components;
using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Core.Systems
{
    public class EntityCollidingMover : BaseSystemR2O1<EntityCollider, TransformComponent, MotionComponent>
    {
        public override void Tick()
        {
            for (int i = 0; i < Count; i++)
            {
                var entityCollider = R1S[i];
                var positionComponent = R2S[i];
                var motionComponent = O1S[i];

                if (positionComponent.CurrentArea == null)
                    continue;

                for (int j = i + 1; j < Count; j++)
                {
                    var position2Component = R2S[j];

                    if (position2Component.CurrentArea == null)
                        continue;

                    if (position2Component.CurrentArea.Id != positionComponent.CurrentArea.Id)
                        continue;

                    var entity2Collider = R1S[j];
                    var motion2Component = O1S[j];

                    var vecDistance = positionComponent.Position - position2Component.Position;

                    var distance = vecDistance.Length() - positionComponent.Radius - position2Component.Radius;

                    if (distance > 0)
                        continue;
                    
                    foreach(var e in entityCollider.CollisionEffects)
                        e.Apply(Manager, Entities[i], Entities[j]);

                    foreach (var e in entity2Collider.CollisionEffects)
                        e.Apply(Manager, Entities[j], Entities[i]);

                    if(!entityCollider.Blocked || !entity2Collider.Blocked)
                        continue;

                    if(motionComponent == null && motion2Component == null)
                        continue;

                    vecDistance = vecDistance.Normalize() * Math.Abs(distance);
                    
                    if (!entityCollider.Fixed && entity2Collider.Fixed && motionComponent != null)
                    {
                        motionComponent.Move += vecDistance;
                    }
                    else if (entityCollider.Fixed && !entity2Collider.Fixed && motion2Component != null)
                    {
                        motion2Component.Move -= vecDistance;
                    }
                    else if(motionComponent != null && motion2Component != null )
                    {
                        var mass = entityCollider.Mass + entity2Collider.Mass;
                        motionComponent.Move += vecDistance * (entity2Collider.Mass / mass);
                        motion2Component.Move -= vecDistance * (entityCollider.Mass / mass);
                    }
                    else if( motionComponent == null && !entity2Collider.Fixed )
                    {
                        motion2Component.Move -= vecDistance;
                    }
                    else if( motion2Component == null && !entityCollider.Fixed )
                    {
                        motionComponent.Move += vecDistance;
                    }

                }
            }
        }
    }
}
