using System.Collections.Generic;

namespace HackAssembler.Core
{
    public class SymbolTableBuilder : ISymbolTableBuilder
    {
        public Dictionary<string, int> BuildSymbolTable(params ParsedLine[] parsedLines)
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
            foreach (var parsedLine in parsedLines)
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
                if (parsedLine.Type == ParsedType.AInstruction && parsedLine.AddressSymbol != null)
                {
                    if (!result.ContainsKey(parsedLine.AddressSymbol))
                    {
                        result.Add(parsedLine.AddressSymbol, variableAddress);
                        variableAddress++;
                    }
                }
                if (parsedLine.Type == ParsedType.AInstruction || parsedLine.Type == ParsedType.CInstruction)
                    pc++;
            }
            return result;
        }

    }
}
