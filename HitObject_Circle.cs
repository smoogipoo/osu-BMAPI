using System;
namespace BMAPI
{
    public class HitObject_Circle
    {
        public HitObject_Circle() { }
        public HitObject_Circle(HitObject_Circle baseInstance)
        {
            //Copy from baseInstance
            Location = baseInstance.Location;
            Radius = baseInstance.Radius;
            StartTime = baseInstance.StartTime;
            Type = baseInstance.Type;
            Effect = baseInstance.Effect;
        }

        public Helper_Point2 Location = new Helper_Point2(0, 0);
        public float Radius = 80;
        public int StartTime { get; set; }
        public HitObjectType Type { get; set; }
        public EffectType Effect = EffectType.None;

        public virtual bool ContainsPoint(Helper_Point2 Point)
        {
            return Math.Sqrt(Math.Pow(Point.X - Location.X, 2) + Math.Pow(Point.Y - Location.Y, 2)) <= Radius;
        }
    }
}
