using System;
using System.Collections.Generic;
using System.IO;
using HackAssembler.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HackAssembler
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceProvider serviceProvider = SetupIocAndLogging();
            //var logger = GetLogger(serviceProvider);
            //logger.LogInformation("Starting application");

            DisplayIntro();
            if (!CheckSourceFileOk(args)) return;
            bool consoleOnly = args.Length > 1 && args[1] == "--console-only";

            IParser parser = serviceProvider.GetRequiredService<IParser>();
            ISymbolTableBuilder symbolTableBuilder = serviceProvider.GetRequiredService<ISymbolTableBuilder>();
            IAssembler assembler = serviceProvider.GetRequiredService<IAssembler>();
            string sourceFile = args[0];

            using var stream = File.OpenText(sourceFile);
            var parsedLines = new List<ParsedLine>();
            string line;
            while ((line = stream.ReadLine()) != null)
            {
                parsedLines.Add(parser.Parse(line));
            }
            Dictionary<string, int> symbolTable = symbolTableBuilder.BuildSymbolTable(parsedLines.ToArray());

            bool valid = true;
            int lineNumber = 1;
            foreach (var parsedLine in parsedLines)
            {
                if (parsedLine.Type == ParsedType.Invalid)
                {
                    Console.WriteLine($"Line {lineNumber}: {parsedLine.Error}");
                    valid = false;
                }
            }
            if (!valid) return;

            var results = new List<string>();
            foreach (var parsedLine in parsedLines)
            {
                string binary = assembler.ConvertToBinary(parsedLine, symbolTable);
                if (binary != null) results.Add(binary);
            }
            if (consoleOnly)
            {
                results.ForEach(r => Console.WriteLine(r));
            }
            else
            {
                string outputFile = $"{Path.GetDirectoryName(sourceFile)}/{Path.GetFileNameWithoutExtension(sourceFile)}.hack";
                if (File.Exists(outputFile)) File.Delete(outputFile);
                File.WriteAllLines(outputFile, results.ToArray());
                Console.WriteLine($"Results written to {outputFile}");
            }
        }

        private static ILogger<Program> GetLogger(ServiceProvider serviceProvider)
        {
            return serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();
        }

        private static ServiceProvider SetupIocAndLogging()
        {
            return new ServiceCollection()
                  .AddLogging((ILoggingBuilder loggingBuilder) => { loggingBuilder.AddConsole(); })
                  .Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information)
                  .AddSingleton<IParser, Parser>()
                  .AddSingleton<ISymbolTableBuilder, SymbolTableBuilder>()
                  .AddSingleton<IAssembler, Assembler>()
                  .BuildServiceProvider();
        }

        private static void DisplayIntro()
        {
            Console.WriteLine("HACK Assembler");
            Console.WriteLine("--------------");
            Console.WriteLine("Usage: dotnet ./HackAssembler.dll [source-file] [--console-only]");
            Console.WriteLine();
            Console.WriteLine("source-file:");
            Console.WriteLine("    Path to file containing assembly code (must have an .asm file extension)");
            Console.WriteLine("--console-only:");
            Console.WriteLine("    If this option is specified, results will be written to the console instead of to a .hack file.");
            Console.WriteLine("");
            Console.WriteLine("Results will be written to a file named after the source file, but with a .hack file extension. Any existing file with this name will be overwritten.");
            Console.WriteLine("");
        }

        private static bool CheckSourceFileOk(string[] args)
        {
            string error = null;
            if (args.Length == 0)
                error = "No source file specified.";
            else
            {
                string sourceFile = args[0];
                if (!File.Exists(sourceFile))
                    error = $"Source file {sourceFile} does not exist.";
                else if (Path.GetExtension(sourceFile) != ".asm")
                    error = $"Source file {sourceFile} does not have an .asm file extension.";
            }
            if (error != null)
                Console.WriteLine(error);
            return error == null;
        }
    }
}
