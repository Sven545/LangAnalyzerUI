using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangAnalyzerCore
{
    public class IfSyntaxisAnalyzer
    {
        public AstNode Analyze(IEnumerable<Lexeme> lexemes)
        {
            var node = If(lexemes.ToList());
            node.AddChild(ToNode(lexemes.Last()));
            return node;
        }
        private AstNode If(IList<Lexeme> lexemes)
        {
            if (lexemes == null
                || lexemes.First().Value != "IF"
                || lexemes.Last().Value != "END_IF"
                )
                throw new Exception("Ошибка оператора IF");

            var ifNode = ToNode(lexemes[0]);

            int firstThenLexemeIndex = lexemes.IndexOf(FirstThenLexeme(lexemes));

            ifNode.AddChild(PredicateNode(lexemes, firstThenLexemeIndex));
            ifNode.AddChild(ThenNode(lexemes, firstThenLexemeIndex));

            var elseLexeme = FirstElseLexeme(lexemes);

            if (elseLexeme != null)
            {
                int firstElseLexemeIndex = lexemes.IndexOf(elseLexeme);

                if (firstElseLexemeIndex == lexemes.Count() - 1)
                    throw new Exception("Ошибка оператора ELSE");

                var elseNode = ToNode(elseLexeme);

                if (lexemes[firstElseLexemeIndex + 1].Value == "IF")
                    elseNode.AddChild(If(lexemes.Skip(firstElseLexemeIndex + 1).ToList()));
                else
                    elseNode.AddChild(OperatorsBody(lexemes.Skip(firstElseLexemeIndex).ToList(), "ELSE"));

                ifNode.AddChild(elseNode);
            }

            return ifNode;
        }
        private AstNode OperatorsBody(List<Lexeme> lexemes, string keyword)
        {
            if (lexemes == null || lexemes[0].Value != keyword)
                throw new Exception($"Ошибка оператора {keyword}");

            var node = ToNode(lexemes[0]);
            AddAssigns(node, lexemes.Skip(1).ToList());

            return node;

        }

        private void AddAssigns(AstNode node, IList<Lexeme> lexemes)
        {
            var assignLexemes = new List<Lexeme>();

            foreach (var lexeme in lexemes)
            {
                assignLexemes.Add(lexeme);
                if (lexeme.Type == LexemeType.Delimiter)
                {
                    node.AddChild(Assign(assignLexemes));
                    assignLexemes.Clear();
                }
                if (lexeme.Type == LexemeType.Keyword)
                {
                    if (lexeme.Value == "ELSE" || lexeme.Value == "END_IF") return;
                }
            }
        }

        private AstNode Assign(IList<Lexeme> lexemes)
        {
            if (lexemes == null
                || lexemes[0].Type != LexemeType.Identifier
                || lexemes[1].Type != LexemeType.Assignment
                || lexemes.Count != 6
                || lexemes[5].Type != LexemeType.Delimiter
                )
                throw new Exception("Ошибка выражения присваивания");

            var node = ToNode(lexemes[1]);
            node.AddChild(ToNode(lexemes[0]));
            node.AddChild(ArithmeticOperation(lexemes.Skip(2).ToList()));

            return node;
        }

        private AstNode ArithmeticOperation(IList<Lexeme> lexemes)
        {
            if (lexemes == null
                || lexemes.Count() != 4
                || !IsIdentifierOrNumber(lexemes[0])
                || !IsIdentifierOrNumber(lexemes[2])
                || lexemes[1].Type != LexemeType.Arithmetic
                || lexemes[3].Type != LexemeType.Delimiter)
                throw new Exception("Ошибка арифметической операции");

            var node = ToNode(lexemes[1]);
            node.AddChild(ToNode(lexemes[0]));
            node.AddChild(ToNode(lexemes[2]));

            return node;
        }

        private AstNode Predicate(IList<Lexeme> lexemes)
        {
            var currentNode = new AstNode()
            {
                Type = "PREDICATE",
                Value = "PREDICATE"
            };
            var leftNode = Comparsion(lexemes.Take(3).ToList());
            currentNode.AddChild(leftNode);

            var rightLexemes = lexemes.Skip(3).ToList();

            if (rightLexemes.Count() == 0)
            {
                return currentNode;

            }


            if (rightLexemes.Count() < 4)
                throw new Exception("Ошибка предиката");

            var rightNode = LogicOperator(rightLexemes);
            currentNode.AddChild(rightNode);
            return currentNode;

        }

        private AstNode LogicOperator(IList<Lexeme> lexemes)
        {
            if (lexemes == null || lexemes[0].Type != LexemeType.LogicOperator)
                throw new Exception("Ошибка логической операции");

            var currentNode = ToNode(lexemes[0]);

            var leftLexemes = lexemes.Skip(1).Take(3).ToList();

            var leftNodes = Comparsion(leftLexemes);
            currentNode.AddChild(leftNodes);

            var rightLexemes = lexemes.Skip(4).ToList();

            if (rightLexemes.Count() == 0)
                return currentNode;


            if (rightLexemes.Count() < 4)
                throw new Exception("Ошибка логической операции");


            var rightNodes = LogicOperator(rightLexemes);
            currentNode.AddChild(rightNodes);

            return currentNode;
        }

        private AstNode Comparsion(IList<Lexeme> lexemes)
        {
            if (lexemes == null || lexemes.Count != 3)
                throw new Exception("Ошибка оператора сравнения");

            if (IsIdentifierOrNumber(lexemes[0])
                && IsIdentifierOrNumber(lexemes[2])
                && IsComparsion(lexemes[1]))
            {
                var node = ToNode(lexemes[1]);
                node.AddChild(ToNode(lexemes[0]));
                node.AddChild(ToNode(lexemes[2]));

                return node;
            }
            throw new Exception("Ошибка оператора сравнения");

        }


        private Lexeme? FirstElseLexeme(IList<Lexeme> lexemes)
        {
            return lexemes.FirstOrDefault(l => l.Value == "ELSE");
        }

        private Lexeme FirstThenLexeme(IList<Lexeme> lexemes)
        {
            var thenLexeme = lexemes.FirstOrDefault(l => l.Value == "THEN");

            if (thenLexeme == null)
                throw new Exception("Не найден оператор THEN");

            return thenLexeme;
        }

        private AstNode ThenNode(IList<Lexeme> lexemes, int firstThenIndex)
        {
            var thenLexemes = lexemes.Skip(firstThenIndex);
            var thenNode = OperatorsBody(thenLexemes.ToList(), "THEN");
            return thenNode;
        }

        private AstNode PredicateNode(IList<Lexeme> lexemes, int firstThenIndex)
        {
            var predicateLexemes = lexemes.Skip(1).Take(firstThenIndex - 1);
            var predicateNodes = Predicate(predicateLexemes.ToList());
            return predicateNodes;
        }

        private AstNode ToNode(Lexeme lexeme)
        {
            return new AstNode()
            {
                Type = lexeme.Type.ToString(),
                Value = lexeme.Value
            };
        }

        private bool IsComparsion(Lexeme lexeme)
        {
            return lexeme.Type == LexemeType.Comparsion;
        }

        private bool IsIdentifierOrNumber(Lexeme lexeme)
        {
            return (lexeme.Type == LexemeType.Identifier) || (lexeme.Type == LexemeType.Number);
        }
    }
}
