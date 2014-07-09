namespace BMAPI
{
    public class Event_Base
    {
        public Event_Base() { }
        public Event_Base(Event_Base baseInstance)
        {
            StartTime = baseInstance.StartTime;
        }

        public int StartTime { get; set; }
    }
}
