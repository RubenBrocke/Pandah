using System.Linq;

namespace Pan_Language
{
    internal class Token
    {
        public TokenType Type { get; }
        public string Value { get; }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        public string GetString() => "<'" + Type.ToString("F") + "', '" + Value + "'>";
    }

    internal enum TokenType
    {
        NUMBER,
        OPERATOR,
        IDENTIFIER,
        KEYWORD,
        NEWLINE,
        STRING,
        SYMBOL
    }
}
