using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMAPI
{
    public class SliderInfo
    {
        public PointInfo Location = new PointInfo();
        public int StartTime = 0;
        public bool NewCombo = true;
        public EffectType Effect = EffectType.None;
        public SliderType Type = SliderType.Linear;
        public List<object> Points = new List<object>();
        public int RepeatCount = 0;
        public int MaxPoints = 0;
    }
}
