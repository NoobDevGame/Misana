namespace Misana.Core.Maps
{
    public class Layer
    {
        public int Id { get; }
        public Tile[] Tiles { get; }

        public Layer(int id,Tile[] tiles)
        {
            Id = id;
            Tiles = tiles;
        }
    }
}