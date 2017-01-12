using Misana.Core.Components;
using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Core.Systems
{
    public class EntityCollidingMover : BaseSystemR3O1<EntityCollider, PositionComponent, DimensionComponent,MotionComponent>
    {
        public override void Tick()
        {
            for (int i = 0; i < Count; i++)
            {
                var entityCollider = R1S[i];
                var positionComponent = R2S[i];
                var dimensionComponent = R3S[i];
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
                    var dimension2Component = R3S[j];
                    var motion2Component = O1S[j];

                    var vecDistance = positionComponent.Position - position2Component.Position;

                    var distance = vecDistance.Length() - dimensionComponent.Radius - dimension2Component.Radius;

                    if (distance > 0)
                        continue;

                    if(entityCollider.AppliesSideEffect)
                        Entities[i].Add<EntityCollision>(ec => {
                            ec.OtherEntityIds.Add(Entities[j].Id);
                        },false);

                    if (entity2Collider.AppliesSideEffect)
                        Entities[j].Add<EntityCollision>(ec => {
                            ec.OtherEntityIds.Add(Entities[i].Id);
                        },false);

                    if(!entityCollider.Blocked || !entity2Collider.Blocked)
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
                }
            }
        }
    }
}
