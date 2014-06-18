namespace BMAPI
{
    public class SpinnerInfo : BaseCircle
    {
        public int EndTime { get; set; }

        public override bool ContainsPoint(PointInfo Point)
        {
            return (Point.X >= 0 && Point.X <= 512 && Point.Y >= 0 && Point.Y <= 384);
        }
    }
}
