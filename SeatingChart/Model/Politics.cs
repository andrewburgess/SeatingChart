namespace SeatingChart.Model
{
    public class Politics
    {
        public int Left { get; set; }

        public int Right { get; set; }

        public override string ToString()
        {
            return "Left: " + Left + ", Right: " + Right;
        }
    }
}