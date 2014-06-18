
namespace BMAPI
{
    public class TimingPointInfo
    {
        public double Time { get; set; }
        public double BpmDelay { get; set; }
        public int TimeSignature = 4;
        public int SampleSet = 0;
        public int CustomSampleSet = 0;
        public int VolumePercentage = 100;
        public bool InheritsBPM = false;
        public bool KiaiTime = false;
        public bool OmitFirstBarLine = false;
    }
}
