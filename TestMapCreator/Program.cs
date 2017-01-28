using System;
using System.Collections.Generic;
using Misana.Core.Maps;
using System.Linq;
using System.IO;
using Misana.Core.Entities;
using Misana.Core.Entities.BaseDefinition;
using Misana.Core.Events.OnUse;
using Misana.Core.Effects.BaseEffects;
using Misana.Core;
using Misana.Core.Entities.Events;
using Misana.Core.Events.Entities;

namespace TestMapCreator
{
    internal class Program
    {
        public static void Main(string[] args)
        {
			List<string> maps = new List<string> ();
			maps.Add ("Markhausen.West"); //1
            maps.Add ("Markhausen.Weg"); //2

            var paths = maps.Select (i => Path.Combine("MisanaMap","Maps",string.Format("{0}.json",i))).ToArray();

			var map = MapLoader.CreateMapFromTiled ("DebugMap", paths);

			//Definitionen

            //Player
			EntityDefinition playerDefinition = new EntityDefinition("Player",map.GetNextDefinitionId());
			playerDefinition.Definitions.Add(new HealthDefinition());
			playerDefinition.Definitions.Add(new CharacterRenderDefinition(new Index2(1,9)));
			playerDefinition.Definitions.Add(new MotionComponentDefinition());
			playerDefinition.Definitions.Add(new EntityColliderDefinition());
			playerDefinition.Definitions.Add(new BlockColliderDefinition());
			playerDefinition.Definitions.Add(new EntityFlagDefintion());
			playerDefinition.Definitions.Add(new EntityInteractableDefinition());
			playerDefinition.Definitions.Add(new TransformDefinition(new Vector2(11, 8),map.StartArea,0.5f));
			playerDefinition.Definitions.Add(new WieldingDefinition());
			playerDefinition.Definitions.Add(new FacingDefinition());

			var createDefinition = new CreateDefinition();
			createDefinition.OnCreateEvents.Add(new ApplyEffectEvent(new CreateEntityEffect("Bow",true)){ApplyTo = ApplicableTo.Self});

			playerDefinition.Definitions.Add(createDefinition);
			map.GlobalEntityDefinitions.Add("Player",playerDefinition);
            {
                //Bogen
                EntityDefinition bowDefinition = new EntityDefinition("Bow", map.GetNextDefinitionId());
                var wieldable = new WieldableDefinition();
                wieldable.OnUseEvents.Add(new ApplyEffectOnUseEvent(new SpawnProjectileEffect()) { CoolDown = TimeSpan.FromSeconds(1)});
                bowDefinition.Definitions.Add(wieldable);
                bowDefinition.Definitions.Add(new CharacterRenderDefinition(new Index2(52, 0)));
                bowDefinition.Definitions.Add(new WieldedDefinition(0.5f, 0.5f));
                bowDefinition.Definitions.Add(new FacingDefinition());
                bowDefinition.Definitions.Add(new TransformDefinition(new Vector2(0.3f, 0.3f), map.StartArea));

                map.GlobalEntityDefinitions.Add("Bow",bowDefinition);
            }


            {
                // Teleporter
                //Area 1
                CreateTeleport(map,5,10,1,11,10,1);

                CreateTeleport(map,19,8,1,1,8,2);
                CreateTeleport(map,19,9,1,1,9,2);
            }

            MapLoader.Save (map, string.Format ("{0}.mm", map.Name));
        }

        private static void CreateTeleport(Map map,float x, float y, int area,int targeX,int targeY,int targetArea)
        {
            var id = map.GetNextDefinitionId();
            //TestTeleporter
            EntityDefinition testTeleporter = new EntityDefinition($"Teleport_{id}", id);

            var entityCollider = new EntityColliderDefinition();
            entityCollider.OnCollisionEvents.Add(
                new ApplyEffectEvent(new TeleportEffect(targeX, targeY, targetArea)) {ApplyTo = ApplicableTo.Other});
            //entityCollider.OnCollisionEvents.Add(new ApplyEffectEvent(new DamageEffect(10)) {ApplyTo = ApplicableTo.Other});

            testTeleporter.Definitions.Add(entityCollider);
            testTeleporter.Definitions.Add(new CharacterRenderDefinition());
            testTeleporter.Definitions.Add(new TransformDefinition(new Vector2(x, y), area, 0.4f));

            map.Areas[area-1].Entities.Add(testTeleporter);
        }
    }
}