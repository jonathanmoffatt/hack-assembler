namespace HackAssembler.Core
{
    public enum Comp
    {
        [AssemblerMapping("0", "0101010")]
        Zero,
        [AssemblerMapping("1", "0111111")]
        One,
        [AssemblerMapping("-1", "0111010")]
        MinusOne,
        [AssemblerMapping("D", "0001100")]
        D,
        [AssemblerMapping("A", "0110000")]
        A,
        [AssemblerMapping("!D", "0001111")]
        NotD,
        [AssemblerMapping("!A", "0110001")]
        NotA,
        [AssemblerMapping("-D", "0001111")]
        MinusD,
        [AssemblerMapping("-A", "0110011")]
        MinusA,
        [AssemblerMapping("D+1", "0011111")]
        DPlusOne,
        [AssemblerMapping("A+1", "0110111")]
        APlusOne,
        [AssemblerMapping("D-1", "0001110")]
        DMinusOne,
        [AssemblerMapping("A-1", "0110010")]
        AMinusOne,
        [AssemblerMapping("D+A", "0000010")]
        DPlusA,
        [AssemblerMapping("D-A", "0010011")]
        DMinusA,
        [AssemblerMapping("A-D", "0000111")]
        AMinusD,
        [AssemblerMapping("D&A", "0000000")]
        DAndA,
        [AssemblerMapping("D|A", "0010101")]
        DOrA,
        [AssemblerMapping("M", "1110000")]
        M,
        [AssemblerMapping("!M", "1110001")]
        NotM,
        [AssemblerMapping("-M", "1110011")]
        MinusM,
        [AssemblerMapping("M+1", "1110111")]
        MPlusOne,
        [AssemblerMapping("M-1", "1110010")]
        MMinusOne,
        [AssemblerMapping("D+M", "1000010")]
        DPlusM,
        [AssemblerMapping("D-M", "1010011")]
        DMinusM,
        [AssemblerMapping("M-D", "1000111")]
        MMinusD,
        [AssemblerMapping("D&M", "1000000")]
        DAndM,
        [AssemblerMapping("D|M", "1010101")]
        DOrM
    }
}
