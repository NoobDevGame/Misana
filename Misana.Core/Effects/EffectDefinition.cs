﻿using System;
using System.IO;
using Misana.Core.Ecs;

namespace Misana.Core.Effects
{
    public abstract class EffectDefinition
    {
        public abstract void Apply(Entity entity, World world);

        public abstract void Serialize(Version version,BinaryWriter bw);
        public abstract void Deserialize(Version version, BinaryReader br);
    }
}