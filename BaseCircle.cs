using System;
namespace BMAPI
{
    public class BaseCircle
    {
        public BaseCircle()
        {
            effect = EffectType.None;
            newCombo = true;
        }
        public PointInfo location = new PointInfo(0, 0);
        public double radius = 80;
        public int startTime { get; set; }
        public bool newCombo { get; set; }
        public EffectType effect { get; set; }

        public bool ContainsPoint(PointInfo Point)
        {
            return Math.Sqrt(Math.Pow(Point.x - location.x, 2) + Math.Pow(Point.y - location.y, 2)) <= radius;
        }
    }
}
