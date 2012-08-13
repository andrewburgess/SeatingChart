using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using SeatingChart.Model;

namespace SeatingChart
{
    class Program
    {
        static void Main(string[] args)
        {
            var jsSerializer = new Newtonsoft.Json.JsonSerializer();
            var textReader = new StreamReader("input.json");
            var reader = new JsonTextReader(textReader);
            var cfg = jsSerializer.Deserialize<Configuration>(reader);

            Console.ReadLine();
        }
    }
}
