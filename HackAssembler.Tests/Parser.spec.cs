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
        public void ShouldSetCorrectJumpDetailsWithoutDest()
        {
            ParseResult result = parser.Parse("D&M;JGE");
            result.Comp.Should().Be(Comp.DAndM);
            result.Type.Should().Be(ParsedType.CInstruction);
            result.Dest.Should().BeNull();
            result.Jump.Should().Be(Jump.JGE);
        }

        [TestMethod]
        public void ShouldSetCorrectJumpDetailsWithDest()
        {
            ParseResult result = parser.Parse("AMD=D|A;JEQ");
            result.Comp.Should().Be(Comp.DOrA);
            result.Type.Should().Be(ParsedType.CInstruction);
            result.Dest.Should().Be(Dest.AMD);
            result.Jump.Should().Be(Jump.JEQ);
        }

        [TestMethod]
        public void ShouldSetLabelDetails()
        {
            ParseResult result = parser.Parse("(LOOP)");
            result.Type.Should().Be(ParsedType.Label);
            result.Label.Should().Be("LOOP");
        }

        [TestMethod]
        public void ShouldTrimLabel()
        {
            ParseResult result = parser.Parse("(  LOOP  )");
            result.Type.Should().Be(ParsedType.Label);
            result.Label.Should().Be("LOOP");
        }

        [TestMethod]
        public void ShouldRaiseErrorForInvalidDest()
        {
            ParseResult result = parser.Parse("Z=M+1");
            result.Type.Should().Be(ParsedType.Invalid);
            result.Error.Should().Be("Requested value 'Z' was not found.");
        }

        [TestMethod]
        public void ShouldRaiseErrorForInvalidJump()
        {
            ParseResult result = parser.Parse("A=M+1;JUMP");
            result.Type.Should().Be(ParsedType.Invalid);
            result.Error.Should().Be("Requested value 'JUMP' was not found.");
        }

        [TestMethod]
        public void ShouldRaiseErrorForInvalidComp()
        {
            ParseResult result = parser.Parse("M=Z+1");
            result.Type.Should().Be(ParsedType.Invalid);
            result.Error.Should().Be("The given key 'Z+1' was not present in the dictionary.");
        }

        [TestMethod]
        public void ShouldRaiseErrorForInvalidLabel()
        {
            ParseResult result = parser.Parse("(SOMETHING");
            result.Type.Should().Be(ParsedType.Invalid);
            result.Error.Should().Be("The given key '(SOMETHING' was not present in the dictionary.");
        }

        [TestMethod]
        public void ShouldRaiseErrorForEmptyLabel()
        {
            ParseResult result = parser.Parse("()");
            result.Type.Should().Be(ParsedType.Invalid);
            result.Error.Should().Be("Empty labels are not permitted.");
        }

        [TestMethod]
        public void ShouldRaiseErrorForEmptyAddress()
        {
            ParseResult result = parser.Parse("@");
            result.Type.Should().Be(ParsedType.Invalid);
            result.Error.Should().Be("Empty addresses are not permitted.");
        }
    }
    #endregion

    #region WhenBuildingASymbolTable
    [TestClass]
    public class WhenBuildingASymbolTAble
    {
        private Parser parser;
        private ParseResult result1 = new ParseResult { Comp = Comp.AMinusD };
        private ParseResult result2 = new ParseResult { Comp = Comp.MPlusOne };
        private ParseResult result3 = new ParseResult { Comp = Comp.DAndM };

        [TestInitialize]
        public void Setup()
        {
            parser = new Parser();
        }

        [TestMethod]
        public void ShouldSetAddressOfLabels()
        {
            ParseResult label = new ParseResult { Type = ParsedType.Label, Label = "LOOP" };
            Dictionary<string, int> dict = parser.BuildSymbolTable(result1, result2, label, result3);
            dict.Should().HaveCount(1);
            dict.Should().ContainKey("LOOP");
            dict["LOOP"].Should().Be(2);
        }

        [TestMethod]
        public void ShouldSetAddressOfVariables()
        {
            ParseResult variable = new ParseResult { Type = ParsedType.AInstruction, AddressSymbol = "counter" };
            Dictionary<string, int> dict = parser.BuildSymbolTable(result1, result2, variable, result3);
            dict.Should().HaveCount(1);
            dict.Should().ContainKey("counter");
            dict["counter"].Should().Be(16);
        }

        [TestMethod]
        public void ShouldSetAddressOfMultipleVariables()
        {
            ParseResult variable1 = new ParseResult { Type = ParsedType.AInstruction, AddressSymbol = "counter" };
            ParseResult variable2 = new ParseResult { Type = ParsedType.AInstruction, AddressSymbol = "temp" };
            Dictionary<string, int> dict = parser.BuildSymbolTable(result1, result2, variable1, variable2, result3);
            dict.Should().HaveCount(2);
            dict.Should().ContainKey("counter").And.ContainKey("temp");
            dict["counter"].Should().Be(16);
            dict["temp"].Should().Be(17);
        }

        [TestMethod]
        public void ShouldUsePreviouslyAssignedAddressInSubsequentVariableReferences()
        {
            ParseResult variable1 = new ParseResult { Type = ParsedType.AInstruction, AddressSymbol = "counter" };
            ParseResult variable2 = new ParseResult { Type = ParsedType.AInstruction, AddressSymbol = "temp" };
            ParseResult variable3 = new ParseResult { Type = ParsedType.AInstruction, AddressSymbol = "i" };
            Dictionary<string, int> dict = parser.BuildSymbolTable(variable1, variable2, result3, variable1, variable3);
            dict.Should().HaveCount(3);
            dict.Should().ContainKey("counter").And.ContainKey("temp").And.ContainKey("i");
            dict["counter"].Should().Be(16);
            dict["temp"].Should().Be(17);
            dict["i"].Should().Be(18);
        }

        [TestMethod]
        public void ShouldIgnoreAInstructionsWithANumericAddress()
        {
            ParseResult variable1 = new ParseResult { Type = ParsedType.AInstruction, Address = 123 };
            ParseResult variable2 = new ParseResult { Type = ParsedType.AInstruction, AddressSymbol = "temp" };
            Dictionary<string, int> dict = parser.BuildSymbolTable(variable1, variable2);
            dict.Should().HaveCount(1);
        }

        [TestMethod]
        public void ShouldMarkAsErrorIfThereAreDuplicateLabels()
        {
            ParseResult label1 = new ParseResult { Type = ParsedType.Label, Label = "LOOP" };
            ParseResult label2 = new ParseResult { Type = ParsedType.Label, Label = "LOOP" };
            Dictionary<string, int> dict = parser.BuildSymbolTable(result1, result2, label1, result3, label2);
            label2.Type.Should().Be(ParsedType.Invalid);
            label2.Error.Should().Be("Duplicated label.");
        }
    }
    #endregion
}
