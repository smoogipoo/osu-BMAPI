using System;
using System.Collections.Generic;
using System.Linq;

namespace BMAPI.v1.HitObjects
{
    public class SliderObject : CircleObject
    {
        public SliderObject() { }
        public SliderObject(CircleObject baseInstance) : base(baseInstance) { }

        public new SliderType Type = SliderType.Linear;
        public List<Point2> Points = new List<Point2>();
        public int RepeatCount { get; set; }
        public float Velocity { get; set; }
        public float MaxPoints { get; set; }

        private float? S_Length { get; set; }
        private float? S_SegmentLength { get; set; }
        public float Length
        {
            get
            {
                if (S_Length != null) return (float)S_Length;
                switch (Type)
                {
                    case SliderType.Linear:
                        S_Length = (float)Math.Sqrt(Math.Pow(Points[1].X - Points[0].X, 2) + Math.Pow(Points[1].Y - Points[0].Y, 2)) * RepeatCount;
                        break;
                    case SliderType.CSpline:
                    case SliderType.PSpline:
                        Spline spl = new Spline(Points);
                        S_Length = spl.Length() * RepeatCount;
                        break;
                    case SliderType.Bezier:
                        S_Length = BezierLength(Points) * RepeatCount;
                        break;
                    default:
                        S_Length = 0;
                        break;
                }
                return (float)S_Length;
            }
        }
        public float SegmentLength
        {
            get
            {
                if (S_SegmentLength != null) return (float)S_SegmentLength;
                switch (Type)
                {
                    case SliderType.Linear:
                        S_SegmentLength = (float)Math.Sqrt(Math.Pow(Points[1].X - Points[0].X, 2) + Math.Pow(Points[1].Y - Points[0].Y, 2));
                        break;
                    case SliderType.CSpline:
                    case SliderType.PSpline:
                        Spline spl = new Spline(Points);
                        S_SegmentLength = spl.Length();
                        break;
                    case SliderType.Bezier:
                        S_SegmentLength = BezierLength(Points);
                        break;
                    default:
                        S_SegmentLength = 0;
                        break;
                }
                return (float)S_SegmentLength;
            }
        }
        public float SegmentEndTime(int SegmentNumber)
        {
            switch (Type)
            {
                case SliderType.Linear:
                    return StartTime + (SegmentLength * SegmentNumber) / Velocity;
                case SliderType.CSpline:
                case SliderType.PSpline:
                    return StartTime + (SegmentLength * SegmentNumber) / Velocity;
                case SliderType.Bezier:
                    return StartTime + (SegmentLength * SegmentNumber) / Velocity;
                default:
                    return 0;
            }
        }
        public Point2 PositionAtTime(int Time)
        {
            float T = (SegmentEndTime(1) - StartTime) / Time;
            switch (Type)
            {
                case SliderType.Linear:
                    return LinInterpolate(Points[0], Points[1], T);
                case SliderType.CSpline:
                case SliderType.PSpline:
                    Spline spl = new Spline(Points);
                    return SplInterpolate(spl, T);
                case SliderType.Bezier:
                    return BezInterpolate(Points, T);
                default:
                    return new Point2();
            }
        }

        #region Linear Interpolation
        public Point2 LinInterpolate(Point2 P1, Point2 P2, float T)
        {
            return new Point2((1 - T) * P1.X + T * P2.X, (1 - T) * P1.Y + T * P2.Y);
        }
        #endregion

        #region Spline Interpolation
        public class Spline : List<SplineFunction>
        {
            public Spline(IList<Point2> Points)
            {
                List<float> Times = new List<float>();
                for (float i = 0; i <= 1; i += 1f / Points.Count)
                {
                    Times.Add(i);
                }
                int n = Points.Count - 1;

                Point2[] b = new Point2[n];
                Point2[] d = new Point2[n];
                Point2[] a = new Point2[n];
                Point2[] c = new Point2[n + 1];
                Point2[] l = new Point2[n + 1];
                Point2[] u = new Point2[n + 1];
                Point2[] z = new Point2[n + 1];
                float[] h = new float[n + 1];

                l[0] = new Point2(1);
                u[0] = new Point2(0);
                z[0] = new Point2(0);
                h[0] = Times[1] - Times[0];

                for (int i = 1; i < n; i++)
                {
                    h[i] = Times[i + 1] - Times[i];
                    l[i] = 2 * (Times[i + 1] - Times[i - 1]) - (h[i - 1] * u[i - 1]);
                    u[i] = h[i] / l[i];
                    a[i] = (3 / h[i]) * (Points[i + 1] - Points[i]) - ((3 / h[i - 1]) * (Points[i] - Points[i - 1]));
                    z[i] = (a[i] - (h[i - 1] * z[i - 1])) / l[i];
                }
                l[n] = new Point2(1);
                z[n] = c[n] = new Point2(0);

                for (int j = n - 1; j >= 0; j--)
                {
                    c[j] = z[j] - (u[j] * c[j + 1]);
                    b[j] = (Points[j + 1] - Points[j]) / h[j] - (h[j] * (c[j + 1] + 2 * c[j])) / 3;
                    d[j] = (c[j + 1] - c[j]) / (3 * h[j]);
                }

                for (int i = 0; i < n; i++)
                {
                    Add(new SplineFunction(Times[i], Points[i], b[i], c[i], d[i]));
                }
            }

            private Point2 Interpolate(float T)
            {
                if (Count == 0) return new Point2();

                Sort();
                SplineFunction it = this.LastOrDefault(sf => sf.T <= T);
                return it.Eval(T);
            }


            public float Length(float prec = 0.01f)
            {
                float ret = 0;
                for (float f = 0; f < 1f; f += prec)
                {
                    Point2 a = Interpolate(f);
                    Point2 b = Interpolate(f + prec);
                    ret += (float)Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
                }
                return ret;
            }
        }
        public class SplineFunction : IComparable
        {
            internal Point2 _a, _b, _c, _d;
            internal float T { get; set; }

            public SplineFunction(float x)
            {
                T = x;
            }
            public SplineFunction(float x, Point2 a, Point2 b, Point2 c, Point2 d)
            {
                _a = a;
                _b = b;
                _c = c;
                _d = d;
                T = x;
            }
            public int CompareTo(object Obj)
            {
                return Obj == null ? 1 : T.CompareTo(((SplineFunction)Obj).T);
            }

            public Point2 Eval(float x)
            {
                float xix = x - T;
                return _a + _b * xix + _c * (xix * xix) + _d * (xix * xix * xix);
            }
        }
        public Point2 SplInterpolate(Spline Spline, float T)
        {
            if (Spline.Count == 0) return new Point2();

            Spline.Sort();
            SplineFunction it = Spline.LastOrDefault(sf => sf.T <= T);
            return it.Eval(T);
        }
        #endregion

        #region Bezier Interpolation
        public Point2 BezInterpolate(List<Point2> Pts, float T)
        {
            //This can be done recursively, current method is probably ineffective
            int n = Pts.Count - 1;
            Point2[] points = new Point2[Pts.Count];

            for (int i = 0; i <= n; i++)
            {
                points[i] = Pts[i];
            }
            for (int k = 1; k <= n; k++)
            {
                for (int i = 0; i <= n - k; i++)
                {
                    points[i] = (1 - T) * points[i] + T * points[i + 1];
                }
            }
            return points[0];
        }

        public float BezierLength(List<Point2> Pts, float prec = 0.01f)
        {
            float ret = 0;
            for (float f = 0; f < 1f; f += prec)
            {
                Point2 a = BezInterpolate(Pts, f);
                Point2 b = BezInterpolate(Pts, f + prec);
                ret += (float)Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
            }
            return ret;
        }
        #endregion




        public override bool ContainsPoint(Point2 Point)
        {
            return ContainsPoint(Point, 0);
        }
        public bool ContainsPoint(Point2 Point, int Time)
        {
            Point2 pAtTime = PositionAtTime(Time);
            return Math.Sqrt(Math.Pow(Point.X - pAtTime.X, 2) + Math.Pow(Point.Y - pAtTime.Y, 2)) <= Radius;            
        }
    }
}
