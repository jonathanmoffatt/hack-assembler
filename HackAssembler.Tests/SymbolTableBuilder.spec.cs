using System.Collections.Generic;
using FluentAssertions;
using HackAssembler.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HackAssembler.Tests
{
    #region WhenBuildingASymbolTable
    [TestClass]
    public class WhenBuildingASymbolTAble
    {
        private SymbolTableBuilder symbolTableBuilder;
        private readonly ParsedLine result1 = new ParsedLine { Comp = Comp.AMinusD };
        private readonly ParsedLine result2 = new ParsedLine { Comp = Comp.MPlusOne };
        private readonly ParsedLine result3 = new ParsedLine { Comp = Comp.DAndM };

        [TestInitialize]
        public void Setup()
        {
            symbolTableBuilder = new SymbolTableBuilder();
        }

        [TestMethod]
        public void ShouldPopulateWithBuiltInRegisters()
        {
            Dictionary<string, int> table = symbolTableBuilder.BuildSymbolTable();
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
            Dictionary<string, int> table = symbolTableBuilder.BuildSymbolTable();
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
            ParsedLine label = new ParsedLine { Type = ParsedType.Label, Label = "LOOP" };
            Dictionary<string, int> dict = symbolTableBuilder.BuildSymbolTable(result1, result2, label, result3);
            dict.Should().ContainKey("LOOP");
            dict["LOOP"].Should().Be(2);
        }

        [TestMethod]
        public void ShouldSetAddressOfVariables()
        {
            ParsedLine variable = new ParsedLine { Type = ParsedType.AInstruction, AddressSymbol = "counter" };
            Dictionary<string, int> dict = symbolTableBuilder.BuildSymbolTable(result1, result2, variable, result3);
            dict.Should().ContainKey("counter");
            dict["counter"].Should().Be(16);
        }

        [TestMethod]
        public void ShouldSetAddressOfMultipleVariables()
        {
            ParsedLine variable1 = new ParsedLine { Type = ParsedType.AInstruction, AddressSymbol = "counter" };
            ParsedLine variable2 = new ParsedLine { Type = ParsedType.AInstruction, AddressSymbol = "temp" };
            Dictionary<string, int> dict = symbolTableBuilder.BuildSymbolTable(result1, result2, variable1, variable2, result3);
            dict.Should().ContainKey("counter").And.ContainKey("temp");
            dict["counter"].Should().Be(16);
            dict["temp"].Should().Be(17);
        }

        [TestMethod]
        public void ShouldUsePreviouslyAssignedAddressInSubsequentVariableReferences()
        {
            ParsedLine variable1 = new ParsedLine { Type = ParsedType.AInstruction, AddressSymbol = "counter" };
            ParsedLine variable2 = new ParsedLine { Type = ParsedType.AInstruction, AddressSymbol = "temp" };
            ParsedLine variable3 = new ParsedLine { Type = ParsedType.AInstruction, AddressSymbol = "i" };
            Dictionary<string, int> dict = symbolTableBuilder.BuildSymbolTable(variable1, variable2, result3, variable1, variable3);
            dict.Should().ContainKey("counter").And.ContainKey("temp").And.ContainKey("i");
            dict["counter"].Should().Be(16);
            dict["temp"].Should().Be(17);
            dict["i"].Should().Be(18);
        }

        [TestMethod]
        public void ShouldIgnoreAInstructionsWithANumericAddress()
        {
            ParsedLine variable1 = new ParsedLine { Type = ParsedType.AInstruction, Address = 123 };
            ParsedLine variable2 = new ParsedLine { Type = ParsedType.AInstruction, AddressSymbol = "temp" };
            Dictionary<string, int> dict = symbolTableBuilder.BuildSymbolTable(variable1, variable2);
            dict.Should().NotContainKey("123");
        }

        [TestMethod]
        public void ShouldMarkAsErrorIfThereAreDuplicateLabels()
        {
            ParsedLine label1 = new ParsedLine { Type = ParsedType.Label, Label = "LOOP" };
            ParsedLine label2 = new ParsedLine { Type = ParsedType.Label, Label = "LOOP" };
            symbolTableBuilder.BuildSymbolTable(result1, result2, label1, result3, label2);
            label2.Type.Should().Be(ParsedType.Invalid);
            label2.Error.Should().Be("Duplicated label.");
        }
    }
    #endregion
}
