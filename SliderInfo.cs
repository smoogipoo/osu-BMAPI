using System.Collections.Generic;

namespace BMAPI
{
    public class SliderInfo
    {
        public PointInfo location = new PointInfo();
        public int startTime = 0;
        public bool newCombo = true;
        public EffectType effect = EffectType.None;
        public SliderType type = SliderType.Linear;
        public List<object> points = new List<object>();
        public int repeatCount = 0;
        public double maxPoints = 0;
    }
}
