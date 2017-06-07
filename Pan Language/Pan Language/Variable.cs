using System;
using System.Collections.Generic;

namespace Pan_Language
{
    internal class Variable
    {
        public object Value { get; set; }
        public Type Type { get; set; }

        //Add kind


        public Variable(object value)
        {
            Value = value;
            try
            {
                Type = value.GetType();
            }
            catch
            {
                //do nothing
            }
        }
    }
}
