namespace HackAssembler.Core
{
    public enum Jump
    {
        [AssemblerMapping(null, "000")]
        NoJump,
        [AssemblerMapping("JGT", "001")]
        JGT,
        [AssemblerMapping("JEQ", "010")]
        JEQ,
        [AssemblerMapping("JGE", "011")]
        JGE,
        [AssemblerMapping("JLT", "100")]
        JLT,
        [AssemblerMapping("JNE", "101")]
        JNE,
        [AssemblerMapping("JLE", "110")]
        JLE,
        [AssemblerMapping("JMP", "111")]
        JMP
    }
}
