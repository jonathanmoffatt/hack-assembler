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

            IParser parser = serviceProvider.GetRequiredService<IParser>();
            string sourceFile = args[0];
            using var stream = File.OpenText(sourceFile);
            var parsedLines = new List<ParseResult>();
            string line;
            while ((line = stream.ReadLine()) != null)
            {
                parsedLines.Add(parser.Parse(line));
            }

            /*

            show console name
            show console app usage instructions

            get filename from args
            if no filename
                show error
                quit

            for each line in the file
                parse and store parsing details in an array

            PC = 0
            parsingerrors=[]
            for each parsedline in allparsedlines
                if parsedline is a label location e.g. "(LOOP)"
                    if symbol exists in symbol table
                        if symbol has no value
                            store value of current PC
                        else
                            add parsingerror (duplicate label)
                    else
                        store symbol and value of current PC
                if parsedline is a reference to a label or a variable declaration e.g. "@counter" or "@LOOP"
                    if symbol does not exist in symbol table
                        store in symbol table with empty value
                    PC++
                if parsedline is an a-instruction or c-instruction
                    PC++
                if parsedline has syntax errors
                    add parsingerror

            if any parsingerrors
                output to the console
                quit


            tempstorage = ???
            foreach symbol in symbol table
                if symbol has no value
                    set symbol to tempstorage
                    tempstorage++

            results = []
            foreach parsedline in allparsedlines
                if parsedline is an a-instruction
                    if value is a symbol
                        results.push value of symbol
                    else
                        results.push a-instruction value
                if parsedline is a c-instruction
                    results.push 111 & comp bits & dest bits & jump bits

            foreach result in results
                write to output file
            */
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
                else if (Path.GetExtension(sourceFile) != "asm")
                    error = $"Source file {sourceFile} does not have an .asm file extension.";
            }
            if (error != null)
                Console.WriteLine(error);
            return error == null;
        }
    }
}
