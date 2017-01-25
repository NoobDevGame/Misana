using System;
using System.IO;
using Misana.Core.Components;
using Misana.Core.Ecs;

namespace Misana.Core.Effects.BaseEffects
{
    public class CreateEntityEffect : EffectDefinition
    {
        public string DefinitionName { get; set; }
        public bool SetParent { get; set; }

        public CreateEntityEffect()
        {

        }

        public CreateEntityEffect(string definitionName,bool setParent)
        {
            DefinitionName = definitionName;
            SetParent = setParent;
        }

        public override void Apply(Entity entity, ISimulation simulation)
        {
            try
            {
                simulation.CreateEntity(DefinitionName,e =>
                {
                    var transform = e.Get<TransformComponent>();

                    if (SetParent && transform != null)
                    {
                        transform.ParentEntityId = entity.Id;
                    }
                }, null);
            }
            catch (Exception e)
            {
            }
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(DefinitionName);
            bw.Write(SetParent);
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            DefinitionName = br.ReadString();
            SetParent = br.ReadBoolean();
        }
    }
}