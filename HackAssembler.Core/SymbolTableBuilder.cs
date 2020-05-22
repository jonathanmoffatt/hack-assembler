using System.Collections.Generic;

namespace HackAssembler.Core
{
    public class SymbolTableBuilder : ISymbolTableBuilder
    {
        public Dictionary<string, int> BuildSymbolTable(params ParsedLine[] parsedLines)
        {
            int pc = 0;
            var firstPass = new Dictionary<string, int?> {
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
                    if (!firstPass.ContainsKey(parsedLine.Label) || firstPass[parsedLine.Label] == null)
                        firstPass[parsedLine.Label] = pc;
                    else
                    {
                        parsedLine.Type = ParsedType.Invalid;
                        parsedLine.Error = "Duplicated label.";
                    }
                }
                if (parsedLine.Type == ParsedType.AInstruction && parsedLine.AddressSymbol != null)
                {
                    if (!firstPass.ContainsKey(parsedLine.AddressSymbol))
                    {
                        firstPass.Add(parsedLine.AddressSymbol, null);
                    }
                }
                if (parsedLine.Type == ParsedType.AInstruction || parsedLine.Type == ParsedType.CInstruction)
                    pc++;
            }

            var secondPass = new Dictionary<string, int>();

            int variableAddress = 16;
            foreach (KeyValuePair<string, int?> result in firstPass)
            {
                if (result.Value == null)
                {
                    secondPass.Add(result.Key, variableAddress);
                    variableAddress++;
                }
                else
                {
                    secondPass.Add(result.Key, result.Value.Value);
                }
            }
            return secondPass;
        }

    }
}
