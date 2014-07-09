namespace BMAPI
{
    public class Event_Break : Event_Base
    {
        public Event_Break() { }
        public Event_Break(Event_Base baseInstance) : base(baseInstance) { }

        public int EndTime { get; set; }
    }
}
