using System;
using System.Collections.Generic;
using System.Linq;

namespace BMAPI
{
    public class SliderInfo : BaseCircle
    {
        public SliderInfo()
        {
            Type = SliderType.Linear;
        }
        public SliderType Type { get; set; }
        public List<PointInfo> Points = new List<PointInfo>();
        public int RepeatCount { get; set; }
        public double Velocity { get; set; }
        public double MaxPoints { get; set; }

        private double? S_Length { get; set; }
        private double? S_SegmentLength { get; set; }
        public double Length
        {
            get
            {
                if (S_Length != null) return (double)S_Length;
                switch (Type)
                {
                    case SliderType.Linear:
                        S_Length = Math.Sqrt(Math.Pow(Points[1].X - Points[0].X, 2) + Math.Pow(Points[1].Y - Points[0].Y, 2)) * RepeatCount;
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
                return (double)S_Length;
            }
        }
        public double SegmentLength
        {
            get
            {
                if (S_SegmentLength != null) return (double)S_SegmentLength;
                switch (Type)
                {
                    case SliderType.Linear:
                        S_SegmentLength = Math.Sqrt(Math.Pow(Points[1].X - Points[0].X, 2) + Math.Pow(Points[1].Y - Points[0].Y, 2));
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
                return (double)S_SegmentLength;
            }
        }
        public double SegmentEndTime(int SegmentNumber)
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
        public PointInfo PositionAtTime(int Time)
        {
            double T = (SegmentEndTime(1) - StartTime) / Time;
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
                    return new PointInfo();
            }
        }

        public PointInfo LinInterpolate(PointInfo P1, PointInfo P2, double T)
        {
            return new PointInfo((1 - T) * P1.X + T * P2.X, (1 - T) * P1.Y + T * P2.Y);
        }

        public PointInfo SplInterpolate(Spline Spline, double T)
        {
            if (Spline.Count == 0) return new PointInfo();

            Spline.Sort();
            SplineFunction it = Spline.LastOrDefault(sf => sf.T <= T);
            return it.Eval(T);
        }

        public PointInfo BezInterpolate(List<PointInfo> Pts, double T)
        {
            //This can be done recursively, current method is probably ineffective
            int n = Pts.Count - 1;
            PointInfo[] points = new PointInfo[Pts.Count];

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

        public double BezierLength(List<PointInfo> Pts, double prec = 0.01)
        {
            double ret = 0;
            for (double f = 0; f < 1d; f += prec)
            {
                PointInfo a = BezInterpolate(Pts, f);
                PointInfo b = BezInterpolate(Pts, f + prec);
                ret += Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
            }
            return ret;
        }


        public override bool ContainsPoint(PointInfo Point)
        {
            return ContainsPoint(Point, 0);
        }
        public bool ContainsPoint(PointInfo Point, int Time)
        {
            PointInfo pAtTime = PositionAtTime(Time);
            return Math.Sqrt(Math.Pow(Point.X - pAtTime.X, 2) + Math.Pow(Point.Y - pAtTime.Y, 2)) <= Radius;            
        }
    }


    public class Spline : List<SplineFunction>
    {
        public Spline(IList<PointInfo> Points)
        {
            List<double> Times = new List<double>();
            for (double i = 0; i <= 1; i += 1d / (Points.Count - 1))
            {
                Times.Add(i);
            }
            int n = Points.Count - 1;

            PointInfo[] b = new PointInfo[n];
            PointInfo[] d = new PointInfo[n];
            PointInfo[] a = new PointInfo[n];
            PointInfo[] c = new PointInfo[n + 1];
            PointInfo[] l = new PointInfo[n + 1];
            PointInfo[] u = new PointInfo[n + 1];
            PointInfo[] z = new PointInfo[n + 1];
            double[] h = new double[n + 1];

            l[0] = new PointInfo(1);
            u[0] = new PointInfo(0);
            z[0] = new PointInfo(0);
            h[0] = Times[1] - Times[0];

            for (int i = 1; i < n; i++)
            {
                h[i] = Times[i + 1] - Times[i];
                l[i] = 2 * (Times[i + 1] - Times[i - 1]) - (h[i - 1] * u[i - 1]);
                u[i] = h[i] / l[i];
                a[i] = (3 / h[i]) * (Points[i + 1] - Points[i]) - ((3 / h[i - 1]) * (Points[i] - Points[i - 1]));
                z[i] = (a[i] - (h[i - 1] * z[i - 1])) / l[i];
            }
            l[n] = new PointInfo(1);
            z[n] = c[n] = new PointInfo(0);

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

        private PointInfo Interpolate(double T)
        {
            if (Count == 0) return new PointInfo();

            Sort();
            SplineFunction it = this.LastOrDefault(sf => sf.T <= T);
            return it.Eval(T);
        }


        public double Length(double prec = 0.01)
        {
            double ret = 0;
            for (double f = 0; f < 1d; f += prec)
            {
                PointInfo a = Interpolate(f);
                PointInfo b = Interpolate(f + prec);
                ret += Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
            }
            return ret;
        }
    }
    public class SplineFunction : IComparable
    {
        internal PointInfo _a, _b, _c, _d;
        internal double T { get; set; }

        public SplineFunction(double x)
        {
            T = x;
        }
        public SplineFunction(double x, PointInfo a, PointInfo b, PointInfo c, PointInfo d)
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

        public PointInfo Eval(double x)
        {
            double xix = x - T;
            return _a + _b * xix + _c * (xix * xix) + _d * (xix * xix * xix);
        }
    }
}
