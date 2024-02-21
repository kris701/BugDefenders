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
        [Option("path", Required = true, HelpText = "Path to your current mod")]
        public string ModPath { get; set; } = "";
    }
}
