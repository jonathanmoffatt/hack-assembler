namespace HackAssembler.Core
{
    public class ParsedLine
    {
        public ParsedType Type { get; set; }
        public int? Address { get; set; }
        public string AddressSymbol { get; set; }
        public string Label { get; set; }
        public Dest? Dest { get; set; }
        public Comp? Comp { get; set; }
        public Jump? Jump { get; set; }

        public string Error { get; set; }
    }
}
