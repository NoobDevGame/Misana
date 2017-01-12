namespace Misana.Core
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

        public static bool operator ==(Index2 obj1, Index2 obj2)
        {
            return obj1.X == obj2.X && obj1.Y == obj2.Y;
        }

        public static bool operator !=(Index2 obj1, Index2 obj2)
        {
            return !(obj1.X == obj2.X && obj1.Y == obj2.Y);
        }
    }
}