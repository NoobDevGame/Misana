using System;
using System.CodeDom;
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
        public bool Weapon { get; set; }

        public CreateEntityEffect()
        {

        }

        public CreateEntityEffect(string definitionName,bool setParent)
        {
            DefinitionName = definitionName;
            SetParent = setParent;
            Weapon = true;
        }

        public override async void Apply(Entity entity, ISimulation simulation)
        {
            if (simulation.Mode == SimulationMode.SinglePlayer)
            {
                await simulation.CreateEntity(DefinitionName,e =>
                {
                    var transform = e.Get<TransformComponent>();

                    if (SetParent && transform != null)
                    {
                        transform.ParentEntityId = entity.Id;
                    }
                },
                e =>{
                    var wielding = entity.Get<WieldingComponent>();
                    if (Weapon && wielding != null)
                    {
                        wielding.RightHandEntityId = e.Id;
                    }
                });
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
                },
                e =>{
                    var wielding = entity.Get<WieldingComponent>();
                    if (Weapon && wielding != null)
                    {
                        wielding.RightHandEntityId = e.Id;
                    }
                });

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

                            var wielding = entity.Get<WieldingComponent>();
                            if (Weapon && wielding != null)
                            {
                                wielding.RightHandEntityId = message.EntityId;
                            }

                        }
                    }, null);
                }

            }
            

        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(DefinitionName);
            bw.Write(SetParent);
            bw.Write(Weapon);
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            DefinitionName = br.ReadString();
            SetParent = br.ReadBoolean();
            Weapon = br.ReadBoolean();
        }
    }
}