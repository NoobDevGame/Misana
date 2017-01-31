using System;
using System.IO;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Maps;

namespace Misana.Core.Entities.BaseDefinition
{
    public class SpawnerDefinition : ComponentDefinition<SpawnerComponent>
    {
        public string SpawnedDefinitionName;
        public int MaxAlive;
        public int TotalSpawnLimit;
        public double CoolDown;
        public bool Active;
        public Vector2 SpawnDirection;
        public bool Projectile;

        public override void Deserialize(Version version, BinaryReader br)
        {
            MaxAlive = br.ReadInt32();
            TotalSpawnLimit = br.ReadInt32();
            Active = br.ReadBoolean();
            Projectile = br.ReadBoolean();
            CoolDown = br.ReadDouble();
            SpawnDirection = new Vector2(br.ReadSingle(), br.ReadSingle());
            SpawnedDefinitionName = br.ReadString();
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(MaxAlive);
            bw.Write(TotalSpawnLimit);
            bw.Write(Active);
            bw.Write(Projectile);
            bw.Write(CoolDown);
            bw.Write(SpawnDirection.X);
            bw.Write(SpawnDirection.Y);
            bw.Write(SpawnedDefinitionName);
        }

        public override void OnApplyDefinition(EntityBuilder entity, Map map, SpawnerComponent component, ISimulation sim)
        {
            component.Template = EntityCreator.CreateEntity(map.GlobalEntityDefinitions[SpawnedDefinitionName], map, new EntityBuilder(), sim);
            component.MaxAlive = MaxAlive;
            component.TotalSpawnLimit = TotalSpawnLimit;
            component.CoolDown = CoolDown;
            component.Active = Active;
            component.SpawnDirection = SpawnDirection;
            component.Projectile = Projectile;
        }
    }
}