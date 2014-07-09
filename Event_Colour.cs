
namespace BMAPI
{
    class Event_Colour : Event_Base
    {
        public Event_Colour() { }
        public Event_Colour(Event_Base baseInstance) : base(baseInstance) { }

        public Helper_Colour Colour { get; set; }
    }
}
