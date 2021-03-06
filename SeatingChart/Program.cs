﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SeatingChart.Model;

namespace SeatingChart
{
    class Program
    {
        private static Dictionary<string, CommandRunner> Commands { get; set; }
        private static Runner Runner { get; set; }

        static void Main(string[] args)
        {
            Console.WriteLine("".PadRight(79, '-'));
            Console.WriteLine("Andrew's Wedding Seat Planner");
            Console.WriteLine("By Andrew Burgess <andrew@deceptacle.com>");
            Console.WriteLine();
            Console.WriteLine("Version 1.0.0");
            Console.WriteLine("".PadRight(79, '-'));
            Console.WriteLine();
            Console.WriteLine();

            Commands = SetupCommands();

            var input = "";

            while (input.ToLower() != "exit" && input.ToLower() != "quit")
            {
                try
                {
                    Console.Write("Enter Command (type help for list): ");
                    input = Console.ReadLine();

                    var tokens = input.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    if (Commands.ContainsKey(tokens[0].ToLower()) == false)
                    {
                        Console.WriteLine("Command was invalid.\n");
                        continue;
                    }

                    Commands[tokens[0].ToLower()].Command.DynamicInvoke(tokens.Skip(1).ToArray());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Invalid input: " + e.Message + "\n");
                }
            }
        }

        private static Dictionary<string, CommandRunner> SetupCommands()
        {
            return new Dictionary<string, CommandRunner>
                       {
                           {
                               "help", new CommandRunner
                                           {
                                               Help = "help : Prints out all of the available commands for the system",
                                               Command = new Action(DisplayHelp)
                                           }
                               },
                           {
                               "parse-names", new CommandRunner
                                                  {
                                                      Help =
                                                          "parse-names {input} {output} : Converts the list of names to a basic input.json file to be edited later",
                                                      Command = new Action<string, string>(ParseNames)
                                                  }
                               },
                           {
                               "initialize", new CommandRunner
                                                 {
                                                     Help =
                                                         "initialize {input-file} {population-size} : Initializes the population with the specified file",
                                                     Command = new Action<string, string>(InitializePopulation)
                                                 }
                               },
                           {
                               "parse-relations", new CommandRunner
                                                      {
                                                          Help =
                                                              "parse-relations {input} {output} : Converts relations to basic json files",
                                                          Command = new Action<string, string>(ParseRelations)
                                                      }
                               },
                               {
                               "run", new CommandRunner
                                                      {
                                                          Help =
                                                              "run {generations} : Runs the specified number of generations",
                                                          Command = new Action<string>(RunGenerations)
                                                      }
                               },
                               {
                               "save", new CommandRunner
                                                      {
                                                          Help =
                                                              "save {output} : Saves the current best arrangement",
                                                          Command = new Action<string>(SaveBest)
                                                      }
                               },
                       };
        }

        private static void InitializePopulation(string inputFile, string populationSize)
        {
            var jsSerializer = new JsonSerializer();
            var textReader = new StreamReader(inputFile);
            var reader = new JsonTextReader(textReader);
            var cfg = jsSerializer.Deserialize<Configuration>(reader);
            textReader.Close();
            reader.Close();

            Runner = new Runner(cfg, int.Parse(populationSize));

            Console.WriteLine("Initialized");
            Console.WriteLine("Best table score is: " + Runner.BestScore());
        }

        private static void RunGenerations(string number)
        {
            for (var i = 0; i < int.Parse(number); i++)
            {
                Runner.RunGeneration();
                Console.WriteLine("Generation " + (i + 1) + ": " + Runner.BestScore() + " (avg: " + Runner.AverageScore() + ")");
            }
        }

        private static void SaveBest(string output)
        {
            var serializer = new JsonSerializer();
            
            var textWriter = new StreamWriter(output);
            var writer = new JsonTextWriter(textWriter) { Formatting = Formatting.Indented, Indentation = 4 };
            serializer.Serialize(writer, Runner.CurrentArrangement);
            textWriter.Flush();
            textWriter.Close();

            textWriter = new StreamWriter("pretty-" + output);
            foreach (var table in Runner.CurrentArrangement.Tables)
            {
                textWriter.WriteLine("Table (" + table.Score + ")");
                textWriter.WriteLine("".PadRight(80, '='));

                foreach (var person in table.People.Where(x => x != null))
                    textWriter.WriteLine(person.Name);

                textWriter.WriteLine("\n");
            }
            textWriter.Flush();
            textWriter.Close();
        }

        private static void ParseNames(string input, string output)
        {
            var stringReader = new StreamReader(input);
            var data = stringReader.ReadToEnd();
            stringReader.Close();
            var lines = data.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

            var cfg = new Configuration();
            foreach (var line in lines)
            {
                var currentLine = line;
                if (cfg.People.Count(x => x.Name == currentLine) > 0)
                    throw new ApplicationException("Name duplication: " + line);

                cfg.People.Add(new Person
                                   {
                                       Name = line
                                   });
            }

            var serializer = new JsonSerializer();
            var textWriter = new StreamWriter(output);
            var writer = new JsonTextWriter(textWriter) {Formatting = Formatting.Indented, Indentation = 4};
            serializer.Serialize(writer, cfg);
            textWriter.Flush();
            textWriter.Close();

            Console.WriteLine("Wrote out " + lines.Count() + " guests");
            Console.WriteLine();
        }

        private static void ParseRelations(string input, string output)
        {
            var serializer = new JsonSerializer();
            var textReader = new StreamReader(output);
            var reader = new JsonTextReader(textReader);
            var cfg = serializer.Deserialize<Configuration>(reader);
            textReader.Close();
            reader.Close();

            var stringReader = new StreamReader(input);
            var data = stringReader.ReadToEnd();
            stringReader.Close();
            var lines = data.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
         
            foreach (var line in lines)
            {
                var currentLine = line;
                var left = currentLine.Split(new[] {" + "}, StringSplitOptions.RemoveEmptyEntries)[0];
                var right = currentLine.Split(new[] { " + " }, StringSplitOptions.RemoveEmptyEntries)[1];

                if (cfg.People.Count(x => x.Name == left) == 0)
                    throw new ApplicationException("Name not found: " + left);
                if (cfg.People.Count(x => x.Name == right) == 0)
                    throw new ApplicationException("Name not found: " + right);

                cfg.Relationships.Add(new Relationship()
                {
                    Left = left,
                    Right = right,
                    Score = 100
                });
            }

            
            var textWriter = new StreamWriter(output);
            var writer = new JsonTextWriter(textWriter) { Formatting = Formatting.Indented, Indentation = 4 };
            serializer.Serialize(writer, cfg);
            textWriter.Flush();
            textWriter.Close();

            Console.WriteLine("Wrote out " + lines.Count() + " relationships");
            Console.WriteLine();
        }

        private static void DisplayHelp()
        {
            foreach (var cmd in Commands.OrderBy(x => x.Key))
            {
                Console.WriteLine(cmd.Value.Help);
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
