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

        public override void Deserialize(Version version, BinaryReader br)
        {
            MaxAlive = br.ReadInt32();
            TotalSpawnLimit = br.ReadInt32();
            CoolDown = br.ReadDouble();
            Active = br.ReadBoolean();
            SpawnedDefinitionName = br.ReadString();
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(MaxAlive);
            bw.Write(TotalSpawnLimit);
            bw.Write(CoolDown);
            bw.Write(Active);
            bw.Write(SpawnedDefinitionName);
        }

        public override void OnApplyDefinition(EntityBuilder entity, Map map, SpawnerComponent component)
        {
            component.Template = EntityCreator.CreateEntity(map.GlobalEntityDefinitions[SpawnedDefinitionName], map, new EntityBuilder());
            component.MaxAlive = MaxAlive;
            component.TotalSpawnLimit = TotalSpawnLimit;
            component.CoolDown = CoolDown;
            component.Active = Active;
        }
    }
}