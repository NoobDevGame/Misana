using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Maps;

namespace Misana.Core.Systems
{
    public class PositionTrackingSystem : BaseSystemR1<TransformComponent>
    {
        public void ChangeMap(Map map)
        {
            _map = map;
            Areas = new List<int>[map.Areas.Count][];
            OccupiedTilesPerArea = new HashSet<int>[map.Areas.Count];

            for (int i = 0; i < map.Areas.Count; i++)
            {
                var area = map.Areas[i];

                var n = area.Width * area.Height;
                Areas[i] = new List<int>[n];
                OccupiedTilesPerArea[i] = new HashSet<int>();

                for (int j = 0; j < n; j++)
                {
                    Areas[i][j] = new List<int>();
                }
            }
        }

        public List<int>[][] Areas;
        public HashSet<int>[] OccupiedTilesPerArea;
        private Map _map;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BoundsFor(Vector2 position, float radius, out int minX, out int maxX, out int minY, out int maxY)
        {
            minX = (int)Math.Floor(position.X - radius);
            maxX = (int)Math.Ceiling(position.X + radius);
            minY = (int)Math.Floor(position.Y - radius);
            maxY = (int)Math.Ceiling(position.Y + radius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckedBoundsFor(Vector2 position, float radius, int areaWidth, int areaHeight, out int minX, out int maxX, out int minY, out int maxY)
        {
            minX = (int)Math.Floor(position.X - radius);
            maxX = (int)Math.Ceiling(position.X + radius);
            minY = (int)Math.Floor(position.Y - radius);
            maxY = (int)Math.Ceiling(position.Y + radius);

            minX = Math.Max(0, Math.Min(areaWidth - 1, minX));
            minY = Math.Max(0, Math.Min(areaHeight - 1 , minY));
            maxX = Math.Min(areaWidth- 1, Math.Max(0, maxX));
            maxY = Math.Min(areaHeight - 1, Math.Max(0, maxY));
        }
        
        public IEnumerable<int> SlowCoarseQuery(int areaIndex, Vector2 position, float radius)
        {
            var areaOccupation = Areas[areaIndex];
            var area = _map.Areas[areaIndex];

            int minCellX, maxCellX, minCellY, maxCellY;
            CheckedBoundsFor(
                position, radius, 
                area.Width, area.Height,
                out minCellX, out maxCellX, 
                out minCellY, out maxCellY
            );
         
            var entityIds = new HashSet<int>();

            for (int x = minCellX; x <= maxCellX; x++)
            {
                for (int y = minCellY; y <= maxCellY; y++)
                {
                    if (position.X - radius > x + 1 ||
                        position.X + radius < x ||
                        position.Y - radius > y + 1 ||
                        position.Y + radius < y)
                        continue;
                    
                    var tileIndex = area.GetTileIndex(x, y);
                    foreach (var i in areaOccupation[tileIndex])
                        entityIds.Add(Entities[i].Id);
                }
            }

            return entityIds;
        }

        public override void Tick()
        {
            for (var index = 0; index < OccupiedTilesPerArea.Length; index++)
            {
                var occ = OccupiedTilesPerArea[index];
                foreach (var a in occ)
                {
                    Areas[index][a].Clear();
                }
                occ.Clear();
            }

            for (int i = 0; i < Count; i++)
            {
                var transformComponent = R1S[i];

                if (transformComponent.CurrentAreaId < 0)
                    continue;
                var position = transformComponent.Position;
                int minCellX,maxCellX, minCellY, maxCellY;
                var a = _map.Areas[transformComponent.CurrentAreaId - 1];
                CheckedBoundsFor(
                    position, transformComponent.Radius, 
                    a.Width, a.Height,
                    out minCellX, out maxCellX, 
                    out minCellY, out maxCellY
                );

                var area = Areas[transformComponent.CurrentAreaId - 1];

                for (int x = minCellX; x <= maxCellX; x++)
                {
                    for (int y = minCellY; y <= maxCellY; y++)
                    {
                        if (position.X - transformComponent.Radius > x + 1 ||
                            position.X + transformComponent.Radius < x ||
                            position.Y - transformComponent.Radius > y + 1 ||
                            position.Y + transformComponent.Radius < y)
                            continue;

                        var tileIndex = a.GetTileIndex(x, y);


                        area[tileIndex].Add(i);
                        OccupiedTilesPerArea[transformComponent.CurrentAreaId - 1].Add(tileIndex);
                    }
                }
            }
        }
    }
}