using System.Collections.Generic;
using System.Linq;

namespace HackAssembler.Core
{
    public class SymbolTableBuilder
    {
        private const int empty = -1;

        public Dictionary<string, int> BuildSymbolTable(params LineOfCode[] parsedLines)
        {
            var table = new Dictionary<string, int> {
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

            int pc = 0;
            foreach (var parsedLine in parsedLines)
            {
                if (parsedLine.Type == InstructionType.Label)
                {
                    if (!table.ContainsKey(parsedLine.Label) || table[parsedLine.Label] == empty)
                        table[parsedLine.Label] = pc;
                    else
                    {
                        parsedLine.Type = InstructionType.Invalid;
                        parsedLine.Error = "Duplicated label.";
                    }
                }
                if (parsedLine.Type == InstructionType.AInstruction && parsedLine.AddressSymbol != null)
                {
                    if (!table.ContainsKey(parsedLine.AddressSymbol))
                        table.Add(parsedLine.AddressSymbol, empty);
                }
                if (parsedLine.Type == InstructionType.AInstruction || parsedLine.Type == InstructionType.CInstruction)
                    pc++;
            }

            int variableAddress = 16;
            string[] variables = table.Where(r => r.Value == empty).Select(r => r.Key).ToArray();
            foreach (string variable in variables)
            {
                table[variable] = variableAddress;
                variableAddress++;
            }

            return table;
        }

    }
}
