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
            var result = new Dictionary<string, int>();
            foreach(var parsedLine in parsedLines)
            {
                if (parsedLine.Type == ParsedType.Label)
                    result.Add(parsedLine.Label, pc);
                pc++;
            }
            return result;
        }

        private static ParseResult ParseLabel(string line)
        {
            string label = line[1..^1].Trim();
            if (label == "")
                return new ParseResult { Type = ParsedType.Unrecognised, Error = "Empty labels are not permitted." };
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
                return new ParseResult { Type = ParsedType.Unrecognised, Error = ex.Message };
            }
        }

        private static ParseResult ParseAddress(string line)
        {
            line = line[1..];
            if (line == "")
                return new ParseResult { Type = ParsedType.Unrecognised, Error = "Empty addresses are not permitted." };
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
