using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HackAssembler.Core;

namespace HackAssembler
{
    class Program
    {
        static void Main(string[] args)
        {
            DisplayIntro();
            if (!CheckSourceFileOk(args))
                return;
            string sourceFile = args[0];

            LineOfCode[] parsedLines = Parse(sourceFile);
            Dictionary<string, int> symbolTable = BuildSymbolTable(parsedLines);
            if (!CheckForParsingErrors(parsedLines))
                return;

            string[] results = Assemble(parsedLines, symbolTable);
            WriteToOutput(args, sourceFile, results);
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

        private static LineOfCode[] Parse(string sourceFile)
        {
            var parser = new Parser();
            var parsedLines = new List<LineOfCode>();
            using (var stream = File.OpenText(sourceFile))
            {
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    parsedLines.Add(parser.Parse(line));
                }
            }
            return parsedLines.ToArray();
        }

        private static string[] Assemble(LineOfCode[] parsedLines, Dictionary<string, int> symbolTable)
        {
            var assembler = new Assembler();
            var results = new List<string>();
            foreach (var parsedLine in parsedLines)
            {
                string binary = assembler.ConvertToBinary(parsedLine, symbolTable);
                if (binary != null) results.Add(binary);
            }
            return results.ToArray();
        }

        private static Dictionary<string, int> BuildSymbolTable(LineOfCode[] parsedLines)
        {
            var symbolTableBuilder = new SymbolTableBuilder();
            Dictionary<string, int> symbolTable = symbolTableBuilder.BuildSymbolTable(parsedLines);
            return symbolTable;
        }

        private static bool CheckForParsingErrors(LineOfCode[] parsedLines)
        {
            bool valid = true;
            int lineNumber = 1;
            foreach (var parsedLine in parsedLines)
            {
                if (parsedLine.Type == InstructionType.Invalid)
                {
                    Console.WriteLine($"Line {lineNumber}: {parsedLine.Error}");
                    valid = false;
                }
            }

            return valid;
        }

        private static bool IsConsoleOnly(string[] args)
        {
            return args.Length > 1 && args[1] == "--console-only";
        }

        private static void WriteToOutput(string[] args, string sourceFile, string[] results)
        {
            if (IsConsoleOnly(args))
            {
                results.ToList().ForEach(r => Console.WriteLine(r));
            }
            else
            {
                string outputFile = $"{Path.GetDirectoryName(sourceFile)}/{Path.GetFileNameWithoutExtension(sourceFile)}.hack";
                if (File.Exists(outputFile))
                    File.Delete(outputFile);
                File.WriteAllLines(outputFile, results);
                Console.WriteLine($"Results written to {outputFile}");
            }
        }

    }
}
