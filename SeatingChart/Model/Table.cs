using System.Collections.Generic;

namespace SeatingChart.Model
{
    public class Table
    {
        public List<Person> People { get; set; }

        public Table()
        {
            People = new List<Person>();
        }
    }
}