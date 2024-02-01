using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangAnalyzerCore
{
    public class LexicalAnalyzer
    {
        private readonly string _program;
        private int _number = 0;

        private readonly List<string> _keywords = new List<string>() { "IF", "THEN", "ELSE", "END_IF" };
        private readonly List<string> _logicOperators = new List<string>() { "AND", "OR" };
        private readonly List<char> _delimiterSymbols = new List<char>() { '\n', '\t', ';' };
        private readonly List<string> _delimiterLexemes = new List<string>() { ";" };
        private readonly List<string> _comparsions = new List<string>() { "<", "<=", ">", ">=", "==", "<>" };
        private readonly List<string> _arithmetics = new List<string>() { "+", "-", "*", "/" };
        private readonly string _assign = ":=";

        public LexicalAnalyzer(string program)
        {
            if (string.IsNullOrEmpty(program))
                throw new Exception("Код не был получен");

            _program = program;
        }

        public IEnumerable<Lexeme> Analyze()
        {

            var lexemes = new List<Lexeme>();
            string lexeme;
            while (ReadLexeme(out lexeme))
            {
                if (IsKeyWord(lexeme))
                {
                    lexemes.Add(new Lexeme() { Type = LexemeType.Keyword, Value = lexeme });
                    continue;
                }

                if (IsLogicOperator(lexeme))
                {
                    lexemes.Add(new Lexeme() { Type = LexemeType.LogicOperator, Value = lexeme });
                    continue;
                }

                if (IsIdentifier(lexeme))
                {
                    lexemes.Add(new Lexeme() { Type = LexemeType.Identifier, Value = lexeme });
                    continue;

                }

                if (IsAssign(lexeme))
                {
                    lexemes.Add(new Lexeme() { Type = LexemeType.Assignment, Value = lexeme });
                    continue;
                }

                if (IsDelimiterLexem(lexeme))
                {
                    lexemes.Add(new Lexeme() { Type = LexemeType.Delimiter, Value = lexeme });
                    continue;
                }

                if (IsComparsion(lexeme))
                {
                    lexemes.Add(new Lexeme() { Type = LexemeType.Comparsion, Value = lexeme });
                    continue;
                }

                if (IsNumber(lexeme))
                {
                    lexemes.Add(new Lexeme() { Type = LexemeType.Number, Value = lexeme });
                    continue;
                }

                if (IsArithmetics(lexeme))
                {
                    lexemes.Add(new Lexeme() { Type = LexemeType.Arithmetic, Value = lexeme });
                    continue;
                }
            }
            _number = 0;

            return lexemes;

        }

        private bool IsKeyWord(string lexeme) => _keywords.Contains(lexeme);
        private bool IsLogicOperator(string lexeme) => _logicOperators.Contains(lexeme);
        private bool IsNumber(string lexeme)
        {
            int result;
            return int.TryParse(lexeme, out result);
        }
        private bool IsDelimiterSymbol(char symbol)
        {
            if (char.IsWhiteSpace(symbol) || _delimiterSymbols.Contains(symbol))
                return true;
            return false;
        }
        private bool IsDelimiterLexem(string symbol) => _delimiterLexemes.Contains(symbol);
        private bool IsComparsion(string lexeme) => _comparsions.Contains(lexeme);
        private bool IsArithmetics(string lexeme) => _arithmetics.Contains(lexeme);
        private bool IsAssign(string lexeme) => _assign == lexeme;
        private bool IsIdentifier(string lexeme)
        {
            if (!IsKeyWord(lexeme) && char.IsLetter(lexeme[0]))
                return true;

            return false;
        }
        private bool TryReadSymbol(out char symbol)
        {
            if (_number < _program.Length)
            {
                symbol = _program[_number++];
                return true;
            }
            symbol = default;
            return false;
        }
        private bool ReadLexeme(out string lexeme)
        {
            var sb = new StringBuilder();
            char symbol;
            while (TryReadSymbol(out symbol))
            {
                if (IsDelimiterLexem(symbol.ToString()))
                {
                    if (sb.Length != 0)
                    {
                        _number--;
                        lexeme = sb.ToString();
                        return true;
                    }
                    else
                    {
                        lexeme = symbol.ToString();
                        return true;
                    }
                }

                if (IsDelimiterSymbol(symbol))
                {
                    if (sb.Length == 0)
                        continue;
                    else
                        break;
                }
                else
                {
                    sb.Append(symbol);
                }

            }

            lexeme = sb.ToString();

            if (sb.Length > 0)
            {
                return true;
            }
            return false;
        }
        
    }
}
