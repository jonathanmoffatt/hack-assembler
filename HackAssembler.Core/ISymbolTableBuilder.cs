using System.Collections.Generic;

namespace HackAssembler.Core
{
    public interface ISymbolTableBuilder
    {
        Dictionary<string, int> BuildSymbolTable(params ParsedLine[] parsedLines);
    }
}