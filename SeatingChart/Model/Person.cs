using System;
using System.Collections.Generic;

namespace SeatingChart.Model
{
    public class Person : IComparable<Person>
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public Politics Politics { get; set; }
        public int DrunkFactor { get; set; }

        public Person()
        {
            Politics = new Politics();
        }

        public int CompareTo(Person obj)
        {
            return Name.CompareTo(obj.Name);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}