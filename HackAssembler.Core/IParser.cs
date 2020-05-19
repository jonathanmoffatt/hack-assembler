using System.Collections.Generic;

namespace HackAssembler.Core
{
    public interface IParser
    {
        ParseResult Parse(string line);
        Dictionary<string, int> BuildSymbolTable(params ParseResult[] parsedLines);
    }
}
