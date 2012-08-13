using System.Collections.Generic;
using System.Linq;

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

        public override string ToString()
        {
            var s = "Score: " + Score + " [" + People.Select(x => x == null ? "<empty>" : x.Name).Aggregate("", (y, z) => y + z + ", ");
            s = s.Substring(0, s.Length - 2);
            return s;
        }
    }
}