using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pan_Language
{
    static class Global
    {
        public static List<Class> Classes = new List<Class>();

        public static Class GetClassById(string idName)
        {
            foreach(Class c in Classes)
            {
                if (c.instanceIDs.Any(s => s == idName))
                {
                    return c;
                }
            }
            return null;
        }

        public static Class GetClass(string className)
        {
            return Classes.Find(c => c.ClassName == className);
        }
    }
}
