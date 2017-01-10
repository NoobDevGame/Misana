namespace Misana.Contracts.Map
{
    public interface ILayer
    {
        int Id { get; }
        int[] Tiles { get; }
    }
}
