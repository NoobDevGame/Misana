namespace Misana.Core.Map
{
    public class Layer
    {
        public int Id { get; }
        public int[] Tiles { get; }

        public Layer(int id,int[] tiles)
        {
            Id = id;
            Tiles = tiles;
        }
    }
}