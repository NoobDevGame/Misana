using Misana.Core.Components;
using Misana.Core.Ecs;
using System;

namespace Misana.Core.Systems
{
    public class EntityCollidingMoverSystem : BaseSystemR2O1<EntityColliderComponent, TransformComponent, MotionComponent>
    {
        private readonly PositionTrackingSystem _posTracker;

        public EntityCollidingMoverSystem(PositionTrackingSystem posTracker)
        {
            _posTracker = posTracker;
            _colArraySize = Capacity * 2;
            _collisionChecks = new bool[_colArraySize][];

            for (int i = 0; i < _colArraySize; i++)
            {
                _collisionChecks[i] = new bool[_colArraySize];
            }
        }

        private ISimulation _simulation;
        
        public void ChangeSimulation(ISimulation simulation)
        {
            _simulation = simulation;
        }

        private int _colArraySize;
        private bool[][] _collisionChecks;

        protected override void Grow(int to)
        {
            base.Grow(to);

            if (to > _colArraySize)
            {
                _colArraySize = to * 2;
                _collisionChecks = new bool[_colArraySize][];

                for (int i = 0; i < _colArraySize; i++)
                {
                    _collisionChecks[i] = new bool[_colArraySize];
                }
            }
        }

        public override void Tick()
        {
            for (int i = 0; i < Count; i++)
            {
                for (int j = 0; j < Count; j++)
                {
                    _collisionChecks[i][j] = false;
                }
            }

            for (var index = 0; index < _posTracker.OccupiedTilesPerArea.Length; index++)
            {
                var occ = _posTracker.OccupiedTilesPerArea[index];

                foreach (var a in occ)
                {
                    var entityIndexes = _posTracker.Areas[index][a];
                    if (entityIndexes.Count <= 1)
                        continue;

                    for (int k = 0; k < entityIndexes.Count; k++)
                    {
                        var i = entityIndexes[k];
                        var e1 = _posTracker.Entities[i];
                        if(!IndexMap.TryGetValue(e1, out i))
                            continue;

                        var entityCollider = R1S[i];
                        var positionComponent = R2S[i];
                        var motionComponent = O1S[i];

                        for (int l = k + 1; l < entityIndexes.Count; l++)
                        {
                            var j = entityIndexes[l];
                            var e2 = _posTracker.Entities[j];

                            if (!IndexMap.TryGetValue(e2, out j))
                                continue;

                            var position2Component = R2S[j];
                            var entity2Collider = R1S[j];
                            var motion2Component = O1S[j];

                            var vecDistance = positionComponent.Position - position2Component.Position;

                            var distance = vecDistance.Length() - positionComponent.Radius - position2Component.Radius;

                            if (distance > 0)
                                continue;

                            var higherIndex = e1.Id > e2.Id ? i : j;
                            var lowerIndex = e1.Id > e2.Id ? j : i;

                            if (_collisionChecks[higherIndex][lowerIndex])
                                continue;

                            _collisionChecks[higherIndex][lowerIndex] = true;


                            foreach (var e in entityCollider.OnCollisionEvents)
                                e.Apply(Manager, e1, e2, _simulation);

                            foreach (var e in entity2Collider.OnCollisionEvents)
                                e.Apply(Manager, e2, e1, _simulation);

                            if (!entityCollider.Blocked || !entity2Collider.Blocked)
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
                            else if (motionComponent != null && motion2Component != null)
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
    }
}
