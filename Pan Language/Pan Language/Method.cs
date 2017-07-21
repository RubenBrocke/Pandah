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
        public Dictionary<string, List<Variable>> MethodArrays;

        public Method(string name, int index)
        {
            MethodName = name;
            MethodIndex = index;
            Params = new Dictionary<string, Variable>();
            MethodVars = new Dictionary<string, Variable>();
            MethodArrays = new Dictionary<string, List<Variable>>();
        }

        public bool HasVariable(string varName)
        {
            return MethodVars.Any(kv => kv.Key == varName);
        }

        public bool HasArray(string varName)
        {
            return MethodArrays.Any(kv => kv.Key == varName);
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

        public Variable GetArrayVariable(string varName, int ArrayIndex)
        {
            foreach(KeyValuePair<string, List<Variable>> kv in MethodArrays)
            {
                if (kv.Key == varName)
                {
                    return kv.Value[ArrayIndex];
                }
            }
            return null;
        }

        public List<Variable> GetArray(string varName)
        {
            foreach (KeyValuePair<string, List<Variable>> kv in MethodArrays)
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

        public void SetArrayVariable(Variable variable, int ArrayIndex, object newvalue)
        {
            foreach (KeyValuePair<string, List<Variable>> kv in MethodArrays)
            {
                if (kv.Value[ArrayIndex] == variable)
                {
                    kv.Value[ArrayIndex].Value = newvalue;
                    return;
                }
            }
            throw new CompilerException("Cannot set variable value. Array doesnt exist");
        }
    }
}
