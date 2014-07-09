namespace BMAPI
{
    public class Info_Combo
    {
        public Info_Combo() { }
        public Info_Combo(Helper_Colour baseInstance)
        {
            Colour.R = baseInstance.R;
            Colour.G = baseInstance.G;
            Colour.B = baseInstance.B;
        }

        public Helper_Colour Colour = new Helper_Colour();
        public int ComboNumber = 0;
    }
}
