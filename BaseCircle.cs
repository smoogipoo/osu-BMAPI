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
        public PointInfo location = new PointInfo();
        public int radius = 128; //SoonTM
        public int startTime { get; set; }
        public bool newCombo { get; set; }
        public EffectType effect { get; set; }

        public bool inCircle(PointInfo cursorLocation)
        {
            return Math.Pow(cursorLocation.x - location.x, 2) + Math.Pow(cursorLocation.y - location.y, 2) < radius;
        }
    }
}
