using System;
using System.Collections.Generic;

namespace HackAssembler.Core
{
    public class Parser
    {
        public LineOfCode Parse(string line)
        {
            line = StripComments(line);
            if (line.StartsWith('@'))
                return ParseAddress(line);
            else if (line == "")
                return new LineOfCode { Type = InstructionType.Whitespace };
            else if (line.StartsWith("(") && line.EndsWith(")"))
                return ParseLabel(line);
            return ParseComputation(line);
        }

        private static LineOfCode ParseLabel(string line)
        {
            string label = line[1..^1].Trim();
            if (label == "")
                return new LineOfCode { Type = InstructionType.Invalid, Error = "Empty labels are not permitted." };
            return new LineOfCode { Type = InstructionType.Label, Label = label };
        }

        private LineOfCode ParseComputation(string line)
        {
            var compMappings = GetCompMappings();
            bool hasDest = line.Contains('=');
            bool hasJump = line.Contains(';');
            string[] split = line.Split(new[] { "=", ";" }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                return new LineOfCode
                {
                    Type = InstructionType.CInstruction,
                    Dest = hasDest ? Enum.Parse<Dest>(split[0]) : Dest.NotStored,
                    Comp = compMappings[split[hasDest ? 1 : 0]],
                    Jump = hasJump ? Enum.Parse<Jump>(split[hasDest ? 2 : 1]) : Jump.NoJump
                };
            }
            catch(Exception ex)
            {
                return new LineOfCode { Type = InstructionType.Invalid, Error = ex.Message };
            }
        }

        private static LineOfCode ParseAddress(string line)
        {
            line = line[1..];
            if (line == "")
                return new LineOfCode { Type = InstructionType.Invalid, Error = "Empty addresses are not permitted." };
            var result = new LineOfCode { Type = InstructionType.AInstruction };
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
