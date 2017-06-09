using System;
using System.Collections.Generic;
using System.Linq;

namespace Pan_Language
{
    internal class Parser
    {
        private readonly List<Token> _parseTokens;      //Tokens to be used in parsing
        private int _tokenCount;                        //number that indicates how far in the token list the parser is
        private readonly CodeGenerator _codeGenerator;  //the codegenerator which i use for stack operations
        private Class _currentClass;                    //The class the parser is currently in cannot be null
        private Method _currentMethod;                  //The method the parser is currently in can be null
        private Stack<int> returnStack;                 //the stack that holds where the praser should go after a series of function calls
        private int ProgramStartIndex;                  //the token where the program should start (class = program method = main)

        public Parser(List<Token> tokens, CodeGenerator codeGenerator)
        {
            _parseTokens = tokens;                                  //get the tokens from the lexer and store them
            _parseTokens.Add(new Token(TokenType.NEWLINE, null));   //add a null newline at the end so we can see when its done
            _tokenCount = -1;                                       //start tokencount at 1 
            _codeGenerator = codeGenerator;                         //store the code generator for later use
            returnStack = new Stack<int>();                         //initialise the return stack
        }

        public void ParseClasses()
        {
            while (PeekToken().Type == TokenType.KEYWORD && PeekToken().Value == "class")
            {
                ParseClass();
                if (_parseTokens[_tokenCount].Type == TokenType.NEWLINE)
                {
                    break;
                }
            }
            _tokenCount = ProgramStartIndex;                                        //set the tokencount to the beginning of the main function
            _currentMethod = _currentClass.GetMethod("Main");                       //set the current method to the main method
            ParseStatements();                                                      //start the parsing process of main
        }

        public void ParseClass()
        {
            if (PeekToken().Type != TokenType.KEYWORD && PeekToken().Value != "class")  //Check if the first thing is a class
            {
                throw new CompilerException("NO CLASS FOUND");                          //if it is not (it should be) throw an error
            }
            Token classToken = NextToken();                                             //store the class token
            if (PeekToken().Type != TokenType.IDENTIFIER)                               //next token should be an identifier
            {
                throw new CompilerException("CLASS HAS NO IDENTIFIER");                 //if it is not throw an error
            }
            Token classIdentifier = NextToken();                                        //store the identifier token
            Class c = new Class(classIdentifier.Value);                                 //create a new class based on the class token
            _currentClass = c;                                                          //set it as the current class
            Global.Classes.Add(c);                                                    //add the class to the variable list (to become namespace)
            ParseLocalVarDecl();                                                        //parse all the local variable declaration
            ParseSubDecls();                                                            //parse the sub declaration of all the other functions
            MatchWhile(new Token(TokenType.KEYWORD, "end"));
            _tokenCount += 2;
            if (_currentClass.HasMethod("Main") && _currentClass.ClassName == "Program")                                        //if there's a main class run it
            {
                ProgramStartIndex = _currentClass.GetMethod("Main").MethodIndex;
            }
        }

        private void ParseSubDecls()
        {
            while (IsNextSubDecl())     //Check if the next token is a sub declaration token (eg. function)
            {
                ParseFunctionDecl();    //It is so parse the sub routine
            }
        }

        private void ParseFunctionDecl()
        {
            Token functionToken = NextToken();                                                      //Get the function token and store it
            Token functionName = NextToken();                                                       //Get the function name token and store it
            if (functionName.Type != TokenType.IDENTIFIER)                                          //Make sure the function name is a valid one (identifier)
            {
                throw new CompilerException("Method name expected got: " + functionName.Value);     //if it is not throw an error telling the use to use another name
            }
            Method m = new Method(functionName.Value, _tokenCount + 3);                             //store the method found in a method class
            Match(new Token(TokenType.OPERATOR, "<-"));                                             //make sure there is a <-
            ParseMethodParams(m);                                                                   //parse all the method parameters                                               //make sure there's an {
            m.MethodIndex = _tokenCount;                                                            //store the current tokencount in the method class for later use
            MatchWhile(new Token(TokenType.KEYWORD, "end"));
            _tokenCount++;
            _currentClass.ClassMethods.Add(m);                                                      //add the method to the current class for later use
        }

        private void ParseMethodParams(Method m)
        {
            if (PeekToken().Value == ";")                                                           //check if parameters have been given
            {
                throw new CompilerException("No parameters in function. Use Void");                 //if no parameters have been found tell the user to use void
            }
            while (PeekToken().Value != ";")                                                        //keep parsing the parameters until there's an ;
            {
                Token idToken = NextToken();                                                        //store the identifier token 
                if (idToken.Type != TokenType.IDENTIFIER)                                           //make sure the token is an identifier
                {
                    if (idToken.Type == TokenType.KEYWORD && idToken.Value == "void")               //check for the void exception
                    {
                        return;                                                                     //do nothing if the given parameter is void (to be checked earlier)
                    }
                    throw new CompilerException("Only Identifiers allowed as function parameters"); //the token is not a valid one. throw an error to tell the user what went wrong
                }
                Variable v = new Variable(null);                                                    //create a variable class for later use
                m.Params.Add(idToken.Value, v);                                                     //store the variable in the method parameter list
                m.MethodVars.Add(idToken.Value, v);                                                 //store the variable in the method variable list
            }
        }

        private void ParseLocalVarDecl()
        {
            while (IsNextVarDecl())     //check to see if the next token is a variable declaration (eg. let) and keep going as long as it is
            {
                ParseLetStatement();    //the next token is a let statement so parse it
            }
        }

        private void ParseStatements()
        {
            while (PeekToken().Value != "end")    //check to see if the next token is a }  keep going as long as it is
            {
                ParseStatement();               //the next token is not a } parse the statment comming up next
            }
        }

        private void ParseStatement()
        {
            Token peekToken = PeekToken();                                                  //store the token for later use
            if (peekToken.Type != TokenType.KEYWORD)                                        //make sure it an identifier
            {
                throw new CompilerException("KEYWORD expected, got: " + peekToken.Value);   //its not an identifer. throw an error to let the user know
            }
            switch (peekToken.Value)                                                        //check the token for the different possibilities
            {
                case "let":                 //it's a let statment
                    ParseLetStatement();    //parse it
                    break;
                case "if":                  //it's an if statment
                    ParseIfStatement();     //parse it
                    break;
                case "exec":                //it's an exec statment
                    ParseFunctionCall();    //parse it
                    break;
                case "print":               //its a print statement
                    ParsePrintStatement();  //prase it
                    break;
                case "while":               //its a while statement
                    ParseWhileStatement();  //parse it
                    break;
                case "input":
                    ParseInputStatement();
                    break;
                case "init":
                    ParseInitStatement();
                    break;
            }
        }

        private void ParseInitStatement()
        {
            Match(new Token(TokenType.KEYWORD, "init"));
            if (PeekToken().Type != TokenType.IDENTIFIER)
            {
                throw new CompilerException("Not an identifier");
            }
            Token ident = NextToken();
            Match(new Token(TokenType.OPERATOR, "<-"));
            if (PeekToken().Type != TokenType.IDENTIFIER)
            {
                throw new CompilerException("Not a class identifier");
            }
            Token Class = NextToken();
            try
            {
                Global.GetClass(Class.Value).instanceIDs.Add(ident.Value);
                Console.WriteLine("INSTANCE CREATED");
            }
            catch
            {
                throw new CompilerException("Could not create instance of: " + Class.Value);
            }
        }

        private void ParseInputStatement()
        {
            Match(new Token(TokenType.KEYWORD, "input"));
            if (PeekToken().Type != TokenType.IDENTIFIER)
            {
                throw new CompilerException("Cant store input in: " + PeekToken().Value);
            }
            Token ident = NextToken();
            if (_currentMethod != null)                                                        //Check if we're in a method
            {
                if (!_currentMethod.HasVariable(ident.Value))                                     //check if the current method has the variable
                {
                    _currentMethod.MethodVars.Add(ident.Value, new Variable(null));               //there's no variable in the currentclass nor in the method. we should add it
                }
            }
            else
            {
                _currentClass.ClassVars.Add(ident.Value, new Variable(null));                     //we're not in a method and its not a class variable. we should add it
            }
            _codeGenerator.STACK.Push(Console.ReadLine());
            Console.WriteLine("Input: " + ident.Value);
            _codeGenerator.Assign(ident, _currentClass, _currentMethod);

        }

        private void ParsePrintStatement()
        {
            Match(new Token(TokenType.KEYWORD, "print"));               //make sure the next token is print keyword
            ParseExpression();                                          //parse the expression to be printed
            Console.WriteLine("Print: {0}", _codeGenerator.STACK.Pop());//write the first thing on the stack in the console
        }

        private void ParseFunctionCall()
        {
            Class c = _currentClass;
            Match(new Token(TokenType.KEYWORD, "exec"));                        //make sure there's an exec keyword
            if (Global.GetClass(PeekToken().Value) != null)
            {
                Token className = NextToken();
                c = Global.
                Match(new Token(TokenType.SYMBOL, "."));
            }
            Token functionName = NextToken();                                   //store the token for later use
            Console.WriteLine("Executing: " + functionName.Value);              //write to console what function it is executing
            Match(new Token(TokenType.SYMBOL, "("));                            //make sure the next token is a (
            ParseGivenParameters(_currentClass.GetMethod(functionName.Value));  //parse all the parameters that have been given
            Match(new Token(TokenType.SYMBOL, ")"));                            //make sure the next token is a )
            returnStack.Push(_tokenCount);                                      //push the current tokencount in a stack
            if (_currentClass.HasMethod(functionName.Value))                    //check to see if the method exists in the current class
            {
                Method m = _currentClass.GetMethod(functionName.Value);         //store the method in a variable for later use
                _currentMethod = m;                                             //set the current method to the executing method
                _tokenCount = m.MethodIndex;                                    //set the token count the beginning of the method                      
                ParseStatements(); //TODO: parsesubbody                         //parse the statements in the function
            }
            else
            {
                throw new CompilerException("Function not existent");           //if there's no known function throw an error
            }
            _tokenCount = returnStack.Pop();                                    //return tho before you executed the function
            _currentMethod = null;                                              //set the current method to null because we arent in one anymore FIXME: dont set it to null
        }

        private void ParseGivenParameters(Method m)
        {
            int argscounter = 0;                                                                    //set the argument counter to 0
            while (PeekToken().Value != ")")                                                        //check if the next token is a )
            {
                ParseExpression();                                                                  //parse the expression (parameter)
                m.Params[m.Params.ElementAt(argscounter).Key].Value = _codeGenerator.STACK.Pop();   //store it in the param list of the method
                argscounter++;                                                                      //increment the argument counter
                if (PeekToken().Value != ")")                                                       //if the next one is not a )
                {
                    Match(new Token(TokenType.SYMBOL, ","));                                        //make sure there's a ,
                }
            }
        }

        private void ParseLetStatement()
        {
            Match(new Token(TokenType.KEYWORD, "let"));                                             //make sure there's a let keyword
            if (PeekToken().Type != TokenType.IDENTIFIER)                                           //make sure the next token is an identifier
            {
                throw new CompilerException("Identifier expected got: " + PeekToken().Value);       //if not throw an error to let the user know
            }
            Token varName = NextToken();                                                            //store the identifier token for later use
            if (_currentClass.HasVariable(varName.Value))                                           //Check if variable exists as class var
            {
                throw new CompilerException("This class already contains the variable: " + varName.Value);                                                                                    //It is a class variable do nothing
            }
            else if (_currentMethod != null)                                                        //Check if we're in a method
            {
                if (!_currentMethod.HasVariable(varName.Value))                                     //check if the current method has the variable
                {
                    _currentMethod.MethodVars.Add(varName.Value, new Variable(null));               //there's no variable in the currentclass nor in the method. we should add it
                }
            }
            else
            {
                _currentClass.ClassVars.Add(varName.Value, new Variable(null));                     //we're not in a method and its not a class variable. we should add it
            }
            Match(new Token(TokenType.OPERATOR, "<-"));                                             //make sure there's a <- 
            ParseExpression();                                                                      //parse the expression after the <-
            _codeGenerator.Assign(varName, _currentClass, _currentMethod);                          //assign the result to the variable                                         //make sure there's a ;
        }

        private void ParseIfStatement()
        {
            Match(new Token(TokenType.KEYWORD, "if"));          //make sure there's an if keyword
            Match(new Token(TokenType.SYMBOL, "("));            //make sure there's a ( after that
            ParseExpression();                                  //parse the if expression
            _codeGenerator.BeginIf();                           //tell the code gen an if is beginning
            Match(new Token(TokenType.SYMBOL, ")"));            //make sure there's a )
            //Match(new Token(TokenType.SYMBOL, "{"));            //make sure there's a {
            if (Convert.ToBoolean(_codeGenerator.STACK.Pop()))  //check if the if expression is true or false
            {
                Console.WriteLine("If is true");
                ParseStatements();                              //parse everything in the if statement
                Match(new Token(TokenType.KEYWORD, "end"));
            }
            else
            {
                Console.WriteLine("If is false");
                MatchWhile(new Token(TokenType.KEYWORD, "end"), new Token(TokenType.KEYWORD, "else"));
                if (PeekToken().Value == "else")
                {
                    _tokenCount++;
                    ParseStatements();
                    Match(new Token(TokenType.KEYWORD, "end"));
                }
                else if (PeekToken().Value == "end")
                {
                    Match(new Token(TokenType.KEYWORD, "end"));
                }
            }
        }

        private void ParseWhileStatement()
        {
            Match(new Token(TokenType.KEYWORD, "while"));       //make sure the next token is a while keyword
            Match(new Token(TokenType.SYMBOL, "("));            //make sure the one after that is a (
            int returnCount = _tokenCount;                      //store the current token count
            ParseExpression();                                  //parse the while expression
            Match(new Token(TokenType.SYMBOL, ")"));            //make sure there's a )
            //Match(new Token(TokenType.SYMBOL, "{"));            //make sure there's a {
            while(Convert.ToBoolean(_codeGenerator.STACK.Pop()))//keep the while loop going as long as there's true on top of the stack
            {
                ParseStatements();                              //parse the statment in the while loop
                _tokenCount = returnCount;                      //set the tokencount back to the expression in the while loop
                ParseExpression();                              //parse that expression again
                Match(new Token(TokenType.SYMBOL, ")"));        //make sure there's a ) (again)
                //Match(new Token(TokenType.SYMBOL, "{"));        //make sure there's a { (again)
            }
            Match(new Token(TokenType.KEYWORD, "end"));            //make sure it ends with a }
        }

        private void ParseExpression()
        {
            ParseRelExpression();                               //first check for any relative expression
            while (IsNextLogicalOp())                           //as long as the next token is a logical operator
            {
                Token logOp = NextToken();                      //store the operator for later use
                ParseRelExpression();                           //parse the relative expression after the logic operator
                switch (logOp.Value)                            //switch all the posibilities 
                {
                    case "&":                                   //in case of &
                        _codeGenerator.And();                   //tell the code gen there's an and operator
                        break;
                    case "|":                                   //in case of |
                        _codeGenerator.Or();                    //tell the code gen there's an or operator
                        break;
                    default:
                        throw new CompilerException("Logical operator not found");  //throw an error if the operator is not in the switch statement
                }
            }
            //Done = true;
        }

        private void ParseRelExpression()
        {
            ParseAddExpression();                       //parse an additive expression first
            while (IsNextRelOp())                       //keep going as long as the next toke is a relative operator
            {
                Token relOp = NextToken();              //store the operator for later use
                ParseAddExpression();                   //parse the additive expression after this operator
                switch (relOp.Value)                    //switch the posibilities of the operator
                {
                    case "<":                           //in case of <
                        _codeGenerator.Less();          //tell the code gen there's a less than operator
                        break;
                    case ">":                           //in case of >
                        _codeGenerator.Greater();       //tell the code gen there'a a greater than operator
                        break;
                    case "?":                           //in case of ?
                        _codeGenerator.Equal();         //tell the code gen there's an equals operator
                        break;
                    case "<=":                          //in case of <=
                        _codeGenerator.LessOrEqual();   //tell the code gen there's a less than or equals operator
                        break;
                    case ">=":                          //in case of >=
                        _codeGenerator.GreaterOrEqual();//tell the code gen there'a a greater than of equals operator
                        break;
                    case "!=":                          //in case of !=
                        _codeGenerator.NotEqual();      //tell the code gen there's a not equals operator
                        break;
                    default:
                        throw new CompilerException("Relational operator not found");   //throw an error if the operator is not in the switch statement
                }
            }
        }

        private void ParseAddExpression()
        {
            ParseMulExpression();               //parse a multiplicative expression first
            while (IsNextAddOp())               //keep going as long as the next token is an additive operator
            {
                Token addOp = NextToken();      //store the operator for later use
                ParseMulExpression();           //parse the multiplicative expression after the operator
                switch (addOp.Value)            //switch the additive operator value
                {   
                    case "+":                   //in case of +
                        _codeGenerator.Add();   //tell the code gen there's an add operator
                        break;
                    case "-":                   //in case of -
                        _codeGenerator.Sub();   //tell the code gen there's a subtract operator
                        break;
                    default:
                        throw new CompilerException("Add operator not found");  //throw an error if the operator is not in the switch statement
                }
            }
        }

        private void ParseMulExpression()
        {
            ParseTerm();                        //parse a type assignment first
            while (IsNextMulOp())               //keep going as long as the next token is a miltiplicative operator
            {
                Token mulOp = NextToken();      //store the token for later use
                ParseTerm();                    //parse a possible term
                switch (mulOp.Value)            //switch trough all values of the operator
                {
                    case "*":                   //in case of *
                        _codeGenerator.Mul();   //tell the code gen there's a mutiply operator
                        break;
                    case "/":                   //in case of /
                        _codeGenerator.Div();   //tell the code gen there's a dividing operator
                        break;
                    case "%":                   //in case of %
                        _codeGenerator.Mod();   //tell the code gen there's a modulo operator
                        break;
                    default:
                        throw new CompilerException("Mul operator not found");  //throw an error if the operator is not in the switch statement
                }
            }
        }

        private void ParseTerm()
        {
            //Check for number
            switch (PeekToken().Type)                                   //switch the value of the type enum of the next token
            {
                case TokenType.NUMBER:                                  //in case of a number
                    Token number = NextToken();                         //store the number token
                    _codeGenerator.Number(int.Parse(number.Value));     //tell the code gen there's a number
                    break;
                case TokenType.STRING:                                  //in case of a string
                    Token str = NextToken();                            //store the string token
                    _codeGenerator.String(str.Value);                   //tell the code gen there's a string
                    break;
                case TokenType.IDENTIFIER:
                    Token ident = NextToken();                                          //in case of an identifier
                     _codeGenerator.VariableRead(ident, _currentClass, _currentMethod); //tell the code gen to read the variable
                    break;
                default:                                                //if its neither of those 2 (its more complicated after this)
                    if (IsNextKeywordConst())                           //is the next token a keyword constant (eg. false)
                    {   
                        Token keywordConst = NextToken();               //store the keyword constant
                        switch (keywordConst.Value)
                        {           
                            case "true":                                //in case of true
                                _codeGenerator.True();                  //tell the code gen there'a a true
                                break;
                            case "false":                               //in case of false
                                _codeGenerator.False();                 //tell the code gen there's a false
                                break;
                            case "null":                                //in case of null
                                _codeGenerator.Null();                  //tell the code gen there's a null
                                break;
                            case "this":                                //in case of this
                                _codeGenerator.This();                  //tell the code gen there'a a this
                                break;
                            default:
                                throw new CompilerException("keywordconst not found");  //throw an error if the keyword is not in the switch statement
                        }
                    }                    
                    else if (PeekToken().Value == "(")                  //if the next token is a (
                    {
                        Match(new Token(TokenType.OPERATOR, "("));      //make sure there's a ( (principle reasons)
                        ParseExpression();                              //parse everything after it 
                        Match(new Token(TokenType.OPERATOR, ")"));      //make sure it ends with a )
                    }
                    else if (IsNextTokenUnaryOp())                      //if the next token is a unary operator
                    {
                        Token unaryOP = NextToken();                    //store it for later use
                        ParseTerm();                                    //parse the term after the operator
                        switch (unaryOP.Value)                          //switch the value of the operator
                        {
                            case "-":                                   //in case of a -
                                _codeGenerator.Negate();                //tell the code gen there's a negate
                                break;
                            case "!":                                   //in case of !
                                _codeGenerator.Not();                   //tell the code gen there's a not
                                break;
                        }
                    }
                    break;
            }
        }

        private Token NextToken() => _parseTokens[++_tokenCount];       //returns the next token from the tokenlist. increments the tokencounter before it does so 

        private Token PeekToken() => _parseTokens[_tokenCount + 1];     //returns the next token from the tokenlist without incrementing the tokencounter

        private bool IsNextKeywordConst()   
        {
            return PeekToken().Type == TokenType.KEYWORD &&
                   new[] {"true", "false", "null", "this"}.Contains(PeekToken().Value);
        }

        private bool IsNextTypeSymbol()
        {
            if (PeekToken().Type == TokenType.SYMBOL)
                return PeekToken().Value == ":";
            return false;
        }

        private bool IsNextTypeOrClass()
        {
            if (PeekToken().Type == TokenType.KEYWORD || PeekToken().Type == TokenType.IDENTIFIER)
                return new[] {"int", "string"}.Contains(PeekToken().Value);
            return false;
        }

        private bool IsNextTokenUnaryOp()
        {
            return PeekToken().Type == TokenType.OPERATOR && new[] {"-", "!"}.Contains(PeekToken().Value);
        }

        private bool IsNextMulOp()
        {
            return PeekToken().Type == TokenType.OPERATOR && new[] {"*", "/", "%"}.Contains(PeekToken().Value);
        }

        private bool IsNextAddOp()
        {
            return PeekToken().Type == TokenType.OPERATOR && new[] {"+", "-"}.Contains(PeekToken().Value);
        }

        private bool IsNextRelOp()
        {
            return PeekToken().Type == TokenType.OPERATOR &&
                   new[] {"<", ">", "?", "<=", ">=", "!="}.Contains(PeekToken().Value);
        }

        private bool IsNextLogicalOp()
        {
            return PeekToken().Type == TokenType.OPERATOR && new[] {"&", "|"}.Contains(PeekToken().Value);
        }

        private bool IsNextVarDecl()
        {
            if (PeekToken().Type == TokenType.KEYWORD)
                return PeekToken().Value == "let";
            return false;
        }

        private bool IsNextSubDecl()
        {
            if (PeekToken().Type == TokenType.KEYWORD)
                return PeekToken().Value == "function";
            return false;
        }

        private bool MatchWhile(params Token[] T)
        {
            bool found = false;
            try
            {
                while (true)
                {
                    //Console.WriteLine("Searching for: {0} Got: {1}", T.Value, PeekToken().Value);
                    foreach(Token t in T)
                    {
                        if (t.Value == PeekToken().Value)
                        {
                            found = true;
                            break;                            
                        }
                    }
                    if (found)
                    {
                        break;
                    }
                    _tokenCount++;
                }
                return true;
            }
            catch
            {
                throw new CompilerException("Correct token couldnt be found");
            }
        }

        private bool Match(Token T)
        {
            if (PeekToken().Value != T.Value || PeekToken().Type != T.Type)
            {
                throw new CompilerException("Expected: " + T.Value + " Got: " + PeekToken().Value);
            }
            _tokenCount++;
            return true;
        }
    }
}
