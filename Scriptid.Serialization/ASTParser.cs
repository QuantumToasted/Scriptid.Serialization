using System;
using System.Collections.Generic;
using Scriptid.Serialization.AST;

namespace Scriptid.Serialization
{
    public static class Scriptid
    {
        public static Program Parse(string input)
            => new ASTParser(new Lexer(new InputStream(input))).Parse();
    }

    internal sealed class ASTParser : IDisposable
    {
        private static readonly IReadOnlyDictionary<string, int> OperatorPrecedences = new Dictionary<string, int>
        {
            ["="] = 1,
            ["||"] = 2,
            ["&&"] = 3,
            ["<"] = 7, [">"] = 7, ["<="] = 7, [">="] = 7, ["=="] = 7, ["!="] = 7,
            ["+"] = 10, ["-"] = 10,
            ["*"] = 20, ["/"] = 20, ["%"] = 20
        };

        private readonly Lexer _lexer;

        public ASTParser(Lexer lexer)
        {
            _lexer = lexer;
        }

        public Program Parse()
            => ParseTopLevel();

        public void Dispose()
        {
            _lexer?.Dispose();
        }

        private bool IsPunctuation(char ch, out Token token)
        {
            token = _lexer.Peek();
            return token != null && token.Type == TokenType.Punctuation &&
                   (ch == char.MinValue || (char) token.Value == ch);
        }

        private bool IsKeyword(string keyword, out Token token)
        {
            token = _lexer.Peek();
            return token != null && token.Type == TokenType.Keyword &&
                   (string.IsNullOrWhiteSpace(keyword) || (string) token.Value == keyword);
        }

        private bool IsOperator(string @operator, out Token token)
        {
            token = _lexer.Peek();
            return token != null && token.Type == TokenType.Operator &&
                   (string.IsNullOrWhiteSpace(@operator) || (string) token.Value == @operator);
        }

        private void SkipPunctuation(char ch)
        {
            if (IsPunctuation(ch, out _))
            {
                _lexer.Next();
            }
            else
            {
                _lexer.Croak($"Expecting punctuation: \"{ch}\"");
            }
        }

        private void SkipKeyword(string keyword)
        {
            if (IsKeyword(keyword, out _))
            {
                _lexer.Next();
            }
            else
            {
                _lexer.Croak($"Expecting keyword: \"{keyword}\"");
            }
        }

        private void SkipOperator(string @operator)
        {
            if (IsOperator(@operator, out _))
            {
                _lexer.Next();
            }
            else
            {
                _lexer.Croak($"Expecting operator: \"{@operator}\"");
            }
        }

        private Node MaybeBinary(Node left, int precedence)
        {
            if (IsOperator(string.Empty, out var opToken))
            {
                var otherPrecedence = OperatorPrecedences[(string) opToken.Value];
                if (otherPrecedence > precedence)
                {
                    _lexer.Next();
                    if ((string) opToken.Value == "=")
                    {
                        return new Assignment(left, MaybeBinary(ParseAtomic(), otherPrecedence));
                    }

                    Operator op;
                    switch ((string) opToken.Value)
                    {
                        case "+":
                            op = Operator.Add;
                            break;
                        case "-":
                            op = Operator.Subtract;
                            break;
                        case "*":
                            op = Operator.Multiply;
                            break;
                        case "/":
                            op = Operator.Divide;
                            break;
                        case "%":
                            op = Operator.Remainder;
                            break;
                        case "^":
                            op = Operator.Exponent;
                            break;
                        case ">":
                            op = Operator.Greater;
                            break;
                        case ">=":
                            op = Operator.GreaterOrEqual;
                            break;
                        case "==":
                            op = Operator.Equal;
                            break;
                        case "!=":
                            op = Operator.NotEqual;
                            break;
                        case "<=":
                            op = Operator.LessOrEqual;
                            break;
                        case "<":
                            op = Operator.Less;
                            break;
                        default:
                            throw new Exception($"Expected operator: {opToken.Value}");
                    }
                    return new Binary(op, left, MaybeBinary(ParseAtomic(), otherPrecedence));
                }
            }

            return left;
        }

        private Node[] Split(char start, char stop, char separator, Func<Node> parser)
        {
            var list = new List<Node>();
            var first = true;

            SkipPunctuation(start);

            while (!_lexer.Eof())
            {
                if (IsPunctuation(stop, out _))
                    break;

                if (first)
                {
                    first = false;
                }
                else
                {
                    SkipPunctuation(separator);
                }

                if (IsPunctuation(stop, out _))
                    break;

                list.Add(parser());
            }

            SkipPunctuation(stop);
            return list.ToArray();
        }

        private Call ParseCall(Func<Node> func)
            => new Call(func, Split('(', ')', ',', ParseExpression));

        private Identifier ParseIdentifier()
        {
            var name = _lexer.Next();
            if (name.Type == TokenType.Identifier)
                return new Identifier((string) name.Value);

            throw new Exception($"Expected variable name, got {name.Value}");
        }

        private If ParseIf()
        {
            SkipKeyword("if");

            var cond = ParseExpression();
            if (!IsPunctuation('{', out _))
                SkipKeyword("then");
            var then = ParseExpression();
            if (IsKeyword("else", out _))
            {
                _lexer.Next();
                return new If(cond, then, ParseExpression());
            }

            return new If(cond, then, null);
        }

        private Node MaybeCall(Func<Node> expr)
        {
            var result = expr();
            return IsPunctuation('(', out _) ? ParseCall(() => result) : result;
        }

        private Node ParseExpression()
            => MaybeCall(() => MaybeBinary(ParseAtomic(), 0));

        private Node ParseAtomic()
        {
            return MaybeCall(() =>
            {
                if (IsPunctuation('(', out _))
                {
                    _lexer.Next();
                    var expr = ParseExpression();
                    SkipPunctuation(')');
                    return expr;
                }

                if (IsPunctuation('{', out _))
                    return ParseProgram();

                if (IsKeyword("if", out _))
                    return ParseIf();

                if (IsKeyword("true", out _) || IsKeyword("false", out _))
                    return ParseBoolean();

                var token = _lexer.Next();
                switch (token.Type)
                {
                    case TokenType.Keyword:
                        return new Keyword((string) token.Value); // TODO: Does this go here?
                    case TokenType.Number:
                        return new Number(int.Parse((string) token.Value));
                    case TokenType.String:
                        return new AST.String((string) token.Value);
                    case TokenType.Identifier: // TODO: Does this go here?
                        return new Identifier((string) token.Value);
                        // return ParseIdentifier();
                    // TODO: TokenType.Embed, and various embed types
                    default:
                        throw new Exception($"Invalid data type: {token.Value}");
                }
            });
        }

        private Program ParseTopLevel()
        {
            var prog = new List<Node>();
            while (!_lexer.Eof())
            {
                prog.Add(ParseExpression());
                if (!_lexer.Eof())
                    SkipPunctuation(';');
                /* TODO: Find out why SkipPunctuation does not function as intended
                if (!_lexer.Eof())
                    SkipPunctuation(';');
                */
            }

            return new Program(prog.ToArray());
        }

        private AST.Boolean ParseBoolean()
            => new AST.Boolean((string) _lexer.Next().Value == "true");

        private Program ParseProgram()
        {
            var prog = Split('{', '}', ';', ParseExpression);
            return prog.Length > 0 ? new Program(prog) : Program.Empty;
        }
    }
}