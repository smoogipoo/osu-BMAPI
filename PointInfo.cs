using System;

namespace BMAPI
{
    public class PointInfo
    {
        public double X { get; set; }
        public double Y { get; set; }
        public PointInfo()
        {
            X = Y = 0;
        }
        public PointInfo(double V)
        {
            X = Y = V;
        }
        public PointInfo(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public double Length
        {
            get
            {
                return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));                
            }
        }
        public void Normalize()
        {
            double length = Length; //Cache for minor performance improvement
            X = X / length;
            Y = Y / length;
        }
        public double DistanceTo(PointInfo P)
        {
            return Math.Sqrt(Math.Pow(P.X - X, 2) + Math.Pow(P.Y - Y, 2));
        }
        public static PointInfo operator -(double Left, PointInfo Right)
        {
            return new PointInfo(Left - Right.X, Left - Right.Y);
        }
        public static PointInfo operator -(PointInfo Left, double Right)
        {
            return new PointInfo(Left.X - Right, Left.Y - Right);
        }
        public static PointInfo operator -(PointInfo Left, PointInfo Right)
        {
            return new PointInfo(Left.X - Right.X, Left.Y - Right.Y);
        }
        public static PointInfo operator +(double Left, PointInfo Right)
        {
            return new PointInfo(Right.X + Left, Right.Y + Left);
        }
        public static PointInfo operator +(PointInfo Left, double Right)
        {
            return new PointInfo(Left.X + Right, Left.Y + Right);
        }
        public static PointInfo operator +(PointInfo Left, PointInfo Right)
        {
            return new PointInfo(Left.X + Right.X, Left.Y + Right.Y);
        }
        public static PointInfo operator *(double Left, PointInfo Right)
        {
            return new PointInfo(Right.X * Left, Right.Y * Left);
        }
        public static PointInfo operator *(PointInfo Left, double Right)
        {
            return new PointInfo(Left.X * Right, Left.Y * Right);
        }
        public static PointInfo operator *(PointInfo Left, PointInfo Right)
        {
            return new PointInfo(Left.X * Right.X, Left.Y * Right.Y);
        }
        public static PointInfo operator /(double Left, PointInfo Right)
        {
            return new PointInfo(Left / Right.X, Left / Right.Y);
        }
        public static PointInfo operator /(PointInfo Left, double Right)
        {
            return new PointInfo(Left.X / Right, Left.Y / Right);
        }
        public static PointInfo operator /(PointInfo Left, PointInfo Right)
        {
            return new PointInfo(Left.X / Right.X, Left.Y / Right.Y);
        }
    }
}
