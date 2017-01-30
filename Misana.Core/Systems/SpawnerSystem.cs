using Misana.Core.Components;
using Misana.Core.Ecs;

namespace Misana.Core.Systems
{
    public class SpawnerSystem : BaseSystemR2<SpawnerComponent, TransformComponent>
    {
        protected override void Update(Entity e, SpawnerComponent r1, TransformComponent r2)
        {
            if(!r1.Active)
                return;

            if (r1.TotalSpawnLimit > 0 && r1.TotalSpawnLimit <= r1.TotalSpawned)
                return;

            if (r1.CoolDown > 0 && r1.CoolDown > Manager.GameTime.TotalTime.TotalSeconds - r1.LastSpawned)
                return;

            if (r1.MaxAlive > 0 && r1.MaxAlive <= r1.AliveSpawnedEntityIds.Count)
            {
                var removed = false;
                for (int i = 0; i < r1.AliveSpawnedEntityIds.Count; i++)
                {
                    if (Manager.GetEntityById(r1.AliveSpawnedEntityIds[i]) == null)
                    {
                        removed = true;
                        r1.AliveSpawnedEntityIds.RemoveAt(i--);
                    }
                }

                if (!removed)
                    return;
            }
            
            var temp = r1.Template;
            r1.Template = r1.Template.Copy();

            var tf = temp.Get<TransformComponent>();
            tf.Position = r2.AbsolutePosition(Manager) + (r1.SpawnDirection * tf.Radius);
            tf.CurrentArea = r2.CurrentArea;

            if (r1.Projectile)
            {
                temp.Add<ProjectileComponent>(x => x.Move = r1.SpawnDirection * 0.5f);
            }

            var spawned = temp.Commit(Manager);
            
            r1.LastSpawned = Manager.GameTime.TotalTime.TotalSeconds;

            if(r1.MaxAlive > 0)
                r1.AliveSpawnedEntityIds.Add(spawned.Id);
        }
    }
}