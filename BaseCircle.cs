using System;
namespace BMAPI
{
    public class BaseCircle
    {
        public BaseCircle()
        {
            Effect = EffectType.None;
            NewCombo = true;
        }
        public PointInfo Location = new PointInfo(0, 0);
        public double Radius = 80;
        public int StartTime { get; set; }
        public bool NewCombo { get; set; }
        public EffectType Effect { get; set; }

        public virtual bool ContainsPoint(PointInfo Point)
        {
            return Math.Sqrt(Math.Pow(Point.X - Location.X, 2) + Math.Pow(Point.Y - Location.Y, 2)) <= Radius;
        }
    }
}
