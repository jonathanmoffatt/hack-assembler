using FluentAssertions;
using HackAssembler.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HackAssembler.Tests
{
    [TestClass]
    public class WhenParsing
    {
        private Parser parser;

        [TestInitialize]
        public void Setup()
        {
            parser = new Parser();
        }

        [TestMethod]
        public void ShouldIdentifyEmptyLineAsWhitespace()
        {
            parser.Parse("").Type.Should().Be(ParsedType.Whitespace);
        }

        [TestMethod]
        public void ShouldIdentifyLineOfSpacesAndTabsAsWhitespace()
        {
            parser.Parse("   \t\t  \t").Type.Should().Be(ParsedType.Whitespace);
        }

        [TestMethod]
        public void ShouldIdentifyCommentsAsWhitespace()
        {
            parser.Parse("// stuff n things").Type.Should().Be(ParsedType.Whitespace);
        }

        [TestMethod]
        public void ShouldIdentifyAInstruction()
        {
            parser.Parse("@1003").Type.Should().Be(ParsedType.AInstruction);
        }

        [TestMethod]
        public void ShouldIdentifyIndentedAInstruction()
        {
            parser.Parse("    @1003").Type.Should().Be(ParsedType.AInstruction);
        }

        [TestMethod]
        public void ShouldIdentifyIndentedByTabsAInstruction()
        {
            parser.Parse("\t\t@1003").Type.Should().Be(ParsedType.AInstruction);
        }

        [TestMethod]
        public void ShouldSetAddressForAInstruction()
        {
            parser.Parse("@1003").Address.Should().Be(1003);
        }

        [TestMethod]
        public void ShouldAllowCommentsOnSameLineAsAInstruction()
        {
            parser.Parse("@1003 // description goes here").Address.Should().Be(1003);
        }

        [TestMethod]
        public void ShouldSetAddressSymbolForAInstruction()
        {
            var result = parser.Parse("@counter");
            result.Address.Should().BeNull();
            result.AddressSymbol.Should().Be("counter");
        }

        [TestMethod]
        public void ShouldRemoveWhitespaceFromAddressSymbol()
        {
            var result = parser.Parse("@counter  // stuff");
            result.AddressSymbol.Should().Be("counter");
        }

        [TestMethod]
        public void ShouldIdentifyCInstruction()
        {
            parser.Parse("M=D+1").Type.Should().Be(ParsedType.CInstruction);
        }

        [TestMethod]
        public void ShouldIdentifyDestOfCInstruction()
        {
            parser.Parse("M=D+1").Dest.Should().Be(Dest.M);
        }

        [TestMethod]
        public void ShouldIdentifyCompOfCInstruction()
        {
            parser.Parse("M=D+1").Comp.Should().Be(Comp.DPlusOne);
        }
    }
}
