using System;
using System.IO;
using Newtonsoft.Json;
using SeatingChart.Model;

namespace SeatingChart
{
    class Program
    {
        static void Main(string[] args)
        {
            var jsSerializer = new JsonSerializer();
            var textReader = new StreamReader(args[0]);
            var reader = new JsonTextReader(textReader);
            var cfg = jsSerializer.Deserialize<Configuration>(reader);

            var runner = new Runner(cfg, 20);

            Console.ReadLine();
        }
    }
}
