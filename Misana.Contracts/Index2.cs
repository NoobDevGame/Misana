namespace Misana.Contracts
{
    public struct Index2
    {
        public int X;
        public int Y;

        public Index2(int x , int y)
        {
            X = x;
            Y = y;
        }

        //TODO: Runden ??
        public Index2(Vector2 vec)
        {
            X = (int)vec.X;
            Y = (int)vec.Y;
        }
    }
}