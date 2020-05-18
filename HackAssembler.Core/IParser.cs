using System;
namespace HackAssembler.Core
{
    public interface IParser
    {
        ParseResult Parse(string line);
    }
}
