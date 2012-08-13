using System.Collections.Generic;

namespace SeatingChart.Model
{
    public class Arrangement
    {
        public int Score { get; set; }
        public List<Table> Tables { get; set; }

        public Arrangement(int numberOfTables, int peoplePerTable)
        {
            Tables = new List<Table>();

            for (var i = 0; i < numberOfTables; i++)
            {
                Tables.Add(new Table(peoplePerTable));
            }
        }
    }
}