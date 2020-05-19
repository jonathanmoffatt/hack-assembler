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
                return new ParseResult { Type = ParsedType.Label, Label = line[1..^1] };
            return ParseComputation(line);
        }

        private ParseResult ParseComputation(string line)
        {
            var compMappings = GetCompMappings();
            bool hasDest = line.Contains('=');
            bool hasJump = line.Contains(';');
            string[] split = line.Split(new[] { "=", ";" }, StringSplitOptions.RemoveEmptyEntries);
            return new ParseResult
            {
                Type = ParsedType.CInstruction,
                Dest = hasDest ? Enum.Parse<Dest>(split[0]) : (Dest?)null,
                Comp = compMappings[split[hasDest ? 1 : 0]],
                Jump = hasJump ? Enum.Parse<Jump>(split[hasDest ? 2 : 1]) : (Jump?)null
            };
        }

        private static ParseResult ParseAddress(string line)
        {
            var result = new ParseResult
            {
                Type = ParsedType.AInstruction
            };
            line = line[1..];
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
