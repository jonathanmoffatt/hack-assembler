using System.Collections.Generic;
using FluentAssertions;
using HackAssembler.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HackAssembler.Tests
{
    [TestClass]
	public class WhenAssemblingBinary
	{
        private Assembler classUnderTest;
        private Dictionary<string, int> symbolTable;

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new Assembler();
            symbolTable = new Dictionary<string, int>
            {
                {"counter", 18 }
            };
        }

        [TestMethod]
        public void ShouldOutputNullForWhiteSpace()
        {
            ParsedLine line = new ParsedLine { Type = ParsedType.Whitespace };
            classUnderTest.ConvertToBinary(line, symbolTable).Should().BeNull();
        }

        [TestMethod]
        public void ShouldOutputNullForLabels()
        {
            ParsedLine line = new ParsedLine { Type = ParsedType.Label };
            classUnderTest.ConvertToBinary(line, symbolTable).Should().BeNull();
        }

        [TestMethod]
        public void ShouldOutputCorrectBinaryForAddresses()
        {
            ParsedLine line = new ParsedLine { Type = ParsedType.AInstruction, Address = 5 };
            classUnderTest.ConvertToBinary(line, symbolTable).Should().Be("0000000000000101");
        }

        [TestMethod]
        public void ShouldOutputCorrectBinaryForAddressesExpressedAsSymbols()
        {
            ParsedLine line = new ParsedLine { Type = ParsedType.AInstruction, AddressSymbol = "counter" };
            classUnderTest.ConvertToBinary(line, symbolTable).Should().Be("0000000000010010");
        }

        [TestMethod]
        public void ShouldOutputCorrectBinaryForCInstructions()
        {
            ParsedLine line = new ParsedLine {
                Type = ParsedType.CInstruction,
                Dest = Dest.AM,
                Comp = Comp.DMinusA,
                Jump = Jump.JEQ
            };
            classUnderTest.ConvertToBinary(line, symbolTable).Should().Be("1110010011101010");
        }
	}
}
