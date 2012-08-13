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

        public List<Arrangement> Population { get; set; }
        private int Seats { get; set; }

        public Runner(Configuration cfg, int initialPopulation)
        {
            Configuration = cfg;
            Population = new List<Arrangement>();

            Seats = cfg.NumberOfTables * cfg.PeoplePerTable;
            if (Configuration.People.Count > Seats)
                throw new ApplicationException("Too many people for the number of seats");

            CurrentArrangement = new Arrangement(Configuration.NumberOfTables, Configuration.PeoplePerTable);

            Initialize(initialPopulation);
        }

        /// <summary>
        /// Initializes the populations
        /// </summary>
        /// <param name="initialPopulation">Size of the initial population</param>
        private void Initialize(int initialPopulation)
        {
            var random = new Random();
            for (var i = 0; i < initialPopulation; i++)
            {
                var enumeration = Enumerable.Range(0, Configuration.People.Count).Shuffle(random).ToList();
                var arrangement = new Arrangement(Configuration.NumberOfTables, Configuration.PeoplePerTable);

                foreach (var index in enumeration)
                {
                    while (true)
                    {
                        var nextTable = random.Next(0, Configuration.NumberOfTables);
                        var nextSeat = random.Next(0, Configuration.PeoplePerTable);

                        if (arrangement.Tables[nextTable].People[nextSeat] == null)
                        {
                            arrangement.Tables[nextTable].People[nextSeat] = Configuration.People[index];
                            break;
                        }
                    }
                }

                CalculateFitness(arrangement);
                Population.Add(arrangement);
            }
        }

        private void CalculateFitness(Arrangement arrangement)
        {
            var score = 0;
            foreach (var table in arrangement.Tables)
            {
                var currentTable = table;
                var tableScore = (from firstPerson in currentTable.People
                                  where firstPerson != null
                                  from secondPerson in currentTable.People
                                  where secondPerson != null
                                  where firstPerson != secondPerson
                                  let r = secondPerson
                                  where firstPerson.Relationships.Count(x => x.Other == r.Name) > 0
                                  select firstPerson.Relationships.First(x => x.Other == r.Name).Score).Sum();
                table.Score = tableScore;
                score += tableScore;
            }

            arrangement.Score = score;
        }
    }
}