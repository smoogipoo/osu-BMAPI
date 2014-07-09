
namespace BMAPI.v1
{
    public class TimingPoint
    {
        public float Time { get; set; }
        public float BpmDelay { get; set; }
        public int TimeSignature = 4;
        public int SampleSet = 0;
        public int CustomSampleSet = 0;
        public int VolumePercentage = 100;
        public bool InheritsBPM = false;
        public TimingPointOptions VisualOptions;
    }
}
