using System.Collections.Generic;

namespace Pan_Language
{
    internal class Variable
    {
        public static Dictionary<string, Method> Methods = new Dictionary<string, Method>();
        public static readonly List<Class> Classes = new List<Class>();
        public static Dictionary<string, Variable> ClassVars = new Dictionary<string, Variable>();

        public object Value { get; set; }
        public string Type { get; set; }

        //Add kind


        public Variable(object value)
        {
            Value = value;
        }             

        public static bool HasClass(string className)
        {
            try
            {
                int i = Classes.IndexOf(new Class(className));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Class GetClass(string className)
        {
            return Classes[Classes.IndexOf(new Class(className))];
        }
    }
}
