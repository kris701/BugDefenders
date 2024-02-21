using CommandLine;

namespace TDGame.CLI
{
    public class Options
    {
        [Option("mods", Required = true, HelpText = "Paths to your mods")]
        public IEnumerable<string> ModPath { get; set; } = new List<string>();
    }
}
