namespace HackAssembler.Core
{
    public enum Dest
    {
        [AssemblerMapping(null, "000")]
        NotStored,
        [AssemblerMapping("M", "001")]
        M,
        [AssemblerMapping("D", "010")]
        D,
        [AssemblerMapping("MD", "011")]
        MD,
        [AssemblerMapping("A", "100")]
        A,
        [AssemblerMapping("AM", "101")]
        AM,
        [AssemblerMapping("AD", "110")]
        AD,
        [AssemblerMapping("AMD", "111")]
        AMD
    }
}
