using System;

namespace SeatingChart
{
    public class CommandRunner
    {
        public string Help { get; set; }
        public Delegate Command { get; set; }
    }
}