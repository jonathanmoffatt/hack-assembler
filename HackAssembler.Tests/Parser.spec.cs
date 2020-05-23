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
            parser.Parse("").Type.Should().Be(InstructionType.Whitespace);
        }

        [TestMethod]
        public void ShouldIdentifyLineOfSpacesAndTabsAsWhitespace()
        {
            parser.Parse("   \t\t  \t").Type.Should().Be(InstructionType.Whitespace);
        }

        [TestMethod]
        public void ShouldIdentifyCommentsAsWhitespace()
        {
            parser.Parse("// stuff n things").Type.Should().Be(InstructionType.Whitespace);
        }

        [TestMethod]
        public void ShouldIdentifyAInstruction()
        {
            parser.Parse("@1003").Type.Should().Be(InstructionType.AInstruction);
        }

        [TestMethod]
        public void ShouldIdentifyIndentedAInstruction()
        {
            parser.Parse("    @1003").Type.Should().Be(InstructionType.AInstruction);
        }

        [TestMethod]
        public void ShouldIdentifyIndentedByTabsAInstruction()
        {
            parser.Parse("\t\t@1003").Type.Should().Be(InstructionType.AInstruction);
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
            parser.Parse("M=D+1").Type.Should().Be(InstructionType.CInstruction);
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
            LineOfCode result = parser.Parse("D&M;JGE");
            result.Comp.Should().Be(Comp.DAndM);
            result.Type.Should().Be(InstructionType.CInstruction);
            result.Dest.Should().Be(Dest.NotStored);
            result.Jump.Should().Be(Jump.JGE);
        }

        [TestMethod]
        public void ShouldSetCorrectJumpDetailsWithDest()
        {
            LineOfCode result = parser.Parse("AMD=D|A;JEQ");
            result.Comp.Should().Be(Comp.DOrA);
            result.Type.Should().Be(InstructionType.CInstruction);
            result.Dest.Should().Be(Dest.AMD);
            result.Jump.Should().Be(Jump.JEQ);
        }

        [TestMethod]
        public void ShouldSetLabelDetails()
        {
            LineOfCode result = parser.Parse("(LOOP)");
            result.Type.Should().Be(InstructionType.Label);
            result.Label.Should().Be("LOOP");
        }

        [TestMethod]
        public void ShouldTrimLabel()
        {
            LineOfCode result = parser.Parse("(  LOOP  )");
            result.Type.Should().Be(InstructionType.Label);
            result.Label.Should().Be("LOOP");
        }

        [TestMethod]
        public void ShouldRaiseErrorForInvalidDest()
        {
            LineOfCode result = parser.Parse("Z=M+1");
            result.Type.Should().Be(InstructionType.Invalid);
            result.Error.Should().Be("Requested value 'Z' was not found.");
        }

        [TestMethod]
        public void ShouldRaiseErrorForInvalidJump()
        {
            LineOfCode result = parser.Parse("A=M+1;JUMP");
            result.Type.Should().Be(InstructionType.Invalid);
            result.Error.Should().Be("Requested value 'JUMP' was not found.");
        }

        [TestMethod]
        public void ShouldRaiseErrorForInvalidComp()
        {
            LineOfCode result = parser.Parse("M=Z+1");
            result.Type.Should().Be(InstructionType.Invalid);
            result.Error.Should().Be("The given key 'Z+1' was not present in the dictionary.");
        }

        [TestMethod]
        public void ShouldRaiseErrorForInvalidLabel()
        {
            LineOfCode result = parser.Parse("(SOMETHING");
            result.Type.Should().Be(InstructionType.Invalid);
            result.Error.Should().Be("The given key '(SOMETHING' was not present in the dictionary.");
        }

        [TestMethod]
        public void ShouldRaiseErrorForEmptyLabel()
        {
            LineOfCode result = parser.Parse("()");
            result.Type.Should().Be(InstructionType.Invalid);
            result.Error.Should().Be("Empty labels are not permitted.");
        }

        [TestMethod]
        public void ShouldRaiseErrorForEmptyAddress()
        {
            LineOfCode result = parser.Parse("@");
            result.Type.Should().Be(InstructionType.Invalid);
            result.Error.Should().Be("Empty addresses are not permitted.");
        }
    }
    #endregion
}
