using System;

namespace BMAPI
{
    public class Helper_Point2
    {
        public float X { get; set; }
        public float Y { get; set; }
        public Helper_Point2()
        {
            X = Y = 0;
        }
        public Helper_Point2(float V)
        {
            X = Y = V;
        }
        public Helper_Point2(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public float Length
        {
            get
            {
                return (float)Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));                
            }
        }
        public void Normalize()
        {
            float length = Length; //Cache for minor performance improvement
            X = X / length;
            Y = Y / length;
        }
        public float DistanceTo(Helper_Point2 P)
        {
            return (float)Math.Sqrt(Math.Pow(P.X - X, 2) + Math.Pow(P.Y - Y, 2));
        }
        public static Helper_Point2 operator -(float Left, Helper_Point2 Right)
        {
            return new Helper_Point2(Left - Right.X, Left - Right.Y);
        }
        public static Helper_Point2 operator -(Helper_Point2 Left, float Right)
        {
            return new Helper_Point2(Left.X - Right, Left.Y - Right);
        }
        public static Helper_Point2 operator -(Helper_Point2 Left, Helper_Point2 Right)
        {
            return new Helper_Point2(Left.X - Right.X, Left.Y - Right.Y);
        }
        public static Helper_Point2 operator +(float Left, Helper_Point2 Right)
        {
            return new Helper_Point2(Right.X + Left, Right.Y + Left);
        }
        public static Helper_Point2 operator +(Helper_Point2 Left, float Right)
        {
            return new Helper_Point2(Left.X + Right, Left.Y + Right);
        }
        public static Helper_Point2 operator +(Helper_Point2 Left, Helper_Point2 Right)
        {
            return new Helper_Point2(Left.X + Right.X, Left.Y + Right.Y);
        }
        public static Helper_Point2 operator *(float Left, Helper_Point2 Right)
        {
            return new Helper_Point2(Right.X * Left, Right.Y * Left);
        }
        public static Helper_Point2 operator *(Helper_Point2 Left, float Right)
        {
            return new Helper_Point2(Left.X * Right, Left.Y * Right);
        }
        public static Helper_Point2 operator *(Helper_Point2 Left, Helper_Point2 Right)
        {
            return new Helper_Point2(Left.X * Right.X, Left.Y * Right.Y);
        }
        public static Helper_Point2 operator /(float Left, Helper_Point2 Right)
        {
            return new Helper_Point2(Left / Right.X, Left / Right.Y);
        }
        public static Helper_Point2 operator /(Helper_Point2 Left, float Right)
        {
            return new Helper_Point2(Left.X / Right, Left.Y / Right);
        }
        public static Helper_Point2 operator /(Helper_Point2 Left, Helper_Point2 Right)
        {
            return new Helper_Point2(Left.X / Right.X, Left.Y / Right.Y);
        }
        public static bool operator ==(Helper_Point2 Left, Helper_Point2 Right)
        {
            return (Left.X.CompareTo(Right.X) == 0 && Left.Y.CompareTo(Right.Y) == 0);
        }
        public static bool operator !=(Helper_Point2 Left, Helper_Point2 Right)
        {
            return (Left.X.CompareTo(Right.X) != 0 || Left.Y.CompareTo(Right.Y) != 0);
        }
    }
}
