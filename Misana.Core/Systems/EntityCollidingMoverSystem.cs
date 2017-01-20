using Misana.Core.Components;
using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Core.Systems
{
    public class EntityCollidingMoverSystem : BaseSystemR2O1<EntityColliderComponent, TransformComponent, MotionComponent>
    {
        public EntityCollidingMoverSystem()
        {
            _colArraySize = Capacity * 2;
            _collisionChecks = new bool[_colArraySize][];

            for (int i = 0; i < _colArraySize; i++)
            {
                _collisionChecks[i] = new bool[_colArraySize];
            }
        }

        private ISimulation _simulation;
        List<int>[][] _areas;
        HashSet<int>[] _occupiedTilesPerArea;
        
        public void ChangeSimulation(ISimulation simulation)
        {
            _simulation = simulation;
            _areas = new List<int>[simulation.CurrentMap.Areas.Count][];
            _occupiedTilesPerArea = new HashSet<int>[simulation.CurrentMap.Areas.Count];

            for (int i = 0; i < simulation.CurrentMap.Areas.Count; i++)
            {
                var area = simulation.CurrentMap.Areas[i];

                var n = area.Width * area.Height;
                _areas[i] = new List<int>[n];
                _occupiedTilesPerArea[i] = new HashSet<int>();

                for (int j = 0; j < n; j++)
                {
                    _areas[i][j] = new List<int>();
                }
            }
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
            for (var index = 0; index < _occupiedTilesPerArea.Length; index++)
            {
                var occ = _occupiedTilesPerArea[index];
                foreach (var a in occ)
                {
                    _areas[index][a].Clear();
                }
                occ.Clear();
            }

            for (int i = 0; i < Count; i++)
            {
                var transformComponent = R2S[i];

                for (int j = 0; j < Count; j++)
                {
                    _collisionChecks[i][j] = false;
                }


                if (transformComponent.CurrentArea == null)
                    continue;

                Vector2 size = transformComponent.HalfSize;
                var position = transformComponent.Position;

                int minCellX = (int)Math.Floor(position.X - size.X);
                int maxCellX = (int)Math.Ceiling(position.X + size.X);
                int minCellY = (int)Math.Floor(position.Y - size.Y);
                int maxCellY = (int)Math.Ceiling(position.Y + size.X);
                

                // Schleife über alle betroffenen Zellen zur Ermittlung der ersten Kollision
                for (int x = minCellX; x <= maxCellX; x++)
                {
                    for (int y = minCellY; y <= maxCellY; y++)
                    {

                        // Zellen ignorieren die vom Spieler nicht berührt werden
                        if (position.X - size.X > x + 1 ||
                            position.X + size.X < x ||
                            position.Y - size.Y > y + 1 ||
                            position.Y + size.Y < y)
                            continue;

                        var tileIndex = transformComponent.CurrentArea.GetTileIndex(x, y);
                        var area = _areas[transformComponent.CurrentArea.Id - 1];
                        if (tileIndex >= 0 && tileIndex < area.Length)
                        {
                            area[tileIndex].Add(i);
                            _occupiedTilesPerArea[transformComponent.CurrentArea.Id - 1].Add(tileIndex);
                        }
                    }
                }
            }
            
            for (var index = 0; index < _occupiedTilesPerArea.Length; index++)
            {
                var occ = _occupiedTilesPerArea[index];

                foreach (var a in occ)
                {
                    var entityIndexes = _areas[index][a];
                    if (entityIndexes.Count <= 1)
                        continue;

                    for (int k = 0; k < entityIndexes.Count; k++)
                    {
                        var i = entityIndexes[k];
                        var e1 = Entities[i];
                        var entityCollider = R1S[i];
                        var positionComponent = R2S[i];
                        var motionComponent = O1S[i];

                        for (int l = k + 1; l < entityIndexes.Count; l++)
                        {
                            var j = entityIndexes[l];
                            var e2 = Entities[j];
                            var position2Component = R2S[j];
                            var entity2Collider = R1S[j];
                            var motion2Component = O1S[j];

                            var vecDistance = positionComponent.Position - position2Component.Position;

                            var distance = vecDistance.Length() - positionComponent.Radius - position2Component.Radius;

                            if (distance > 0)
                                continue;

                            var higherIndex = e1.Id > e2.Id ? i : j;
                            var lowerIndex = e1.Id > e2.Id ? j : i;

                            if(_collisionChecks[higherIndex][lowerIndex])
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
