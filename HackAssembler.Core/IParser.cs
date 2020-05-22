using System.Collections.Generic;

namespace HackAssembler.Core
{
    public interface IParser
    {
        ParsedLine Parse(string line);
    }
}
