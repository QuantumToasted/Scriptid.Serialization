using System;
using System.Text;

namespace Scriptid.Serialization
{
    internal sealed class Lexer : IDisposable
    {
        private const string KEYWORDS = " if then else function num str embed ";
        private readonly InputStream _input;
        private Token _current;

        public Lexer(InputStream input)
        {
            _input = input;
        }

        public Token Peek()
            => _current ?? (_current = ReadNext());

        public Token Next()
        {
            var token = _current;
            _current = null;
            return token ?? ReadNext();
        }

        public bool Eof()
            => Peek() == null;

        public void Croak(string msg)
            => _input.Croak(msg);

        public void Dispose()
        {
            _input?.Dispose();
        }

        private bool IsKeyword(string str)
            => KEYWORDS.IndexOf($" {str} ", StringComparison.InvariantCulture) >= 0;

        private bool IsDigit(char ch)
            => char.IsNumber(ch);

        private bool IsIdentifierStart(char ch)
            => char.IsLetter(ch);

        private bool IsIdentifier(char ch)
            => char.IsLetterOrDigit(ch);

        private bool IsOperator(char ch)
            => "+-*/%=<>!".IndexOf(ch) >= 0;

        private bool IsPunctuation(char ch)
            => ",;(){}[]".IndexOf(ch) >= 0;

        private bool IsWhitespace(char ch)
            => " \t\r\n".IndexOf(ch) >= 0;

        private string ReadWhile(Func<char, bool> func)
        {
            var builder = new StringBuilder();
            while (!_input.Eof() && func((char) _input.Peek()))
            {
                builder.Append((char) _input.Next());
            }

            return builder.ToString();
        }

        private Token ReadNumber()
        {
            var number = ReadWhile(x => int.TryParse(x.ToString(), out _));
            return new Token(TokenType.Number, int.Parse(number));
        }

        private Token ReadIdentifier()
        {
            var identifier = ReadWhile(IsIdentifier);
            return new Token(IsKeyword(identifier) ? TokenType.Keyword : TokenType.Identifier, identifier);
        }

        private string ReadUntilTerminated(char terminator)
        {
            var escaped = false;
            var builder = new StringBuilder();
            _input.Next();

            while (!_input.Eof())
            {
                var ch = (char) _input.Next();
                if (escaped)
                {
                    builder.Append(ch);
                    escaped = false;
                }
                else if (ch == '\\')
                {
                    escaped = true;
                }
                else if (ch == terminator)
                {
                    break;
                }
                else
                {
                    builder.Append(ch);
                }
            }

            return builder.ToString();
        }

        private Token ReadString()
            => new Token(TokenType.String, ReadUntilTerminated('"'));

        private void SkipComment()
        {
            ReadWhile(x => x != '\n');
            _input.Next();
        }

        private Token ReadNext()
        {
            ReadWhile(IsWhitespace);
            if (_input.Eof()) return null;

            var ch = (char) _input.Peek();

            if (ch == '#')
            {
                SkipComment();
                return ReadNext();
            }

            if (ch == '"')
                return ReadString();

            if (IsDigit(ch))
                return ReadNumber();

            if (IsIdentifierStart(ch))
                return ReadIdentifier();

            if (IsPunctuation(ch))
                return new Token(TokenType.Punctuation, (char) _input.Next());

            if (IsOperator(ch))
                return new Token(TokenType.Operator, ReadWhile(IsOperator));

            // TODO: Custom exceptions(?)
            _input.Croak($"Cannot handle character: {ch}");
            return null;
        }
    }
}