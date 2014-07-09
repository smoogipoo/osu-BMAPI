namespace BMAPI
{
    public class Event_Background : Event_Base
    {
        public Event_Background() { }
        public Event_Background(Event_Base baseInstance) : base(baseInstance) { }

        public string Filename { get; set; }
    }
}
