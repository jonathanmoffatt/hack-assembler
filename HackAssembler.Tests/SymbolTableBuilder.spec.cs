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
        private readonly LineOfCode line1 = new LineOfCode { Type = InstructionType.CInstruction, Comp = Comp.AMinusD };
        private readonly LineOfCode line2 = new LineOfCode { Type = InstructionType.CInstruction, Comp = Comp.MPlusOne };
        private readonly LineOfCode line3 = new LineOfCode { Type = InstructionType.CInstruction, Comp = Comp.DAndM };

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
            LineOfCode label = new LineOfCode { Type = InstructionType.Label, Label = "LOOP" };
            Dictionary<string, int> dict = symbolTableBuilder.BuildSymbolTable(line1, line2, label, line3);
            dict.Should().ContainKey("LOOP");
            dict["LOOP"].Should().Be(2);
        }

        [TestMethod]
        public void ShouldSetAddressOfVariables()
        {
            LineOfCode variable = new LineOfCode { Type = InstructionType.AInstruction, AddressSymbol = "counter" };
            Dictionary<string, int> dict = symbolTableBuilder.BuildSymbolTable(line1, line2, variable, line3);
            dict.Should().ContainKey("counter");
            dict["counter"].Should().Be(16);
        }

        [TestMethod]
        public void ShouldSetAddressOfMultipleVariables()
        {
            LineOfCode variable1 = new LineOfCode { Type = InstructionType.AInstruction, AddressSymbol = "counter" };
            LineOfCode variable2 = new LineOfCode { Type = InstructionType.AInstruction, AddressSymbol = "temp" };
            Dictionary<string, int> dict = symbolTableBuilder.BuildSymbolTable(line1, line2, variable1, variable2, line3);
            dict.Should().ContainKey("counter").And.ContainKey("temp");
            dict["counter"].Should().Be(16);
            dict["temp"].Should().Be(17);
        }

        [TestMethod]
        public void ShouldUsePreviouslyAssignedAddressInSubsequentVariableReferences()
        {
            LineOfCode variable1 = new LineOfCode { Type = InstructionType.AInstruction, AddressSymbol = "counter" };
            LineOfCode variable2 = new LineOfCode { Type = InstructionType.AInstruction, AddressSymbol = "temp" };
            LineOfCode variable3 = new LineOfCode { Type = InstructionType.AInstruction, AddressSymbol = "i" };
            Dictionary<string, int> dict = symbolTableBuilder.BuildSymbolTable(variable1, variable2, line3, variable1, variable3);
            dict.Should().ContainKey("counter").And.ContainKey("temp").And.ContainKey("i");
            dict["counter"].Should().Be(16);
            dict["temp"].Should().Be(17);
            dict["i"].Should().Be(18);
        }

        [TestMethod]
        public void ShouldNotIncrementProgramCounterForLabelsAndWhitespace()
        {
            LineOfCode whitespace = new LineOfCode { Type = InstructionType.Whitespace };
            LineOfCode comp1 = new LineOfCode { Type = InstructionType.CInstruction };
            LineOfCode comp2 = new LineOfCode { Type = InstructionType.CInstruction };
            LineOfCode label1 = new LineOfCode { Type = InstructionType.Label, Label = "LOOP"};
            LineOfCode comp3 = new LineOfCode { Type = InstructionType.CInstruction };
            LineOfCode comp4 = new LineOfCode { Type = InstructionType.CInstruction };
            LineOfCode label2 = new LineOfCode { Type = InstructionType.Label, Label = "FINISH"};
            LineOfCode comp5 = new LineOfCode { Type = InstructionType.CInstruction };
            Dictionary<string, int> dict = symbolTableBuilder.BuildSymbolTable(whitespace, comp1, comp2, whitespace, label1, comp3, comp4, whitespace, whitespace, label2, comp5);
            dict["LOOP"].Should().Be(2);
            dict["FINISH"].Should().Be(4);
        }

        [TestMethod]
        public void ShouldDistinguishBetweenVariablesAndLabels()
        {
            LineOfCode referenceToVariable = new LineOfCode { Type = InstructionType.AInstruction, AddressSymbol = "counter" };
            LineOfCode comp1 = new LineOfCode { Type = InstructionType.CInstruction };
            LineOfCode comp2 = new LineOfCode { Type = InstructionType.CInstruction };
            LineOfCode referenceToLabel = new LineOfCode { Type = InstructionType.AInstruction, AddressSymbol = "FINISH" };
            LineOfCode comp3 = new LineOfCode { Type = InstructionType.CInstruction };
            LineOfCode label2 = new LineOfCode { Type = InstructionType.Label, Label = "FINISH"};
            LineOfCode comp4 = new LineOfCode { Type = InstructionType.CInstruction };
            Dictionary<string, int> dict = symbolTableBuilder.BuildSymbolTable(referenceToVariable, comp1, comp2, referenceToLabel, comp3, label2, comp4);
            dict["counter"].Should().Be(16);
            dict["FINISH"].Should().Be(5);
        }

        [TestMethod]
        public void ShouldIgnoreAInstructionsWithANumericAddress()
        {
            LineOfCode variable1 = new LineOfCode { Type = InstructionType.AInstruction, Address = 123 };
            LineOfCode variable2 = new LineOfCode { Type = InstructionType.AInstruction, AddressSymbol = "temp" };
            Dictionary<string, int> dict = symbolTableBuilder.BuildSymbolTable(variable1, variable2);
            dict.Should().NotContainKey("123");
        }

        [TestMethod]
        public void ShouldMarkAsErrorIfThereAreDuplicateLabels()
        {
            LineOfCode label1 = new LineOfCode { Type = InstructionType.Label, Label = "LOOP" };
            LineOfCode label2 = new LineOfCode { Type = InstructionType.Label, Label = "LOOP" };
            symbolTableBuilder.BuildSymbolTable(line1, line2, label1, line3, label2);
            label2.Type.Should().Be(InstructionType.Invalid);
            label2.Error.Should().Be("Duplicated label.");
        }
    }
    #endregion
}
