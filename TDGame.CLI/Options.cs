using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.CLI
{
    public class Options
    {
        [Option("mods", Required = true, HelpText = "Paths to your mods")]
        public IEnumerable<string> ModPath { get; set; } = new List<string>();
    }
}
