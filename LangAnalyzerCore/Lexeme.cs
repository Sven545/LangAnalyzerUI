using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangAnalyzerCore
{
    public class Lexeme
    {
        public LexemeType Type { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return $"{Type}: '{Value}'";
        }
    }
}
