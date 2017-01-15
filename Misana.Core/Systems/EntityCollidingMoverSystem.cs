using Misana.Core.Components;
using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Core.Systems
{
    public class EntityCollidingMover : BaseSystemR2O1<EntityColliderComponent, TransformComponent, MotionComponent>
    {
        private struct CollisionPair
        {
            public int HigherEntityId;
            public int LowerEntityId;

            public CollisionPair(int higher, int lower)
            {
                HigherEntityId = higher;
                LowerEntityId = lower;
            }
        }

        private World _world;
        List<int>[][] _areas;
        HashSet<int>[] _occupiedTilesPerArea;
        HashSet<CollisionPair>[] _collisionPairsPerArea;
        
        public void ChangeWorld(World world)
        {
            _world = world;
            _areas = new List<int>[world.CurrentMap.Areas.Length][];
            _occupiedTilesPerArea = new HashSet<int>[world.CurrentMap.Areas.Length];
            _collisionPairsPerArea = new HashSet<CollisionPair>[world.CurrentMap.Areas.Length];

            for (int i = 0; i < world.CurrentMap.Areas.Length; i++)
            {
                var area = world.CurrentMap.Areas[i];

                var n = area.Width * area.Height;
                _areas[i] = new List<int>[n];
                _occupiedTilesPerArea[i] = new HashSet<int>();
                _collisionPairsPerArea[i] = new HashSet<CollisionPair>();

                for (int j = 0; j < n; j++)
                {
                    _areas[i][j] = new List<int>();
                }
            }
        }

        public override void Tick()
        {
            // Cleanup
            for (var index = 0; index < _occupiedTilesPerArea.Length; index++)
            {
                var occ = _occupiedTilesPerArea[index];
                foreach (var a in occ)
                {
                    _areas[index][a].Clear();
                }
                occ.Clear();
            }

            foreach(var cps in _collisionPairsPerArea)
                cps.Clear();
            
            for (int i = 0; i < Count; i++)
            {
                var transformComponent = R2S[i];
                
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
                var collisionPairs = _collisionPairsPerArea[index];

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

                            var pair = new CollisionPair(e1.Id > e2.Id ? e1.Id : e2.Id, e1.Id > e2.Id ? e2.Id : e1.Id);

                            if(!collisionPairs.Add(pair))
                                continue;
                            

                            foreach (var e in entityCollider.CollisionEffects)
                                e.Apply(Manager, e1, e2);

                            foreach (var e in entity2Collider.CollisionEffects)
                                e.Apply(Manager, e2, e1);

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
