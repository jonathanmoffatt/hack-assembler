using System;
namespace HackAssembler.Core
{
    public class AssemblerMappingAttribute: Attribute
    {
        public string Assembler { get; }
        public string Binary { get; }

        public AssemblerMappingAttribute(string assembler, string binary)
        {
            Assembler = assembler;
            Binary = binary;
        }
    }
}
