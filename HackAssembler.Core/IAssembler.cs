using System;
using System.Collections.Generic;

namespace HackAssembler.Core
{
    public interface IAssembler
    {
        string ConvertToBinary(ParsedLine parsedLine, Dictionary<string, int> symbolTable);
    }
}
