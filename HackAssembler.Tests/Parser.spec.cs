using System.Collections.Generic;
using FluentAssertions;
using HackAssembler.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HackAssembler.Tests
{
    #region WhenParsingLinesOfAssemblyLanguage
    [TestClass]
    public class WhenParsingLinesOfAssemblyLanguage
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

        [TestMethod]
        public void ShouldSetNoJumpIfThereIsNoJumpSpecified()
        {
            parser.Parse("M=D+1").Jump.Should().Be(Jump.NoJump);
        }

        [TestMethod]
        public void ShouldSetDestToNotStoredIfThereIsNoDestSpecified()
        {
            parser.Parse("D+1;JEQ").Dest.Should().Be(Dest.NotStored);
        }

        [TestMethod]
        public void ShouldSetCorrectJumpDetailsWithoutDest()
        {
            ParsedLine result = parser.Parse("D&M;JGE");
            result.Comp.Should().Be(Comp.DAndM);
            result.Type.Should().Be(ParsedType.CInstruction);
            result.Dest.Should().Be(Dest.NotStored);
            result.Jump.Should().Be(Jump.JGE);
        }

        [TestMethod]
        public void ShouldSetCorrectJumpDetailsWithDest()
        {
            ParsedLine result = parser.Parse("AMD=D|A;JEQ");
            result.Comp.Should().Be(Comp.DOrA);
            result.Type.Should().Be(ParsedType.CInstruction);
            result.Dest.Should().Be(Dest.AMD);
            result.Jump.Should().Be(Jump.JEQ);
        }

        [TestMethod]
        public void ShouldSetLabelDetails()
        {
            ParsedLine result = parser.Parse("(LOOP)");
            result.Type.Should().Be(ParsedType.Label);
            result.Label.Should().Be("LOOP");
        }

        [TestMethod]
        public void ShouldTrimLabel()
        {
            ParsedLine result = parser.Parse("(  LOOP  )");
            result.Type.Should().Be(ParsedType.Label);
            result.Label.Should().Be("LOOP");
        }

        [TestMethod]
        public void ShouldRaiseErrorForInvalidDest()
        {
            ParsedLine result = parser.Parse("Z=M+1");
            result.Type.Should().Be(ParsedType.Invalid);
            result.Error.Should().Be("Requested value 'Z' was not found.");
        }

        [TestMethod]
        public void ShouldRaiseErrorForInvalidJump()
        {
            ParsedLine result = parser.Parse("A=M+1;JUMP");
            result.Type.Should().Be(ParsedType.Invalid);
            result.Error.Should().Be("Requested value 'JUMP' was not found.");
        }

        [TestMethod]
        public void ShouldRaiseErrorForInvalidComp()
        {
            ParsedLine result = parser.Parse("M=Z+1");
            result.Type.Should().Be(ParsedType.Invalid);
            result.Error.Should().Be("The given key 'Z+1' was not present in the dictionary.");
        }

        [TestMethod]
        public void ShouldRaiseErrorForInvalidLabel()
        {
            ParsedLine result = parser.Parse("(SOMETHING");
            result.Type.Should().Be(ParsedType.Invalid);
            result.Error.Should().Be("The given key '(SOMETHING' was not present in the dictionary.");
        }

        [TestMethod]
        public void ShouldRaiseErrorForEmptyLabel()
        {
            ParsedLine result = parser.Parse("()");
            result.Type.Should().Be(ParsedType.Invalid);
            result.Error.Should().Be("Empty labels are not permitted.");
        }

        [TestMethod]
        public void ShouldRaiseErrorForEmptyAddress()
        {
            ParsedLine result = parser.Parse("@");
            result.Type.Should().Be(ParsedType.Invalid);
            result.Error.Should().Be("Empty addresses are not permitted.");
        }
    }
    #endregion
}
