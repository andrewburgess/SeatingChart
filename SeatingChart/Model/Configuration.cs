using System.Collections.Generic;

namespace SeatingChart.Model
{
    public class Configuration
    {
        public int NumberOfTables { get; set; }
        public int PeoplePerTable { get; set; }

        public List<Person> People { get; set; }

        public Configuration()
        {
            People = new List<Person>();
        }
    }
}