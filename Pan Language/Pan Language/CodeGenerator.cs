using System;
using System.Collections.Generic;
using System.IO;

namespace Pan_Language
{
    internal class CodeGenerator : ICodeGenerator
    {
        private readonly StreamWriter _output = new StreamWriter("output.txt");
        public readonly Stack<object> STACK = new Stack<object>();
        private object TEMP1;
        private readonly Lexer lx = new Lexer();

        public CodeGenerator()
        {
            lx.Tokenize();
            Parser ps = new Parser(lx.Tokens, this);
            _output.AutoFlush = true;

            //Write Nesesarry code and initialization to the file
            EmitEnvironment();
            EmitBootstrapper();
            ps.ParseClass();
            EmitEnd();
        }

        private void EmitBootstrapper()
        {
            _output.WriteLine("namespace Test");
            _output.WriteLine("{");
            _output.WriteLine("  class Program");
            _output.WriteLine("  {");
            _output.WriteLine("public static Stack<object> STACK = new Stack<object>();");
            _output.WriteLine("public static int TEMP1;");
            _output.WriteLine("    static void Main(string[] args)");
            _output.WriteLine("    {");
        }

        private void EmitEnvironment()
        {
            _output.WriteLine("using System;");
            _output.WriteLine("using System.Collections.Generic;");
            _output.WriteLine();
            Console.WriteLine("BEGIN");
        }

        private void EmitEnd()
        {
            _output.WriteLine("    }");
            _output.WriteLine("  }");
            _output.WriteLine("}");
            Console.WriteLine("END");
        }
        public void Number(int number)
        {
            Console.WriteLine("Number: {0}", number);
            _output.WriteLine("STACK.Push({0});", number);
            STACK.Push(number);
        }
        public void String(string str)
        {
            Console.WriteLine("String: {0}", str);
            _output.WriteLine("STACK.Push(\"{0}\");", str);
            STACK.Push(str);
        }
        public void Bool(bool bl)
        {
            Console.WriteLine("Bool: {0}", bl);
            _output.WriteLine("STACK.Push({0});", bl);
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
            _output.WriteLine("TEMP1 = STACK.Pop();");
            _output.WriteLine("STACK.Push(TEMP && STACK.Pop());");
            TEMP1 = STACK.Pop();
            STACK.Push((bool)TEMP1 && (bool)STACK.Pop());
        }
        public void Less()
        {
            Console.WriteLine("Less");
            _output.WriteLine("TEMP1 = STACK.Pop();");
            _output.WriteLine("STACK.Push(TEMP < STACK.Pop());");
            TEMP1 = STACK.Pop();
            STACK.Push((int)STACK.Pop() < (int)TEMP1);
        }
        public void Or()
        {
            Console.WriteLine("Or");
            _output.WriteLine("TEMP1 = STACK.Pop();");
            _output.WriteLine("STACK.Push(TEMP || STACK.Pop());");
            TEMP1 = STACK.Pop();
            STACK.Push((bool)TEMP1 || (bool)STACK.Pop());
        }
        public void Greater()
        {
            Console.WriteLine("Greater");
            _output.WriteLine("TEMP1 = STACK.Pop();");
            _output.WriteLine("STACK.Push(TEMP > STACK.Pop());");
            TEMP1 = STACK.Pop();
            STACK.Push((int)TEMP1 > (int)STACK.Pop());
        }
        public void Add()
        {
            Console.WriteLine("Add");
            _output.WriteLine("TEMP1 = STACK.Pop();");
            _output.WriteLine("STACK.Push(TEMP + STACK.Pop());");
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
            _output.WriteLine("TEMP1 = STACK.Pop();");
            _output.WriteLine("STACK.Push(TEMP == STACK.Pop());");
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
            _output.WriteLine("TEMP1 = STACK.Pop();");
            _output.WriteLine("STACK.Push(TEMP <= STACK.Pop());");
            TEMP1 = STACK.Pop();
            STACK.Push((int)TEMP1 <= (int)STACK.Pop());
        }
        public void GreaterOrEqual()
        {
            Console.WriteLine("Greater or Equal");
            _output.WriteLine("TEMP1 = STACK.Pop();");
            _output.WriteLine("STACK.Push(TEMP >= STACK.Pop());");
            TEMP1 = STACK.Pop();
            STACK.Push((int)TEMP1 >= (int)STACK.Pop());
        }
        public void NotEqual()
        {
            Console.WriteLine("Not Equal");
            _output.WriteLine("TEMP1 = STACK.Pop();");
            _output.WriteLine("STACK.Push(TEMP != STACK.Pop());");
            TEMP1 = STACK.Pop();
            STACK.Push((int)TEMP1 != (int)STACK.Pop());
        }
        public void Sub()
        {
            Console.WriteLine("Sub");
            _output.WriteLine("TEMP1 = STACK.Pop();");
            _output.WriteLine("STACK.Push(TEMP - STACK.Pop());");
            TEMP1 = STACK.Pop();
            STACK.Push((int)TEMP1 - (int)STACK.Pop());
        }
        public void Mul()
        {
            Console.WriteLine("Mul");
            _output.WriteLine("TEMP1 = STACK.Pop();");
            _output.WriteLine("STACK.Push(TEMP * STACK.Pop());");
            TEMP1 = STACK.Pop();
            STACK.Push((int)TEMP1 * (int)STACK.Pop());
        }
        public void Div()
        {
            Console.WriteLine("Div");
            _output.WriteLine("TEMP1 = STACK.Pop();");
            _output.WriteLine("STACK.Push(TEMP / STACK.Pop());");
            TEMP1 = STACK.Pop();
            STACK.Push((int)STACK.Pop() / (int)TEMP1);
        }
        public void Mod()
        {
            Console.WriteLine("Mod");
            _output.WriteLine("TEMP1 = STACK.Pop();");
            _output.WriteLine("STACK.Push(TEMP % STACK.Pop());");
            TEMP1 = STACK.Pop();
            STACK.Push((int)TEMP1 % (int)STACK.Pop());
        }
        public void This()
        {
            Console.WriteLine("This");
            _output.Write("this");
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
            _output.WriteLine("STACK.Push(-STACK.Pop());");
            STACK.Push(-(int)STACK.Pop());
        }
        public void Not()
        {
            Console.WriteLine("Not");
            _output.WriteLine("STACK.Push(!STACK.Pop());");
            STACK.Push(!(bool)STACK.Pop());
        }
        public void Return()
        {
            Console.WriteLine("Return");
            _output.Write("return");
            STACK.Pop();
        }
        public void BeginIf()
        {
            Console.WriteLine("If");
            _output.Write("if");
        }
        public void PossibleElse()
        {
            Console.WriteLine("Else");
            _output.Write("else");
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
