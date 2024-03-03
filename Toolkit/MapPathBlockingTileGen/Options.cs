using CommandLine;

namespace MapPathBlockingTileGen
{
    public class Options
    {
        [Option("paths", Required = true, HelpText = "Path to the 'paths.json' file")]
        public string PathsFile { get; set; }
        [Option("tileRange", Required = true, HelpText = "Range for how big the blocking tiles should be around the paths.")]
        public float TileRange { get; set; } = 50;
    }
}
