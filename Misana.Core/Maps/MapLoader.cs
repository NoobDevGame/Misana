using System;
using System.IO;
using System.Linq;
using Misana.Core.Components;
using Misana.Core.Effects.BaseEffects;
using Misana.Core.Entities;
using Misana.Core.Entities.BaseDefinition;
using Misana.Core.Entities.Events;
using Misana.Core.Events.Entities;
using Misana.Core.Maps.MapSerializers;

namespace Misana.Core.Maps
{
    public static class MapLoader
    {

        private static readonly string dirPath =  Path.Combine("Content", "Maps");

        public static Map LoadPath(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("File not founded", path);
            Map map;

            using (var fs = File.Open(path, FileMode.Open, FileAccess.ReadWrite))
            using (var br = new BinaryReader(fs))
            {
                var major = br.ReadInt32();
                var minor = br.ReadInt32();
                var build = br.ReadInt32();
                var revision = br.ReadInt32();

                Version version = null;

                if (build > 0 && revision > 0)
                    version = new Version(major, minor, build, revision);
                else if (build > 0)
                    version = new Version(major, minor, build);
                else
                    version = new Version(major, minor);

                map =  MapSerializer.DeserializeMap(version, br);

            }
            if (!map.GlobalEntityDefinitions.ContainsKey("Player"))
            {
                EntityDefinition playerDefinition = new EntityDefinition(map.GetNextDefinitionId());
                playerDefinition.Definitions.Add(new HealthDefinition());
                playerDefinition.Definitions.Add(new CharacterRenderDefinition(new Index2(1,9)));
                playerDefinition.Definitions.Add(new MotionComponentDefinition());
                playerDefinition.Definitions.Add(new EntityColliderDefinition());
                playerDefinition.Definitions.Add(new BlockColliderDefinition());
                playerDefinition.Definitions.Add(new EntityFlagDefintion());
                playerDefinition.Definitions.Add(new EntityInteractableDefinition());
                playerDefinition.Definitions.Add(new TransformDefinition(new Vector2(5, 3),map.StartArea,0.5f));
                playerDefinition.Definitions.Add(new WieldingDefinition());

                var createDefinition = new CreateDefinition();
                createDefinition.OnCreateEvents.Add(new ApplyEffectEvent(new CreateEntityEffect("Bow",true)){ApplyTo = ApplicableTo.Self});

                playerDefinition.Definitions.Add(createDefinition);
                map.GlobalEntityDefinitions.Add("Player",playerDefinition);

                EntityDefinition bowDefinition = new EntityDefinition(map.GetNextDefinitionId());
                bowDefinition.Definitions.Add(new WieldableDefinition());
                bowDefinition.Definitions.Add(new CharacterRenderDefinition());
                bowDefinition.Definitions.Add(new WieldedDefinition(0.5f,0.5f));
                bowDefinition.Definitions.Add(new FacingDefinition());
                bowDefinition.Definitions.Add(new TransformDefinition(new Vector2(0.3f,0.3f),map.StartArea));

                map.GlobalEntityDefinitions.Add("Bow",bowDefinition);
                //TODO:BowDebug

            }

            return map;
        }

        public static Map Load(string name)
        {
            var path = Path.Combine(dirPath, $"{name}.mm");
            return LoadPath(path);

        }

        public static void Save(Map map, string path)
        {

            var fs = File.Create(path);
            //using (var fs = File.Create(path))
            using (var bw = new BinaryWriter(fs))
            {
                //Version
                bw.Write(Map.MapVersion.Major);
                bw.Write(Map.MapVersion.Minor);
                bw.Write(Map.MapVersion.Build);
                bw.Write(Map.MapVersion.Revision);

                MapSerializer.SerializeMap(map,bw);

            }
        }

        public static Map CreateMapFromTiled(string name,params string[] tiledareas)
        {
            Area[] areas = new Area[tiledareas.Length];
            for (int i = 0; i < areas.Length; i++)
            {
                areas[i] = TiledMapConverter.LoadArea(tiledareas[i],i+1);
            }

            return new Map(name,areas.First(),areas.ToList(),0);
        }
    }
}