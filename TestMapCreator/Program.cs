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
using Misana.Core.Components;
using Misana.Core.Ecs;
using Misana.Core.Entities.Events;
using Misana.Core.Events.Entities;

namespace TestMapCreator
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            List<string> maps = new List<string>();
            maps.Add("Markhausen.West"); //1
            maps.Add("Markhausen.Weg"); //2

            var paths = maps.Select(i => Path.Combine("MisanaMap", "Maps", $"{i}.json")).ToArray();

            var map = MapLoader.CreateMapFromTiled("DebugMap", paths);

            //Definitionen

            //Player
            EntityDefinition playerDefinition = new EntityDefinition("Player", map.GetNextDefinitionId());
            playerDefinition.Definitions.Add(new HealthDefinition());
            playerDefinition.Definitions.Add(new CharacterRenderDefinition(new Index2(1, 9)) { RunsOn = RunsOn.Client });
            playerDefinition.Definitions.Add(new MotionComponentDefinition());
            playerDefinition.Definitions.Add(new EntityColliderDefinition());
            playerDefinition.Definitions.Add(new BlockColliderDefinition());
            playerDefinition.Definitions.Add(new EntityFlagDefintion());
            playerDefinition.Definitions.Add(new EntityInteractableDefinition());
            playerDefinition.Definitions.Add(new TransformDefinition(new Vector2(11, 8), map.StartArea, 0.5f));
            playerDefinition.Definitions.Add(new WieldingDefinition() { WieldedDefinitionName = "Bow"});
            playerDefinition.Definitions.Add(new FacingDefinition());

            //var createDefinition = new CreateDefinition();
            //createDefinition.OnCreateEvents.Add(new ApplyEffectEvent(new CreateEntityEffect("Bow", true)) {ApplyTo = ApplicableTo.Self});

            //playerDefinition.Definitions.Add(createDefinition);
            map.GlobalEntityDefinitions.Add("Player", playerDefinition);
           

            var arrow = new EntityDefinition("Arrow", map.GetNextDefinitionId());
            arrow.Definitions.Add(new EntityColliderDefinition {
                    OnCollisionEvents = new List<OnEvent> {
                        new ApplyEffectEvent(new DamageEffect(2)) {
                            ApplyTo = ApplicableTo.Other,
                            RunsOn = RunsOn.Server
                        },
                        new ApplyEffectEvent(new RemoveSelfEffect()) { ApplyTo = ApplicableTo.Self }
                    }
                });
            arrow.Definitions.Add(new TransformDefinition { Radius = 0.2f, AreaId = -1});
            arrow.Definitions.Add(new MotionComponentDefinition());
            arrow.Definitions.Add(new ExpiringComponentDefinition(1500));
            arrow.Definitions.Add(new CharacterRenderDefinition(new Index2(37,0) ) { RunsOn = RunsOn.Client });
            map.GlobalEntityDefinitions.Add("Arrow", arrow);
            {
                //Bogen
                EntityDefinition bowDefinition = new EntityDefinition("Bow", map.GetNextDefinitionId());
                var wieldable = new WieldableDefinition(0.5f, 0.5f);
                //wieldable.OnUseEvents.Add(new ApplyEffectOnUseEvent(new SpawnProjectileEffect()) {CoolDown = TimeSpan.FromSeconds(1)});
                bowDefinition.Definitions.Add(wieldable);
                bowDefinition.Definitions.Add(new SpawnerDefinition
                {
                    Active = true,
                    CoolDown = 0.2f,
                    SpawnedDefinitionName = "Arrow",
                    //MaxAlive = 2,
                    Projectile = true,
                });
                bowDefinition.Definitions.Add(new CharacterRenderDefinition(new Index2(52, 0)) {
                    RunsOn = RunsOn.Client
                });
                bowDefinition.Definitions.Add(new FacingDefinition());
                bowDefinition.Definitions.Add(new TransformDefinition(new Vector2(0.3f, 0.3f), map.StartArea));

                map.GlobalEntityDefinitions.Add("Bow", bowDefinition);
            }

            var basicOrc = new EntityDefinition("orc1", map.GetNextDefinitionId());
            basicOrc.Definitions.Add(new HealthDefinition());
            basicOrc.Definitions.Add(new CharacterRenderDefinition(new Index2(1, 3)));
            basicOrc.Definitions.Add(new MotionComponentDefinition());
            basicOrc.Definitions.Add(new EntityColliderDefinition());
            basicOrc.Definitions.Add(new BlockColliderDefinition());
            basicOrc.Definitions.Add(new EntityFlagDefintion());
            basicOrc.Definitions.Add(new EntityInteractableDefinition());
            basicOrc.Definitions.Add(new TransformDefinition {AreaId = -1, Radius = 0.5f});
            basicOrc.Definitions.Add(new WieldingDefinition());
            basicOrc.Definitions.Add(new FacingDefinition());
            basicOrc.Definitions.Add(new CharacterDefinition("ORC"));

            map.GlobalEntityDefinitions.Add("orc1", basicOrc);

            var area1 = map.Areas[0];
            var area2 = map.Areas[1];


            // Teleporter
            //Area 1
            area1.Entities.Add(CreateTeleport(map, 5, 10, area1, 11, 10, area1));

            area1.Entities.Add(CreateTeleport(map, 19, 8, area1, 1, 8, area2));
            area1.Entities.Add(CreateTeleport(map, 19, 9, area1, 1, 9, area2));
            
            area1.Entities.Add(CreateSpawner(map, 15, 6, area1, new SpawnerDefinition {
                Active = true,
                CoolDown = 5,
                SpawnedDefinitionName = "orc1",
                SpawnDirection = new Vector2(-1,1),
                Projectile = true,
                RunsOn = RunsOn.Server
            }));

            MapLoader.Save(map, $"{map.Name}.mm");
        }

        private static EntityDefinition CreateSpawner(Map map, int x, int y, Area area, SpawnerDefinition sdef)
        {
            var id = map.GetNextDefinitionId();
            var def = new EntityDefinition($"Spawner_{id}", id);
            def.Definitions.Add(new CharacterRenderDefinition() { RunsOn = RunsOn.Client });
            def.Definitions.Add(new TransformDefinition(new Vector2(x, y), area, 0.4f));
            def.Definitions.Add(sdef);
            return def;
        }

        private static EntityDefinition CreateTeleport(Map map,float x, float y, Area area,int targeX,int targeY,Area targetArea)
        {
            var id = map.GetNextDefinitionId();
            //TestTeleporter
            EntityDefinition testTeleporter = new EntityDefinition($"Teleport_{id}", id);

            var entityCollider = new EntityColliderDefinition();
            entityCollider.OnCollisionEvents.Add(
                new ApplyEffectEvent(new TeleportEffect(targeX, targeY, targetArea.Id)) {ApplyTo = ApplicableTo.Other, RunsOn = RunsOn.Server});
            //entityCollider.OnCollisionEvents.Add(new ApplyEffectEvent(new DamageEffect(10)) {ApplyTo = ApplicableTo.Other});

            testTeleporter.Definitions.Add(entityCollider);
            testTeleporter.Definitions.Add(new CharacterRenderDefinition() { RunsOn = RunsOn.Client });
            testTeleporter.Definitions.Add(new TransformDefinition(new Vector2(x, y), area, 0.4f));

            return testTeleporter;
        }
    }
}