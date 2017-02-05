using Misana.Core.Components;
using Misana.Core.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Misana.Core.Maps;

namespace Misana.Core.Systems
{
    public class BlockCollidingMoverSystem : BaseSystemR3<MotionComponent, TransformComponent, BlockColliderComponent>
    {

        private readonly float gap = 0.00001f;
        private Map _map;

        public void ChangeMap(Map map)
        {
            _map = map;
        }

        protected override void Update(Entity e, MotionComponent r1, TransformComponent r2, BlockColliderComponent _)
        {
            bool collision = false;
            int loops = 0;

            Vector2 size = r2.HalfSize;
            var area = _map.Areas[r2.CurrentAreaId - 1];

            var move = r1.Move;

            do
            {
                // Grenzbereiche für die zu überprüfenden Zellen ermitteln
                Vector2 position = r2.Position + move;

                int minCellX = (int)Math.Floor(position.X - size.X);
                int maxCellX = (int)Math.Ceiling(position.X + size.X);
                int minCellY = (int)Math.Floor(position.Y - size.Y);
                int maxCellY = (int)Math.Ceiling(position.Y + size.X);

                collision = false;
                float minImpact = 2f;
                int minAxis = 0;


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


                        // Zellen ignorieren die den Spieler nicht blockieren
                        if (!area.IsCellBlocked(x, y))
                            continue;

                        collision = true;


                        // Kollisionszeitpunkt auf X-Achse ermitteln
                        float diffX = float.MaxValue;
                        if (move.X > 0)
                            diffX = position.X + size.X - x + gap;
                        if (move.X < 0)
                            diffX = position.X - size.X - (x + 1) - gap;

                        float impactX = 1f - (diffX / move.X);



                        // Kollisionszeitpunkt auf Y-Achse ermitteln
                        float diffY = float.MaxValue;
                        if (move.Y > 0)
                            diffY = position.Y + size.Y - y + gap;
                        if (move.Y < 0)
                            diffY = position.Y -size.Y - (y + 1) - gap;

                        float impactY = 1f - (diffY / move.Y);

                        // Relevante Achse ermitteln
                        // Ergibt sich aus dem spätesten Kollisionszeitpunkt
                        int axis = 0;
                        float impact = 0;
                        if (impactX > impactY)
                        {
                            axis = 1;
                            impact = impactX;
                        }
                        else
                        {
                            axis = 2;
                            impact = impactY;
                        }

                        // Ist diese Kollision eher als die bisher erkannten
                        if (impact < minImpact)
                        {
                            minImpact = impact;
                            minAxis = axis;
                        }
                    }
                }


                // Im Falle einer Kollision in diesem Schleifendurchlauf...
                if (collision)
                {
                    // X-Anteil ab dem Kollisionszeitpunkt kürzen
                    if (minAxis == 1)
                    {
                        move *= new Vector2(minImpact, 1f);
                    }

                    // Y-Anteil ab dem Kollisionszeitpunkt kürzen
                    if (minAxis == 2)
                    {
                        move *= new Vector2(1f, minImpact);
                    }
                }
                loops++;
            } while (collision && loops < 2);

            r1.Move = move;
        }
    }
}
