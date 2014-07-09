namespace BMAPI
{
    public class Event_Video : Event_Base
    {
        public Event_Video() { }
        public Event_Video(Event_Base baseInstance) : base(baseInstance) { }

        public string Filename { get; set; }
    }
}
