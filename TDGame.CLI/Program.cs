using CommandLine;
using CommandLine.Text;
using System;
using TDGame.Core.Resources;
using TDGame.Core.Resources.Integrity;

namespace TDGame.CLI
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
            var mods = opts.ModPath.ToList();
            for (int i = 0; i < mods.Count; i++)
                mods[i] = RootPath(mods[i]);

            Console.WriteLine("Resetting resource manager...");
            ResourceManager.UnloadExternalResources();

            Console.WriteLine("Loading mod into resource manager...");
            foreach(var mod in mods)
                ResourceManager.LoadResource(new DirectoryInfo(mod));

            Console.WriteLine("Checking mod integrity...");
            var checker = new ResourceIntegrityChecker();
            checker.CheckGameIntegrity();

            if (checker.Errors.Count == 0)
            {
                Console.WriteLine("No issues was found!");
            }
            else
            {
                Console.WriteLine($"{checker.Errors.Count} issues found!");
                Console.WriteLine();
                foreach(var error in checker.Errors)
                    Console.WriteLine(error.ToString());
            }
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