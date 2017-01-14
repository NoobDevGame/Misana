using System;
using System.IO;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Maps;

namespace Misana.Core.Entities.BaseDefinition
{
    public class CharacterDefinition : ComponentDefinition<CharacterComponent>
    {
        public string Name { get; set; }

        public CharacterDefinition()
        {
            Name = "Unnamed";
        }

        public CharacterDefinition(string name)
        {
            Name = name;
        }

        public override void OnApplyDefinition(Entity entity, Map map, CharacterComponent component)
        {
            component.Name = Name;
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(Name);
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            Name = br.ReadString();
        }
    }
}