namespace Scriptid.Serialization
{
    internal enum TokenType
    {
        Punctuation,
        Number,
        String,
        Keyword,
        Identifier,
        Operator
    }

    internal sealed class Token
    {
        public Token(TokenType type, object value)
        {
            Type = type;
            Value = value;
        }

        public TokenType Type { get; }

        public object Value { get; }
    }
}