using System.Collections.Generic;

namespace BMAPI
{
    public class SliderInfo : BaseCircle
    {
        public SliderInfo()
        {
            type = SliderType.Linear;
        }
        public SliderType type { get; set; }
        public List<PointInfo> points = new List<PointInfo>();
        public int repeatCount { get; set; }
        public double maxPoints { get; set; }
    }
}
