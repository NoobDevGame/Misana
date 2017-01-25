namespace Misana.Core.Maps
{
    public class Layer
    {
        public int Id { get; }
        public Tile[] Tiles { get; }

        public string Name { get; set; }

        public Layer(int id,Tile[] tiles)
        {
            Id = id;
            Tiles = tiles;
            Name = "Layer " + id;
        }

        public Layer(int id, string name, Tile[] tiles)
        {
            Id = id;
            Tiles = tiles;
            Name = name;
        }


    }
}