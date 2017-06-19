using System;
using System.Collections.Generic;
using System.IO;

namespace Pan_Language
{
    internal class CodeGenerator : ICodeGenerator
    {
        public readonly Stack<object> STACK = new Stack<object>();
        private object TEMP1;
        private readonly Lexer lx = new Lexer();

        public CodeGenerator()
        {
            lx.Tokenize();
            Parser ps = new Parser(lx.Tokens, this);

            //Write Nesesarry code and initialization to the file
            Begin();
            ps.ParseClasses();
            End();
        }

        private void Begin()
        {
            Console.WriteLine("BEGIN");
        }

        private void End()
        {
            Console.WriteLine("END");
        }
        public void Number(int number)
        {
            Console.WriteLine("Number: {0}", number);
            STACK.Push(number);
        }
        public void String(string str)
        {
            Console.WriteLine("String: {0}", str);
            STACK.Push(str);
        }
        public void Bool(bool bl)
        {
            Console.WriteLine("Bool: {0}", bl);
            STACK.Push(bl);
        }
        public void True()
        {
            Console.WriteLine("True");
            Bool(true);
        }
        public void False()
        {
            Console.WriteLine("False");
            Bool(false);
        }
        public void Null()
        {
            Console.WriteLine("Null");
            String(null);
        }
        public void And()
        {
            Console.WriteLine("And");
            TEMP1 = STACK.Pop();
            STACK.Push((bool)TEMP1 && (bool)STACK.Pop());
        }
        public void Less()
        {
            Console.WriteLine("Less");
            TEMP1 = STACK.Pop();
            STACK.Push((int)STACK.Pop() < (int)TEMP1);
        }
        public void Or()
        {
            Console.WriteLine("Or");
            TEMP1 = STACK.Pop();
            STACK.Push((bool)TEMP1 || (bool)STACK.Pop());
        }
        public void Greater()
        {
            Console.WriteLine("Greater");
            TEMP1 = STACK.Pop();
            STACK.Push((int)TEMP1 > (int)STACK.Pop());
        }
        public void Add()
        {
            Console.WriteLine("Add");
            TEMP1 = STACK.Pop();
            try
            {
                STACK.Push((int)TEMP1 + (int)STACK.Pop());
            }
            catch
            {
                STACK.Push((string)STACK.Pop() + (string)TEMP1);
            }
        }
        public void Equal()
        {
            Console.WriteLine("Equal");
            TEMP1 = STACK.Pop();
            if (TEMP1.GetType() == typeof(string))
            {
                string STRING = (string)TEMP1;
                STACK.Push(STRING.Equals(STACK.Pop()));
            }
            else
            {
                STACK.Push(TEMP1 == STACK.Pop());
            }
        }
        public void LessOrEqual()
        {
            Console.WriteLine("Less or Equal");
            TEMP1 = STACK.Pop();
            STACK.Push((int)TEMP1 <= (int)STACK.Pop());
        }
        public void GreaterOrEqual()
        {
            Console.WriteLine("Greater or Equal");
            TEMP1 = STACK.Pop();
            STACK.Push((int)TEMP1 >= (int)STACK.Pop());
        }
        public void NotEqual()
        {
            Console.WriteLine("Not Equal");
            TEMP1 = STACK.Pop();
            STACK.Push((int)TEMP1 != (int)STACK.Pop());
        }
        public void Sub()
        {
            Console.WriteLine("Sub");
            TEMP1 = STACK.Pop();
            STACK.Push((int)TEMP1 - (int)STACK.Pop());
        }
        public void Mul()
        {
            Console.WriteLine("Mul");
            TEMP1 = STACK.Pop();
            STACK.Push((int)TEMP1 * (int)STACK.Pop());
        }
        public void Div()
        {
            Console.WriteLine("Div");
            TEMP1 = STACK.Pop();
            STACK.Push((int)STACK.Pop() / (int)TEMP1);
        }
        public void Mod()
        {
            Console.WriteLine("Mod");
            TEMP1 = STACK.Pop();
            STACK.Push((int)TEMP1 % (int)STACK.Pop());
        }
        public void This()
        {
            Console.WriteLine("This");
        }
        public void VariableRead(Token varName, Class c, Method m = null)
        {
            //Check if its a method variable
            if (m != null && m.HasVariable(varName.Value))
            {
                STACK.Push(m.GetVariable(varName.Value).Value);
            }
            //Check if its a class variable
            else if (c.HasVariable(varName.Value))
            {
                STACK.Push(c.GetVariable(varName.Value).Value);
            }
            else
            {
                throw new CompilerException("Variable does not exist in the current context");
            }
        }
        public void Negate()
        {
            Console.WriteLine("Negate");
            STACK.Push(-(int)STACK.Pop());
        }
        public void Not()
        {
            Console.WriteLine("Not");
            STACK.Push(!(bool)STACK.Pop());
        }
        public void Return()
        {
            Console.WriteLine("Return");
            STACK.Pop();
        }
        public void BeginIf()
        {
            Console.WriteLine("If");
        }
        public void PossibleElse()
        {
            Console.WriteLine("Else");
        }
        public void EndIf()
        {
            Console.WriteLine("End If");
        }
        public void Assign(Token varName, Class c, Method m = null)
        {
            //Check if its a method variable
            if (m != null && m.HasVariable(varName.Value))
            {
                Variable v = m.GetVariable(varName.Value);
                m.SetVariable(v, STACK.Pop());
            }
            //Check if its a class variable
            else if (c.HasVariable(varName.Value))
            {
                Variable v = c.GetVariable(varName.Value);
                c.SetVariable(v, STACK.Pop());
            }
            else
            {
                throw new CompilerException("Variable does not exist");
            }
        }
        public void Call(string className, string subroutineName) => Console.WriteLine("Function Call");  
        public void SetType(Token symbol) => Console.WriteLine("Set Type to: " + symbol.Value);
    }
}
