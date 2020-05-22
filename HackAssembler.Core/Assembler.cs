using System;
using System.Collections.Generic;

namespace HackAssembler.Core
{
    public class Assembler: IAssembler
    {
        public string ConvertToBinary(ParsedLine parsedLine, Dictionary<string, int> symbolTable)
        {
            switch (parsedLine.Type)
            {
                case ParsedType.AInstruction:
                    int address = parsedLine.Address.HasValue ? parsedLine.Address.Value : symbolTable[parsedLine.AddressSymbol];
                    return Convert.ToString(address, 2).PadLeft(16, '0');
                case ParsedType.CInstruction:
                    string cBits = parsedLine.Comp.Value.GetAttribute<AssemblerMappingAttribute, Comp>().Binary;
                    string dBits = parsedLine.Dest.Value.GetAttribute<AssemblerMappingAttribute, Dest>().Binary;
                    string jBits = parsedLine.Jump.Value.GetAttribute<AssemblerMappingAttribute, Jump>().Binary;
                    return $"111{cBits}{dBits}{jBits}";
                default:
                    return null;
            }
        }
    }
}
