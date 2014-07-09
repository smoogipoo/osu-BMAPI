namespace BMAPI
{
    public class HitObject_Spinner : HitObject_Circle
    {
        public HitObject_Spinner() { }
        public HitObject_Spinner(HitObject_Circle baseInstance) : base(baseInstance) { }

        public int EndTime { get; set; }

        public override bool ContainsPoint(Helper_Point2 Point)
        {
            return (Point.X >= 0 && Point.X <= 512 && Point.Y >= 0 && Point.Y <= 384);
        }
    }
}
