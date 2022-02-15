using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace fractals
{
    internal static class DllWrapper
    {
        private static Int32 BlockWidth, BlockHeight;
        private static double CompareTolerance;

        public static void GeneratorInit(Int32 width, Int32 height, double compare_tolerance)
        {
            BlockWidth = width;
            BlockHeight = height;
            CompareTolerance = compare_tolerance;
            generator_init(width, height, compare_tolerance);
        }

        public static void LoadPolynomial(Polynomial p)
        {
            Int32[] coefficients = new Int32[p.terms.Count];
            Int32[] powers = new Int32[p.terms.Count];

            for (int i = 0; i < p.terms.Count; i++)
            {
                coefficients[i] = p.terms[i].coeff;
                powers[i] = p.terms[i].power;
            }

            generator_load_polynomial(coefficients, powers, p.terms.Count);
        }

        public unsafe static Int32[,] GenerateBlock(ComplexNumber a, ComplexNumber b, Int32 accuracy)
        {
            Int32[,] outblock = new Int32[BlockWidth, BlockHeight];
            generator_generate(a.real, a.imag, b.real, b.imag, accuracy);
            Int32** result = generator_read_results();
            for(int i = 0; i < BlockWidth; i++)
            {
                for(int j = 0; j < BlockHeight; j++)
                {
                    outblock[i, j] = result[i][j];
                }
            }
            return outblock;
        }


        [DllImport(@"GeneratorLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static unsafe extern void generator_init(Int32 width, Int32 height, double compare_tolerance);
        [DllImport(@"GeneratorLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static unsafe extern void generator_load_polynomial(Int32[] coefficients, Int32[] powers, Int32 len);
        [DllImport(@"GeneratorLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static unsafe extern void generator_generate(double ax, double ay, double cx, double cy, Int32 accuracy);
        [DllImport(@"GeneratorLib.dll", CallingConvention = CallingConvention.Cdecl)]

        private static unsafe extern Int32** generator_read_results();
    }
}
