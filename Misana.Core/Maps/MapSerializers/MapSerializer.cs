﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Misana.Core.Maps.MapSerializers
{
    public abstract class MapSerializer
    {
        private static Dictionary<Version, Type> serializers = new Dictionary<Version, Type>
        {
            [new Version(0,1)] = typeof(MapSerializer_0_1)  ,
            [new Version(0,2)] = typeof(MapSerializer_0_2)  ,
        };

        public abstract Version MapVersion { get; }

        public static Map DeserializeMap(Version version,BinaryReader br)
        {
            Type serializerType = null;
            if (!serializers.TryGetValue(version,out serializerType))
                throw new Exception($"Serializer not founded Map Version {version}");

            var serializer = (MapSerializer)Activator.CreateInstance(serializerType);

            return serializer.Deserialize(br);
        }

        public static void SerializeMap(Map map,BinaryWriter bw)
        {
            Type serializerType = null;
            if (!serializers.TryGetValue(Map.MapVersion,out serializerType))
                throw new Exception($"Serializer not founded MapVersion {Map.MapVersion}");

            var serializer = (MapSerializer)Activator.CreateInstance(serializerType);

            serializer.Serialize(map,bw);
        }

        protected abstract Map Deserialize(BinaryReader br);
        protected abstract void Serialize(Map map,BinaryWriter bw);
    }
}