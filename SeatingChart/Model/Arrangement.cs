using System;
using System.Collections.Generic;
using System.Linq;

namespace SeatingChart.Model
{
    public class Arrangement : IComparable<Arrangement>
    {
        private Guid Id { get; set; }
        public int Score { get; set; }
        public List<Table> Tables { get; set; }

        public Arrangement(int numberOfTables, int peoplePerTable)
        {
            Id = Guid.NewGuid();
            Tables = new List<Table>();

            for (var i = 0; i < numberOfTables; i++)
            {
                Tables.Add(new Table(peoplePerTable));
            }
        }

        public bool HasPerson(Person randomPerson)
        {
            return Tables.Any(table => table.People.Contains(randomPerson) || table.People.Where(x => x != null).Count(x => x.Name == randomPerson.Name) > 0);
        }

        public int CompareTo(Arrangement other)
        {
            return Id.CompareTo(other.Id);
        }

        public override string ToString()
        {
            return "Arrangment Score: " + Score;
        }
    }
}