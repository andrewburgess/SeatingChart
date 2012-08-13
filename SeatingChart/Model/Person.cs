using System.Collections.Generic;

namespace SeatingChart.Model
{
    public class Person
    {
        public string Name { get; set; }

        public List<Relationship> Relationships { get; set; }

        public Person()
        {
            Relationships = new List<Relationship>();
        }
    }
}