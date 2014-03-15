
namespace BMAPI
{
    public class TimingPointInfo
    {
        public double time = 0.0f;
        public double bpmDelay = 0.0f;
        public int timeSignature = 4;
        public int sampleSet = 0;
        public int customSampleSet = 0;
        public int volumePercentage = 100;
        public bool inheritsBPM = true;
        public bool kiaiTime = false;
        public bool omitFirstBarLine = false;
    }
}
