namespace HackAssembler.Core
{
    public class Parser: IParser
    {
        public ParseResult Parse(string line)
        {
            ParseResult result = new ParseResult();
            line = StripComments(line);
            if (line.StartsWith('@'))
            {
                result.Type = ParsedType.AInstruction;
                line = line.Substring(1);
                if (int.TryParse(line, out int addr))
                    result.Address = addr;
                else
                    result.AddressSymbol = line;
            }
            else
                result.Type = ParsedType.Whitespace;
            return result;
        }

        private string StripComments(string line)
        {
            return line.Trim().Split("//")[0].Trim();
        }
    }
}
