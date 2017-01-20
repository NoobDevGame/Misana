using System;

namespace Misana.Core
{
    public struct Vector2
    {
        public float X;
        public float Y;

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vector2 Zero  = new Vector2(0,0);
        public static Vector2 One  = new Vector2(1,1);

        public Vector2 Normalize()
        {
            return new Vector2(X, Y) / Length();
        }

        public float LengthSquared()
        {
            return X * X + Y * Y;
        }


        public float Length()
        {
            return (float)Math.Sqrt(LengthSquared());
        }

        public static Vector2 operator +(Vector2 vec1, Vector2 vec2)
        {
            return new Vector2(vec1.X+ vec2.X,vec1.Y + vec2.Y);
        }

        public static Vector2 operator -(Vector2 vec1, Vector2 vec2)
        {
            return new Vector2(vec1.X- vec2.X,vec1.Y - vec2.Y);
        }

        public static Vector2 operator * (Vector2 vec1, Vector2 vec2)
        {
            return new Vector2(vec1.X * vec2.X,vec1.Y *  vec2.Y);
        }

        public static Vector2 operator * (Vector2 vec1, float value)
        {
            return new Vector2(vec1.X * value,vec1.Y *  value);
        }
        
        public static Vector2 operator * (Vector2 vec1, double value)
        {
            return new Vector2(vec1.X * (float)value,vec1.Y *  (float)value);
        }

        public static Vector2 operator /(Vector2 vec1, float value)
        {
            return new Vector2(vec1.X / value, vec1.Y / value);
        }

        public static Vector2 operator /(Vector2 vec1, double value)
        {
            return new Vector2(vec1.X / (float)value, vec1.Y / (float)value);
        }

        public static bool operator ==(Vector2 obj1, Vector2 obj2)
        {
            return obj1.X == obj2.X && obj1.Y == obj2.Y;
        }

        public static bool operator !=(Vector2 obj1, Vector2 obj2)
        {
            return !(obj1.X == obj2.X && obj1.Y == obj2.Y);
        }

        public static float Distance(Vector2 position1, Vector2 position2)
        {
            return (position1 - position2).Length();
        }


    }
}