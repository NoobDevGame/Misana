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
			maps.Add ("Dorf_Markhausen");

			var paths = maps.Select (i => Path.Combine("MisanaMap","Maps",string.Format("{0}.json",i))).ToArray();

			var map = MapLoader.CreateMapFromTiled ("DebugMap", paths);

			//Definitionen
			EntityDefinition playerDefinition = new EntityDefinition("Player",map.GetNextDefinitionId());
			playerDefinition.Definitions.Add(new HealthDefinition());
			playerDefinition.Definitions.Add(new CharacterRenderDefinition(new Index2(1,9)));
			playerDefinition.Definitions.Add(new MotionComponentDefinition());
			//playerDefinition.Definitions.Add(new EntityColliderDefinition());
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

			EntityDefinition bowDefinition = new EntityDefinition("Bow",map.GetNextDefinitionId());
			var wieldable = new WieldableDefinition();
			wieldable.OnUseEvents.Add(new ApplyEffectOnUseEvent(new SpawnProjectileEffect() ));
			bowDefinition.Definitions.Add(wieldable);
			bowDefinition.Definitions.Add(new CharacterRenderDefinition(new Index2(52,0)));
			bowDefinition.Definitions.Add(new WieldedDefinition(0.5f,0.5f));
			bowDefinition.Definitions.Add(new FacingDefinition());
			bowDefinition.Definitions.Add(new TransformDefinition(new Vector2(0.3f,0.3f),map.StartArea));

			map.GlobalEntityDefinitions.Add("Bow",bowDefinition);


			MapLoader.Save (map, string.Format ("{0}.mm", map.Name));
        }
    }
}