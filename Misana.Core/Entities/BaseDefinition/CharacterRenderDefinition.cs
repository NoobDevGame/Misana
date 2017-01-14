using Misana.Core.Components;
using Misana.Core.Ecs;

namespace Misana.Core.Entities.BaseDefinition
{
    public class CharacterRenderDefinition : ComponentDefinition<CharacterRenderComponent>
    {
        public Index2 TilePosition { get; set; }

        public CharacterRenderDefinition()
        {
            TilePosition = new Index2(0,0);
        }

        public CharacterRenderDefinition(Index2 tilePosition)
        {
            TilePosition = tilePosition;
        }

        public override void OnApplyDefinition(Entity entity, CharacterRenderComponent component)
        {
            component.TilePosition = TilePosition;
        }
    }
}