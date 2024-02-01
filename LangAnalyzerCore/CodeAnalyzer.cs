using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangAnalyzerCore
{
    public class CodeAnalyzer
    {
        public AstNode Analyze(string program)
        {
            var lexicalAnalyzer = new LexicalAnalyzer(program);
            var lexemes = lexicalAnalyzer.Analyze();

            var syntaxisAnalyzer = new IfSyntaxisAnalyzer();
            return syntaxisAnalyzer.Analyze(lexemes);
        }
    }
}
