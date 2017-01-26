using System;
using System.IO;
using Misana.Core.Communication.Components;
using Misana.Core.Communication.Messages;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Effects.Messages;

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

        public override async void Apply(Entity entity, ISimulation simulation)
        {
            if (simulation.Mode == SimulationMode.SinglePlayer)
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
            else if (simulation.Mode == SimulationMode.Server)
            {
                var id = await simulation.CreateEntity(DefinitionName,e =>
                {
                    var transform = e.Get<TransformComponent>();

                    if (SetParent && transform != null)
                    {
                        transform.ParentEntityId = entity.Id;
                    }
                }, null);

                OnCreateEntityEffectMessage message = new OnCreateEntityEffectMessage(id);
                simulation.EffectMessenger.SendMessage(ref message);
            }
            else if (simulation.Mode == SimulationMode.Local)
            {
                OnCreateEntityEffectMessage message;
                var result = simulation.EffectMessenger.TryGetMessage(out message);
                if (result)
                {
                    var id = await simulation.CreateEntity(DefinitionName,message.EntityId,b =>
                    {
                        var transform = b.Get<TransformComponent>();

                        if (SetParent && transform != null)
                        {
                            transform.ParentEntityId = entity.Id;

                            if (entity.Get<SendComponent>() != null)
                                b.Add<SendComponent>();

                        }
                    }, null);
                }

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