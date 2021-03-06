﻿using System;
using System.IO;
using Misana.Core.Ecs;

namespace Misana.Core.Events
{
    public abstract class EventCondition
    {
        public abstract bool Test (EntityManager manager, Entity self, Entity other, ISimulation simulation);

        public abstract void Serialize(Version version,BinaryWriter bw);
        public abstract void Deserialize(Version version, BinaryReader br);

        public abstract EventCondition Copy();
    }
}