using System;
using System.Collections;
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

        private int InitialPopulation { get; set; }

        public Runner(Configuration cfg, int initialPopulation)
        {
            Random = new Random();
            Configuration = cfg;
            Population = new List<Arrangement>();

            Seats = cfg.NumberOfTables * cfg.PeoplePerTable;
            if (Configuration.People.Count > Seats)
                throw new ApplicationException("Too many people for the number of seats");

            InitialPopulation = initialPopulation;
            Initialize(InitialPopulation);
            CurrentArrangement = Population.OrderByDescending(x => x.Score).First();
        }

        public void RunGeneration()
        {
            var combinations = Enumerable.Range(2, 1)
                .Aggregate(
                    Enumerable.Empty<IEnumerable<Arrangement>>(),
                    (acc, i) =>
                    acc.Concat(Enumerable.Repeat(Population, i).Combinations()).OrderByDescending(
                        x => x.First().Score + x.Skip(1).First().Score)).ToList();

            var newPopulation = new List<Arrangement>();
            foreach (var combo in combinations)
            {
                var left = combo.First();
                var right = combo.Skip(1).First();

                var next = Combine(left, right);

                next.Score = CalculateFitness(next);

                if (next.Score > left.Score && next.Score > right.Score)
                {
                    newPopulation.Add(next);
                    newPopulation.Add(left.Score > right.Score ? left : right);
                }
                else if (next.Score > left.Score && next.Score < right.Score)
                {
                    newPopulation.Add(right);
                    newPopulation.Add(next);
                }
                else
                {
                    newPopulation.Add(left);
                    newPopulation.Add(right);
                }
            }

            Population.Clear();
            Population = newPopulation.OrderByDescending(x => x.Score).Take(5).ToList();
            Population.AddRange(newPopulation.Shuffle(Random).Take(InitialPopulation - 5).ToList());
            CurrentArrangement = Population.OrderByDescending(x => x.Score).First();
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

                arrangement.Score = CalculateFitness(arrangement);
                Population.Add(arrangement);
            }
        }

        private Arrangement Combine(Arrangement left, Arrangement right)
        {
            var nextArrangement = new Arrangement(Configuration.NumberOfTables, Configuration.PeoplePerTable);
            var odds = Enumerable.Range(0, Configuration.NumberOfTables).Where(x => x % 2 != 0);
            var evens = Enumerable.Range(0, Configuration.NumberOfTables).Where(x => x % 2 == 0);

            //Pick random tables from each side
            foreach (var odd in odds)
                nextArrangement.Tables[odd] = new Table(left.Tables[odd]);
            foreach (var even in evens)
                nextArrangement.Tables[even] = new Table(right.Tables[even]);

            var inTheClear = false;

            while (!inTheClear)
            {
                var found = false;
                foreach (var person in Configuration.People)
                {
                    var current = person;
                    var tables = nextArrangement.Tables.Where(x => x.People.Contains(current)).ToList();
                    if (tables.Count() > 1)
                    {
                        found = true;
                        var offending = nextArrangement.Tables.Where(x => x.People.Contains(current)).ToList();
                        var first = offending.First();
                        var second = offending.Skip(1).First();
                        var firstIndex = first.People.IndexOf(current);
                        var secondIndex = second.People.IndexOf(current);

                        first.People[firstIndex] = null;
                        var firstScore = CalculateFitness(nextArrangement);

                        first.People[firstIndex] = current;
                        second.People[secondIndex] = null;
                        var secondScore = CalculateFitness(nextArrangement);

                        if (firstScore > secondScore)
                        {
                            first.People[firstIndex] = null;
                            second.People[secondIndex] = current;
                        }
                    }
                }

                inTheClear = !found;
            }

            var unassigned = Configuration.People.Where(x => nextArrangement.HasPerson(x) == false).ToList();

            while (unassigned.Count() > 0)
            {
                var tablesWithEmptySeats = nextArrangement.Tables.Where(x => x.People.Count(y => y == null) > 0).ToList();
                var bestTable = tablesWithEmptySeats.First();

                var position = bestTable.People.IndexOf(null);
                bestTable.People[position] = unassigned[0];
                var bestScore = CalculateFitness(nextArrangement);
                var bestPosition = position;

                foreach (var table in tablesWithEmptySeats.Skip(1))
                {
                    bestTable.People[bestPosition] = null;
                    position = table.People.IndexOf(null);
                    table.People[position] = unassigned[0];
                    var score = CalculateFitness(nextArrangement);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestTable.People[bestPosition] = null;
                        bestPosition = position;
                        bestTable = table;
                        bestTable.People[bestPosition] = unassigned[0];
                    }
                    else
                    {
                        bestTable.People[bestPosition] = unassigned[0];
                        table.People[position] = null;
                    }
                }

                unassigned.RemoveAt(0);
            }

            return nextArrangement;
        }

        private int CalculateFitness(Arrangement arrangement)
        {
            var score = 0;
            foreach (var table in arrangement.Tables)
            {
                var currentTable = table;

                var tableScore = 0;

                var combinations = Enumerable.Range(2, 1)
                    .Aggregate(
                        Enumerable.Empty<IEnumerable<Person>>(),
                        (acc, i) =>
                        acc.Concat(Enumerable.Repeat(currentTable.People, i).Combinations())).ToList();

                foreach (var combo in combinations)
                {
                    var leftPerson = combo.FirstOrDefault();
                    var rightPerson = combo.Skip(1).FirstOrDefault();
                    if (leftPerson == null || rightPerson == null) continue;

                    var relationship = Configuration.Relationships.Where(
                        x => ((x.Left == leftPerson.Name) || (x.Right == leftPerson.Name)) &&
                             ((x.Left == rightPerson.Name) || (x.Right == rightPerson.Name))).FirstOrDefault();

                    if (relationship != null)
                    {
                        tableScore += relationship.Score;
                    }
                    else
                    {
                        tableScore -= 25;
                    }

                    //Calculate age differences
                    tableScore -= (int)Math.Pow(leftPerson.Age - rightPerson.Age, 2);

                    //Calculate political differences
                    tableScore += CalculatePolitics(leftPerson.Politics, rightPerson.Politics);

                    //Calculate drunkenness (+15 is for general tolerance of drunken shenanigans)
                    //(i.e. Left is 100, right is 20, score would be -80 + 15
                    tableScore += -1 * Math.Abs(leftPerson.DrunkFactor - rightPerson.DrunkFactor) + 15;
                }

                table.Score = tableScore;
                score += tableScore;
            }

            
            return score;
        }

        private static int CalculatePolitics(Politics leftPolitics, Politics rightPolitics)
        {
            return leftPolitics.Left + rightPolitics.Left + leftPolitics.Right + rightPolitics.Right;
        }

        public int BestScore()
        {
            return this.Population.Max(x => x.Score);
        }

        public int AverageScore()
        {
            return (int)Population.Average(x => x.Score);
        }
    }
}