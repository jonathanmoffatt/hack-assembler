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
        public void ShouldPopulateWithBuiltInRegisters()
        {
            Dictionary<string, int> table = parser.BuildSymbolTable();
            table["R0"].Should().Be(0);
            table["R1"].Should().Be(1);
            table["R2"].Should().Be(2);
            table["R3"].Should().Be(3);
            table["R4"].Should().Be(4);
            table["R5"].Should().Be(5);
            table["R6"].Should().Be(6);
            table["R7"].Should().Be(7);
            table["R8"].Should().Be(8);
            table["R9"].Should().Be(9);
            table["R10"].Should().Be(10);
            table["R11"].Should().Be(11);
            table["R12"].Should().Be(12);
            table["R13"].Should().Be(13);
            table["R14"].Should().Be(14);
            table["R15"].Should().Be(15);
        }

        [TestMethod]
        public void ShouldPopulateWithOtherBuiltInSymbols()
        {
            Dictionary<string, int> table = parser.BuildSymbolTable();
            table["SCREEN"].Should().Be(16384);
            table["KBD"].Should().Be(24576);
            table["SP"].Should().Be(0);
            table["LCL"].Should().Be(1);
            table["ARG"].Should().Be(2);
            table["THIS"].Should().Be(3);
            table["THAT"].Should().Be(4);
        }

        [TestMethod]
        public void ShouldSetAddressOfLabels()
        {
            ParseResult label = new ParseResult { Type = ParsedType.Label, Label = "LOOP" };
            Dictionary<string, int> dict = parser.BuildSymbolTable(result1, result2, label, result3);
            dict.Should().ContainKey("LOOP");
            dict["LOOP"].Should().Be(2);
        }

        [TestMethod]
        public void ShouldSetAddressOfVariables()
        {
            ParseResult variable = new ParseResult { Type = ParsedType.AInstruction, AddressSymbol = "counter" };
            Dictionary<string, int> dict = parser.BuildSymbolTable(result1, result2, variable, result3);
            dict.Should().ContainKey("counter");
            dict["counter"].Should().Be(16);
        }

        [TestMethod]
        public void ShouldSetAddressOfMultipleVariables()
        {
            ParseResult variable1 = new ParseResult { Type = ParsedType.AInstruction, AddressSymbol = "counter" };
            ParseResult variable2 = new ParseResult { Type = ParsedType.AInstruction, AddressSymbol = "temp" };
            Dictionary<string, int> dict = parser.BuildSymbolTable(result1, result2, variable1, variable2, result3);
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
            dict.Should().NotContainKey("123");
        }

        [TestMethod]
        public void ShouldMarkAsErrorIfThereAreDuplicateLabels()
        {
            ParseResult label1 = new ParseResult { Type = ParsedType.Label, Label = "LOOP" };
            ParseResult label2 = new ParseResult { Type = ParsedType.Label, Label = "LOOP" };
            parser.BuildSymbolTable(result1, result2, label1, result3, label2);
            label2.Type.Should().Be(ParsedType.Invalid);
            label2.Error.Should().Be("Duplicated label.");
        }
    }
    #endregion
}
