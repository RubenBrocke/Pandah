using System.Collections.Generic;
using System.Linq;

namespace Pan_Language
{
    internal class Class
    {
        public string ClassName { get; }
        public List<Method> ClassMethods { get; }
        public Dictionary<string, Variable> ClassVars;
        public List<string> instanceIDs;

        public Class(string className)
        {
            ClassName = className;
            ClassMethods = new List<Method>();
            ClassVars = new Dictionary<string, Variable>();
            instanceIDs = new List<string>();
        }

        public bool HasMethod(string methodName)
        {
            return ClassMethods.Any(m => m.MethodName == methodName);
        }

        public Method GetMethod(string methodName)
        {
            return ClassMethods.FirstOrDefault(m => m.MethodName == methodName);
        }

        public bool HasVariable(string varName)
        {
            return ClassVars.Any(kv => kv.Key == varName);
        }

        public Variable GetVariable(string varName)
        {
            foreach (KeyValuePair<string, Variable> kv in ClassVars)
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
            foreach (KeyValuePair<string, Variable> kv in ClassVars)
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
