using Misana.Contracts.Map;

namespace Misana.Core.Map
{
    public class Layer : ILayer
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