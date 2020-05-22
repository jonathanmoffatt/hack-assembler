using System;
using System.Collections.Generic;

namespace HackAssembler.Core
{
    public class Parser: IParser
    {
        public ParseResult Parse(string line)
        {
            line = StripComments(line);
            if (line.StartsWith('@'))
                return ParseAddress(line);
            else if (line == "")
                return new ParseResult { Type = ParsedType.Whitespace };
            else if (line.StartsWith("(") && line.EndsWith(")"))
                return ParseLabel(line);
            return ParseComputation(line);
        }

        public Dictionary<string, int> BuildSymbolTable(params ParseResult[] parsedLines)
        {
            int pc = 0;
            int variableAddress = 16;
            var result = new Dictionary<string, int> {
                {"R0", 0 },
                {"R1", 1 },
                {"R2", 2 },
                {"R3", 3 },
                {"R4", 4 },
                {"R5", 5 },
                {"R6", 6 },
                {"R7", 7 },
                {"R8", 8 },
                {"R9", 9 },
                {"R10", 10 },
                {"R11", 11 },
                {"R12", 12 },
                {"R13", 13 },
                {"R14", 14 },
                {"R15", 15 },
                {"SCREEN", 16384 },
                {"KBD", 24576 },
                {"SP", 0 },
                {"LCL", 1 },
                {"ARG", 2 },
                {"THIS", 3 },
                {"THAT", 4 }
            };
            foreach(var parsedLine in parsedLines)
            {
                if (parsedLine.Type == ParsedType.Label)
                {
                    if (!result.ContainsKey(parsedLine.Label))
                        result.Add(parsedLine.Label, pc);
                    else
                    {
                        parsedLine.Type = ParsedType.Invalid;
                        parsedLine.Error = "Duplicated label.";
                    }

                }
                pc++;
                if (parsedLine.Type == ParsedType.AInstruction && parsedLine.AddressSymbol != null)
                {
                    if (!result.ContainsKey(parsedLine.AddressSymbol))
                    {
                        result.Add(parsedLine.AddressSymbol, variableAddress);
                        variableAddress++;
                    }
                }
            }
            return result;
        }

        private static ParseResult ParseLabel(string line)
        {
            string label = line[1..^1].Trim();
            if (label == "")
                return new ParseResult { Type = ParsedType.Invalid, Error = "Empty labels are not permitted." };
            return new ParseResult { Type = ParsedType.Label, Label = label };
        }

        private ParseResult ParseComputation(string line)
        {
            var compMappings = GetCompMappings();
            bool hasDest = line.Contains('=');
            bool hasJump = line.Contains(';');
            string[] split = line.Split(new[] { "=", ";" }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                return new ParseResult
                {
                    Type = ParsedType.CInstruction,
                    Dest = hasDest ? Enum.Parse<Dest>(split[0]) : (Dest?)null,
                    Comp = compMappings[split[hasDest ? 1 : 0]],
                    Jump = hasJump ? Enum.Parse<Jump>(split[hasDest ? 2 : 1]) : (Jump?)null
                };
            }
            catch(Exception ex)
            {
                return new ParseResult { Type = ParsedType.Invalid, Error = ex.Message };
            }
        }

        private static ParseResult ParseAddress(string line)
        {
            line = line[1..];
            if (line == "")
                return new ParseResult { Type = ParsedType.Invalid, Error = "Empty addresses are not permitted." };
            var result = new ParseResult { Type = ParsedType.AInstruction };
            if (int.TryParse(line, out int addr))
                result.Address = addr;
            else
                result.AddressSymbol = line;
            return result;
        }

        private string StripComments(string line)
        {
            return line.Trim().Split("//")[0].Trim();
        }

        private Dictionary<string, Comp> GetCompMappings()
        {
            var result = new Dictionary<string, Comp>();
            foreach(Comp c in Enum.GetValues(typeof(Comp)))
            {
                result.Add(c.GetAttribute<AssemblerMappingAttribute, Comp>().Assembler, c);
            }
            return result;
        }

    }
}
