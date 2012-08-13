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

        private Random Random { get; set; }

        public Runner(Configuration cfg, int initialPopulation)
        {
            Random = new Random();
            Configuration = cfg;
            Population = new List<Arrangement>();

            Seats = cfg.NumberOfTables * cfg.PeoplePerTable;
            if (Configuration.People.Count > Seats)
                throw new ApplicationException("Too many people for the number of seats");

            CurrentArrangement = new Arrangement(Configuration.NumberOfTables, Configuration.PeoplePerTable);

            Initialize(initialPopulation);
        }

        public void RunGeneration()
        {
            var left = Random.Next(0, Population.Count);
            var right = Random.Next(0, Population.Count);

            var next = Combine(Population[left], Population[right]);
            CalculateFitness(next);

            if (next.Score > Population[left].Score && next.Score > Population[right].Score)
            {
                if (Population[left].Score > Population[right].Score)
                {
                    Population[right] = next;
                }
            }
            else if (next.Score > Population[left].Score && next.Score < Population[right].Score)
            {
                Population[left] = next;
            }
        }

        /// <summary>
        /// Initializes the populations
        /// </summary>
        /// <param name="initialPopulation">Size of the initial population</param>
        private void Initialize(int initialPopulation)
        {
            for (var i = 0; i < initialPopulation; i++)
            {
                var enumeration = Enumerable.Range(0, Configuration.People.Count).Shuffle(Random).ToList();
                var arrangement = new Arrangement(Configuration.NumberOfTables, Configuration.PeoplePerTable);

                foreach (var index in enumeration)
                {
                    while (true)
                    {
                        var nextTable = Random.Next(0, Configuration.NumberOfTables);
                        var nextSeat = Random.Next(0, Configuration.PeoplePerTable);

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

        private Arrangement Combine(Arrangement left, Arrangement right)
        {
            var nextArrangement = new Arrangement(Configuration.NumberOfTables, Configuration.PeoplePerTable);

            for (var i = 0; i < Configuration.NumberOfTables; i++)
            {
                for (var j = 0; j < Configuration.PeoplePerTable; j++)
                {
                    var leftPerson = left.Tables[i].People[j];
                    var rightPerson = right.Tables[i].People[j];

                    if (leftPerson == null && rightPerson == null)
                    {
                        if (Random.Next(0, 3) == 0)
                            continue;

                        var randomPerson = Configuration.People[Random.Next(0, Configuration.People.Count)];
                        if (!nextArrangement.HasPerson(randomPerson))
                            nextArrangement.Tables[i].People[j] = randomPerson;
                    }
                    else if (leftPerson != null && rightPerson == null)
                    {
                        if (Random.Next(0, 3) == 0)
                            continue;

                        if (!nextArrangement.HasPerson(leftPerson))
                            nextArrangement.Tables[i].People[j] = leftPerson;
                    }
                    else if (leftPerson == null && rightPerson != null)
                    {
                        if (Random.Next(0, 3) == 0)
                            continue;

                        if (!nextArrangement.HasPerson(rightPerson))
                            nextArrangement.Tables[i].People[j] = rightPerson;
                    }
                    else
                    {
                        if (Random.Next(0, 3) == 0)
                            continue;

                        var next = Random.Next(0, 1);
                        if (next == 0)
                            nextArrangement.Tables[i].People[j] = leftPerson;
                        else
                            nextArrangement.Tables[i].People[j] = rightPerson;
                    }
                }
            }

            var unassigned = Configuration.People.Where(x => nextArrangement.HasPerson(x) == false).ToList();
            while (unassigned.Count() > 0)
            {
                var nextTable = Random.Next(0, Configuration.NumberOfTables);
                var nextSeat = Random.Next(0, Configuration.PeoplePerTable);

                if (nextArrangement.Tables[nextTable].People[nextSeat] == null)
                {
                    nextArrangement.Tables[nextTable].People[nextSeat] = unassigned[0];
                    unassigned.RemoveAt(0);
                    continue;
                }
            }

            return nextArrangement;
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