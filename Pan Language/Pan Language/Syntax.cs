using System.Collections.Generic;

namespace Pan_Language
{
    public static class Syntax
    {

        private static readonly List<string> KeyWords = new List<string> { "if", "else", "function", "true", "false", "null", "this", "int", "string", "let", "class", "exec", "print", "void", "while" };
        private static readonly List<char> Operators = new List<char> { '+', '-', '*', '/', '%', '<', '>', '=', '!', '&', '|', '?', ';' };
        private static readonly List<char> Symbols = new List<char> { '{', '}', '(', ')', ':', ',' };

        public static bool IsNumber(char input)
        {
            return char.IsDigit(input);
        }

        public static bool IsOperator(char input)
        {
            return Operators.Contains(input);
        }

        public static bool IsKeywordOrId(char input)
        {
            return char.IsLetterOrDigit(input);
        }

        public static bool IsStringStart(char input)
        {
            return input == '"';
        }

        public static bool IsKeyword(string input)
        {
            return KeyWords.Contains(input);
        }

        public static bool IsSymbol(char input)
        {
            return Symbols.Contains(input);
        }

        public static bool IsNewline(char input)
        {
            return char.IsControl(input);
        }
    }
}
