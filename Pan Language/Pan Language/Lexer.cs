using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pan_Language
{
    internal class Lexer
    {
        private readonly StreamReader _sr = new StreamReader("input.txt");
        private Token _currentToken;
        public readonly List<Token> Tokens = new List<Token>();
        private int _currentLine;
        private bool _done;


        public void Tokenize()
        {
            _done = false;
            _currentLine = 0;
            while (!_done)
            {
                _currentLine++;
                Advance();
            }
            _sr.Close();
        }

        private void Advance()
        {
            //Eat useless whitspace
            EatWhiteSpace();

            char nextChar = NextChar();

            if (nextChar == '\uffff')
            {
                _done = true;
                return;
            }

            if (Syntax.IsNumber(nextChar))
            {
                string number = nextChar.ToString();
                //Get the full number
                number += EatWhile(Syntax.IsNumber);

                int result;
                if (!int.TryParse(number, out result))
                {
                    //Throw error number is too big
                    throw new CompilerException("Unable to parse number must be between 0-2147483648 but got: " + number + " At Token: " + _currentLine);
                }

                //Set currenttoken to number to be added to the list
                _currentToken = new Token(TokenType.NUMBER, number);
            }
            else if (Syntax.IsOperator(nextChar))
            {
                //Check if character needs a look ahead
                if ((new[] { '<', '>', '!' }.Contains(nextChar)))
                {
                    if (PeekNext() == '=')
                    {
                        _currentToken = new Token(TokenType.OPERATOR, nextChar + "=");
                        NextChar();
                    }
                    else if (PeekNext() == '-' && nextChar == '<')
                    {
                        _currentToken = new Token(TokenType.OPERATOR, nextChar + "-");
                        NextChar();
                    }
                    else
                    {
                        _currentToken = new Token(TokenType.OPERATOR, nextChar.ToString());
                    }
                }
                else
                {
                    _currentToken = new Token(TokenType.OPERATOR, nextChar.ToString());
                }
            }
            else if (Syntax.IsSymbol(nextChar))
            {
                _currentToken = new Token(TokenType.SYMBOL, nextChar.ToString());
            }
            else if (Syntax.IsStringStart(nextChar))
            {
                string str = EatWhile(c => !Syntax.IsStringStart(c));
                NextChar(); //Swallow ending of string
                _currentToken = new Token(TokenType.STRING, str);
            }
            else if (Syntax.IsKeywordOrId(nextChar))
            {
                string str = nextChar + EatWhile(c => Syntax.IsKeywordOrId(c));
                if (Syntax.IsKeyword(str))
                {
                    _currentToken = new Token(TokenType.KEYWORD, str);
                }
                else
                {
                    _currentToken = new Token(TokenType.IDENTIFIER, str);
                }
            }
            else if (Syntax.IsNewline(nextChar))
            {
                return;
            }
            else
            {
                throw new CompilerException("Unexpected character: " + nextChar + " At Token: " + _currentLine);
            }
            Tokens.Add(_currentToken);
            Console.WriteLine(_currentToken.GetString());
        }

        private char NextChar() => (char)_sr.Read();

        private void EatWhiteSpace()
        {
            while (_sr.Peek() == ' ')
            {
                _sr.Read();
            }
        }

        private string EatWhile(Func<char, bool> function)
        {
            string returnString = "";
            while (function((char)_sr.Peek()))
            {
                returnString += (char)_sr.Read();
            }
            return returnString;
        }

        private char PeekNext() => (char)_sr.Peek();
    }
}

