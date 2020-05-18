using System;
using System.Collections.Generic;
using System.Linq;

namespace HackAssembler.Core
{
    public class Parser: IParser
    {
        public ParseResult Parse(string line)
        {
            ParseResult result = new ParseResult();
            var compMappings = GetCompMappings();
            line = StripComments(line);
            if (line.StartsWith('@'))
            {
                result.Type = ParsedType.AInstruction;
                line = line.Substring(1);
                if (int.TryParse(line, out int addr))
                    result.Address = addr;
                else
                    result.AddressSymbol = line;
            }
            else if (line == "")
                result.Type = ParsedType.Whitespace;
            else
            {
                result.Type = ParsedType.CInstruction;
                result.Dest = Enum.Parse<Dest>(line.Split("=")[0]);
                string rest = line.Split("=")[1];
                result.Comp = compMappings[rest];
            }
            return result;
        }

        private string StripComments(string line)
        {
            return line.Trim().Split("//")[0].Trim();
        }

        private Dictionary<string, Comp> GetCompMappings()
        {
            var result = new Dictionary<string, Comp>();
            foreach(Comp c in Enum.GetValues(typeof(Comp)))
            {
                var enumType = typeof(Comp);
                var memberInfos = enumType.GetMember(c.ToString());
                var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
                var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(AssemblerMappingAttribute), false);
                var attribute = (AssemblerMappingAttribute)valueAttributes[0];
                result.Add(attribute.Assembler, c);
            }
            return result;
        }
    }
}
