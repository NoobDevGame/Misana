﻿using System;
using System.IO;
using Misana.Core.Ecs;

namespace Misana.Core.Events
{
    public abstract class EventCondition
    {
        public abstract bool Test (Entity entity, World world);

        public abstract void Serialize(Version version,BinaryWriter bw);
        public abstract void Deserialize(Version version, BinaryReader br);
    }
}