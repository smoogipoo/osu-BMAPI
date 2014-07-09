
namespace BMAPI.v1.Events
{
    public class ContentEvent : EventBase
    {
        public ContentEvent() { }
        public ContentEvent(EventBase baseInstance) : base(baseInstance) { }

        public string Filename { get; set; }
    }
}
