using Misana.Core;
using Misana.Core.Maps;
using Redbus.Events;

namespace Misana.Editor.Events
{
    public class MapTileSelectionEvent : EventBase
    {
        public bool IsSingleTile {
            get
            {
                return SelectedTilesIndices.Length == 1;
            }
        }

        public Index2[] SelectedTilesIndices { get; set; }
        public int SelectedLayer { get; set; }
             
        public Tile[] SelectedTiles { get; set; }

        public MapTileSelectionEvent(int selectedLayer, Tile[] selectedTiles, Index2[] selectedTileIndices)
        {
            SelectedLayer = selectedLayer;
            SelectedTilesIndices = selectedTileIndices;
            SelectedTiles = selectedTiles;
        }
    }
}
