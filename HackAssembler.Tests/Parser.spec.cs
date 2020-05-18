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
    }
}
