namespace Pan_Language
{
    interface ICodeGenerator
    {
        void Return();
        void BeginIf();
        void PossibleElse();
        void EndIf();
        void Assign(Token varName, Class c, Method m = null, int arrayindex = -1);
        void Add();
        void Sub();
        void Mul();
        void Div();
        void Mod();
        void And();
        void Or();
        void Less();
        void Greater();
        void Equal();
        void LessOrEqual();
        void GreaterOrEqual();
        void NotEqual();
        void True();
        void False();
        void Null();
        void This();
        void Negate();
        void Not();
        void VariableRead(Token varName, Class c, Method m = null);
        void Call(string className, string subroutineName);
    }
}
