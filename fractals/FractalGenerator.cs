using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fractals
{
    public static class FractalGenerator
    {
        public static int GeneratePixel(Polynomial p, ref ComplexNumber cn, int accuracy)
        {
            var roots = p.GetRoots();
            for(int i = 0; i < accuracy; i++)
            {
                p.ApproximationStep(ref cn);
                for(int j = 0; j < roots.Count; j++)
                {
                    if (Program.ComplexNumbersEqual(roots[j], cn)) return j;
                }
            }
            return -1;
        }
    }
}
