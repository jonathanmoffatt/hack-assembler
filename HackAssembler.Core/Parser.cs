using System;

namespace HackAssembler.Core
{
    public class Parser: IParser
    {
        public ParseResult Parse(string line)
        {
            return new ParseResult { Type = ParsedType.Whitespace };
        }
    }
}
