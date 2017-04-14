using System.Collections.Generic;
using System.Linq;

namespace Pan_Language
{
    internal class Method
    {
        public string MethodName { get; }
        public int MethodIndex { get; set; }
        public readonly Dictionary<string, Variable> Params;
        public Dictionary<string, Variable> MethodVars;

        public Method(string name, int index)
        {
            MethodName = name;
            MethodIndex = index;
            Params = new Dictionary<string, Variable>();
            MethodVars = new Dictionary<string, Variable>();
        }

        public bool HasVariable(string varName)
        {
            return MethodVars.Any(kv => kv.Key == varName);
        }

        public Variable GetVariable(string varName)
        {
            foreach (KeyValuePair<string,Variable> kv in MethodVars)
            {
                if (kv.Key == varName)
                {
                    return kv.Value;
                }
            }
            return null;
        }

        public void SetVariable(Variable variable, object newvalue)
        {
            foreach (KeyValuePair<string, Variable> kv in MethodVars)
            {
                if (kv.Value == variable)
                {
                    kv.Value.Value = newvalue;
                    return;
                }
            }
            throw new CompilerException("Cannot set variable value. Variable doesnt exist");
        }
    }
}
