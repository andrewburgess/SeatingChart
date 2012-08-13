using System;
using System.Collections.Generic;
using System.Linq;
using SeatingChart.Model;

namespace SeatingChart
{
    public class Runner
    {
        public Configuration Configuration { get; set; }
        public Arrangement CurrentArrangement { get; set; }

        private List<Arrangement> Population { get; set; }

        public Runner(Configuration cfg, int initialPopulation)
        {
            Configuration = cfg;
            Population = new List<Arrangement>();

            var seats = cfg.NumberOfTables*cfg.PeoplePerTable;
            if (Configuration.People.Count > seats)
                throw new ApplicationException("Too many people for the number of seats");

            CurrentArrangement = new Arrangement(Configuration.NumberOfTables, Configuration.PeoplePerTable);

            Initialize(initialPopulation);
        }

        private void Initialize(int initialPopulation)
        {
            for (var i = 0; i < initialPopulation; i++)
            {
                var enumeration = Enumerable.Range(0, Configuration.People.Count).Shuffle().ToList();
                var arrangement = new Arrangement(Configuration.NumberOfTables, Configuration.PeoplePerTable);
                for (var j = 0; j < Configuration.NumberOfTables; j++)
                {
                    arrangement.Tables[j] = new Table(Configuration.PeoplePerTable);
                    for (var k = 0; k < Configuration.PeoplePerTable; k++)
                    {
                        arrangement.Tables[j].People[k] = Configuration.People[enumeration[0]];
                        enumeration.RemoveAt(0);

                        if (enumeration.Count == 0)
                            break;
                    }

                    if (enumeration.Count == 0)
                        break;
                }

                Population.Add(arrangement);
            }
        }
    }
}