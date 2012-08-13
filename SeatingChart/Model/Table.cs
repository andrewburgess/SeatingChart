using System.Collections.Generic;

namespace SeatingChart.Model
{
    public class Table
    {
        public List<Person> People { get; set; }
        public int Score { get; set; }

        public Table(int peoplePerTable)
        {
            People = new List<Person>(peoplePerTable);

            for (var i = 0; i < peoplePerTable; i++)
                People.Add(null);
        }
    }
}