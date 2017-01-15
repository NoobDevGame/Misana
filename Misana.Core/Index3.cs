using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misana.Core
{
    public struct Index3
    {
        public int X;
        public int Y;
        public int Z;

        public Index3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        //TODO: Runden ??
        public Index3(Vector2 vec, int z)
        {
            X = (int)vec.X;
            Y = (int)vec.Y;
            Z = z;
        }

        public static bool operator ==(Index3 obj1, Index3 obj2)
        {
            return obj1.X == obj2.X && obj1.Y == obj2.Y && obj1.Z == obj2.Z;
        }

        public static bool operator !=(Index3 obj1, Index3 obj2)
        {
            return !(obj1.X == obj2.X && obj1.Y == obj2.Y && obj1.Z == obj2.Z);
        }

        public Vector2 ToVector2()
        {
            return new Vector2(this.X, this.Y);
        }
    }
}