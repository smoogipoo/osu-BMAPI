using System;

namespace BMAPI
{
    public class CircleInfo
    {
        public PointInfo location = new PointInfo();
        public int radius = 128;
        public int startTime = 0;
        public bool newCombo = true;
        public EffectType effect = EffectType.None;

        public bool inCircle(PointInfo cursorLocation)
        {
            return Math.Pow(cursorLocation.x - location.x, 2) + Math.Pow(cursorLocation.y - location.y, 2) < radius;
        }
    }
}
