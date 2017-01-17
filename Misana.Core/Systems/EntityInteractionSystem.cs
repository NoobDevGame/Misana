using System;
using System.Collections.Generic;
using Misana.Core.Components;
using Misana.Core.Ecs;

namespace Misana.Core.Systems
{
    public class EntityInteractionSystem : BaseSystemR2<TransformComponent,EntityInteractableComponent>
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
            _areas = new List<int>[world.CurrentMap.Areas.Count][];
            _occupiedTilesPerArea = new HashSet<int>[world.CurrentMap.Areas.Count];
            _collisionPairsPerArea = new HashSet<CollisionPair>[world.CurrentMap.Areas.Count];

            for (int i = 0; i < world.CurrentMap.Areas.Count; i++)
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
                var transformComponent = R1S[i];
                var interactableComponent = R2S[i];

                if (transformComponent.CurrentArea == null)
                    continue;

                Vector2 size = new Vector2(interactableComponent.InteractionRadius,interactableComponent.InteractionRadius);
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
                        var positionComponent = R1S[i];
                        var interactableComponent = R2S[i];

                        for (int l = k + 1; l < entityIndexes.Count; l++)
                        {
                            var j = entityIndexes[l];
                            var e2 = Entities[j];
                            var position2Component = R1S[j];
                            var interactable2Component = R2S[j];

                            if(!interactableComponent.Interacting && !interactable2Component.Interacting)
                                continue;

                            var vecDistance = positionComponent.Position - position2Component.Position;

                            var distance = vecDistance.Length() - interactableComponent.InteractionRadius - interactable2Component.InteractionRadius;

                            if (distance > 0)
                                continue;

                            var pair = new CollisionPair(e1.Id > e2.Id ? e1.Id : e2.Id, e1.Id > e2.Id ? e2.Id : e1.Id);

                            if(!collisionPairs.Add(pair))
                                continue;

                            if(interactableComponent.Interacting)
                            foreach (var e in interactable2Component.OnInteractionEvents)
                                e.Apply(Manager, e2, e1, _world);

                            if(interactable2Component.Interacting)
                            foreach (var e in interactableComponent.OnInteractionEvents)
                                e.Apply(Manager, e1, e2, _world);
                        }
                    }
                }
            }
        }
    }
}