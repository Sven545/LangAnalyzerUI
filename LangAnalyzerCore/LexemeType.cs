using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangAnalyzerCore
{
    public enum LexemeType
    {
        Keyword,
        LogicOperator,
        Identifier,
        Assignment,
        Delimiter,
        Comparsion,
        Number,
        Arithmetic
    }
}
