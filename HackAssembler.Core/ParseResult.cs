namespace HackAssembler.Core
{
    public class ParseResult
    {
        public ParsedType Type { get; set; }
        public int? Address { get; set; }
        public string AddressSymbol { get; set; }
    }
}
