namespace BMAPI
{
    public class Helper_Colour
    {
        public Helper_Colour() { }
        public Helper_Colour(Helper_Colour baseInstance)
        {
            R = baseInstance.R;
            G = baseInstance.G;
            B = baseInstance.B;
        }

        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
    }
}
