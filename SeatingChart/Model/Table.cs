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

        public Table(Table toCopy)
        {
            People = new List<Person>(toCopy.People.Count);

            for (var i = 0; i < toCopy.People.Count; i++)
                People.Add(null);

            for (var i = 0; i < toCopy.People.Count; i++)
                People[i] = toCopy.People[i];
        }

        public override string ToString()
        {
            var s = "Score: " + Score + " [" + People.Select(x => x == null ? "<empty>" : x.Name).Aggregate("", (y, z) => y + z + ", ");
            s = s.Substring(0, s.Length - 2);
            return s;
        }
    }
}