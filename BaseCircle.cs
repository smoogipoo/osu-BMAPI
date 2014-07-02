using System;
namespace BMAPI
{
    public class BaseCircle
    {
        public BaseCircle()
        {
            Effect = EffectType.None;
        }
        public BaseCircle(BaseCircle baseInstance)
        {
            //Copy from baseInstance
            Location = baseInstance.Location;
            Radius = baseInstance.Radius;
            StartTime = baseInstance.StartTime;
            Type = baseInstance.Type;
            Effect = baseInstance.Effect;
        }

        public PointInfo Location = new PointInfo(0, 0);
        public double Radius = 80;
        public int StartTime { get; set; }
        public HitObjectType Type { get; set; }
        public EffectType Effect { get; set; }

        public virtual bool ContainsPoint(PointInfo Point)
        {
            return Math.Sqrt(Math.Pow(Point.X - Location.X, 2) + Math.Pow(Point.Y - Location.Y, 2)) <= Radius;
        }
    }
}
