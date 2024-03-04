using BugDefender.Core.Game.Models.Maps;
using CommandLine;
using CommandLine.Text;
using MapPathBlockingTileGen.Models;
using System.Text.Json;

namespace MapPathBlockingTileGen
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<Options>(args);
            parserResult.WithNotParsed(errs => DisplayHelp(parserResult, errs));
            parserResult.WithParsed(Run);
        }

        private static void Run(Options opts)
        {
            Console.WriteLine("Parsing paths file...");
            var target = RootPath(opts.PathsFile);
            var parsed = JsonSerializer.Deserialize<PathsModel>(File.ReadAllText(target));
            if (parsed != null)
            {
                var newBlocks = new List<BlockedTile>();
                foreach (var path in parsed.Paths)
                {
                    var from = path[0];
                    foreach (var point in path.Skip(1))
                    {
                        var minX = Math.Min(from.X, point.X);
                        var minY = Math.Min(from.Y, point.Y);
                        var maxX = Math.Max(from.X, point.X);
                        var maxY = Math.Max(from.Y, point.Y);

                        minX -= opts.TileRange / 2;
                        minY -= opts.TileRange / 2;
                        maxX += opts.TileRange / 2;
                        maxY += opts.TileRange / 2;

                        var width = Math.Abs(maxX - minX);
                        var heigth = Math.Abs(maxY - minY);
                        newBlocks.Add(new BlockedTile(minX, minY, width, heigth));
                        from = point;
                    }
                }

                Console.WriteLine("Completed:");
                Console.WriteLine(JsonSerializer.Serialize(newBlocks));
            }
            else
                Console.WriteLine("Paths file is malformed!");
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            var sentenceBuilder = SentenceBuilder.Create();
            foreach (var error in errs)
                if (error is not HelpRequestedError)
                    Console.WriteLine(sentenceBuilder.FormatError(error));
        }

        private static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
        {
            var helpText = HelpText.AutoBuild(result, h =>
            {
                h.AddEnumValuesToHelpText = true;
                return h;
            }, e => e, verbsIndex: true);
            Console.WriteLine(helpText);
            HandleParseError(errs);
        }

        private static string RootPath(string path)
        {
            if (!Path.IsPathRooted(path))
                path = Path.Join(Directory.GetCurrentDirectory(), path);
            path = path.Replace("\\", "/");
            return path;
        }
    }
}