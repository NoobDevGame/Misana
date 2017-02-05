using System;
using System.IO;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Effects.Messages;
using Misana.Core.Maps;
using Misana.Core.Systems;

namespace Misana.Core.Entities.BaseDefinition
{
    public class WieldingDefinition : ComponentDefinition<WieldingComponent>
    {
        public string WieldedDefinitionName;

        public override void OnApplyDefinition(EntityBuilder entity, Map map, WieldingComponent component, ISimulation sim)
        {
            EntityDefinition wieldedDef;
            if (string.IsNullOrWhiteSpace(WieldedDefinitionName) || !sim.CurrentMap.GlobalEntityDefinitions.TryGetValue(WieldedDefinitionName, out wieldedDef))
            {
                return;
            }

            var wieldedBuilder = EntityCreator.CreateEntity(wieldedDef, map, new EntityBuilder(), sim);

            entity.AttachedEntities.Add(new AttachedEntity(wieldedBuilder, AttachmentType.Wielded));
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            base.Serialize(version, bw);
            if(string.IsNullOrWhiteSpace(WieldedDefinitionName))
                bw.Write(false);
            else
            {
                bw.Write(true);
                bw.Write(WieldedDefinitionName);
            }

        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            base.Deserialize(version, br);
            if(br.ReadBoolean())
                WieldedDefinitionName = br.ReadString();
        }
    }
}