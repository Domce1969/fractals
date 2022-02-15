using System;
using System.Collections.Generic;
using System.Text;

namespace fractals
{
    static class Factorial
    {
        private static List<int> values;
        static Factorial()
        {
            values = new List<int>() { 0 };
            while(values.Count <= Program.MaxFactorialValue)
            {
                values.Add(values[values.Count - 1] * values.Count);
            }
        }
        static int GetValue(int x)
        {
            return values[x];
        }
    }
}
