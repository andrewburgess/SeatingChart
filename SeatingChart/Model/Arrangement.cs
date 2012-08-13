using System.Collections.Generic;

namespace SeatingChart.Model
{
    public class Arrangement
    {
        public int Score { get; set; }
        public List<Table> Tables { get; set; }

        public Arrangement()
        {
            Tables = new List<Table>();
        }
    }
}