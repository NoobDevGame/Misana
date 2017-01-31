using System;
using System.IO;
using Misana.Core.Components.StatusComponents;
using Misana.Core.Ecs;
using Misana.Core.Maps;

namespace Misana.Core.Entities.StatusDefinitions
{
    public class StateDefinition : ComponentDefinition<StatsComponent>
    {
        public int Attack { get; set; } = 1;
        public int Defence { get; set; } = 0;

        public StateDefinition()
        {

        }

        public StateDefinition(int attack, int defence)
        {
            Attack = attack;
            Defence = defence;
        }

        public override void OnApplyDefinition(EntityBuilder entity, Map map, StatsComponent component, ISimulation sim)
        {
            component.Attack = Attack;
            component.Defense = Defence;
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(Attack);
            bw.Write(Defence);
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            Attack = br.ReadInt32();
            Defence = br.ReadInt32();
        }
    }
}