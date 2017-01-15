using System;
using System.IO;
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Maps;

namespace Misana.Core.Entities.BaseDefinition
{
    public class EntityColliderDefinition : ComponentDefinition<EntityColliderComponent>
    {
        public bool Blocked { get; set; } = true;
        public bool Fixed { get; set; } = false;

        public float Mass { get; set; } = 50f;

        public override void OnApplyDefinition(EntityBuilder entity, Map map, EntityColliderComponent component)
        {
            component.Blocked = Blocked;
            component.Fixed = Fixed;
            component.Mass = Mass;
            component.AppliesSideEffect = ApplySidesEffect;
        }

        public override void Serialize(Version version, BinaryWriter bw)
        {
            bw.Write(Mass);
            bw.Write(Blocked);
            bw.Write(Fixed);
            bw.Write(ApplySidesEffect);
        }

        public override void Deserialize(Version version, BinaryReader br)
        {
            Mass = br.ReadSingle();
            Blocked = br.ReadBoolean();
            Fixed = br.ReadBoolean();
            ApplySidesEffect = br.ReadBoolean();
        }
    }
}