using Misana.Core.Communication.Systems;
using Misana.Core.Components;
using Misana.Core.Components.StatusComponents;
using Misana.Core.Ecs;

namespace Misana.Core.Systems
{
    public class SpawnerSystem : BaseSystemR2<SpawnerComponent, TransformComponent>
    {
        protected override void Update(Entity e, SpawnerComponent r1, TransformComponent r2)
        {
            if(r1.RunsOn != RunsOn.Both && (byte) Manager.Mode != (byte)r1.RunsOn)
                return;
            
            if(!r1.Active)
                return;
            
            if(!CanSpawn(r1, r2))
                return;

            SpawnLocally(e, r1, r2);
        }

        private void SpawnLocally(Entity e,SpawnerComponent r1, TransformComponent r2)
        {
            ProjectileComponent pc = null;
            var tf = ComponentRegistry<TransformComponent>.Take();

            var radius = r1.Template.Get<TransformComponent>()?.Radius ?? 0.5f;

            tf.Position = r2.AbsolutePosition(Manager) + r1.SpawnDirection * (radius + r2.Radius);
            tf.CurrentArea = r2.CurrentArea;

            if (r1.Projectile)
            {
                pc = ComponentRegistry<ProjectileComponent>.Take();
                pc.Move = r1.SpawnDirection * 0.25f;
                var attack = CalcAttack(e);
                pc.BaseAttack = attack;
            }
            
            var spawned = SpawnRemote(r1, Manager.NextId(), tf, pc);
            r1.LastSpawned = Manager.GameTime.TotalTime.TotalSeconds;
            
            Manager.NoteForSend(new SpawnerTriggeredMessage(e.Id, spawned.Id, tf, pc));
        }

        public Entity SpawnRemote(SpawnerComponent r1, int entityId, TransformComponent spawnedTransform, ProjectileComponent spawnedProjectileComponent)
        {
            var temp = r1.Template.Copy();
            temp.Add(spawnedTransform);

            if (spawnedProjectileComponent != null)
                temp.Add(spawnedProjectileComponent);



            if (r1.MaxAlive > 0)
                r1.AliveSpawnedEntityIds.Add(entityId);

            return temp.Commit(Manager, entityId);

        }

        public bool CanSpawn(SpawnerComponent r1, TransformComponent r2)
        {
            if (r1.TotalSpawnLimit > 0 && r1.TotalSpawnLimit <= r1.TotalSpawned)
                return false;

            if (r1.CoolDown > 0 && r1.CoolDown > Manager.GameTime.TotalTime.TotalSeconds - r1.LastSpawned)
                return false;

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
                    return false ;
            }
            return true;
        }

        private int CalcAttack(Entity entity)
        {
            int attack = 0;

            do
            {
                var state = entity.Get<StatsComponent>();

                attack += state?.Attack ?? 0;

                var transform = entity.Get<TransformComponent>();
                entity = transform?.Parent(Manager);
                if (entity == null)
                    break;


            } while (true);

            return attack;
        }
    }
}