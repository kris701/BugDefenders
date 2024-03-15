using BugDefender.Core.Game.Models.Maps;
using BugDefender.Tools;
using CommandLine;
using CommandLine.Text;
using MapPathBlockingTileGen.Models;
using System.Text.Json;

namespace MapPathBlockingTileGen
{
    public class Program
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
                Console.WriteLine(JsonSerializer.Serialize(AutoGenTiles(parsed.Paths, opts.TileRange)));
            else
                Console.WriteLine("Paths file is malformed!");
        }

        public static List<BlockedTile> AutoGenTiles(List<List<FloatPoint>> paths, float range)
        {
            var newBlocks = new List<BlockedTile>();
            foreach (var path in paths)
            {
                var from = path[0];
                foreach (var point in path.Skip(1))
                {
                    var minX = Math.Min(from.X, point.X);
                    var minY = Math.Min(from.Y, point.Y);
                    var maxX = Math.Max(from.X, point.X);
                    var maxY = Math.Max(from.Y, point.Y);

                    minX -= range / 2;
                    minY -= range / 2;
                    maxX += range / 2;
                    maxY += range / 2;

                    var width = Math.Abs(maxX - minX);
                    var heigth = Math.Abs(maxY - minY);
                    newBlocks.Add(new BlockedTile(minX, minY, width, heigth));
                    from = point;
                }
            }
            return newBlocks;
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