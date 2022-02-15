using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace fractals
{
    public class Block
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public double Tolerance { get; private set; }

        private unsafe void* blockPointer;
        private unsafe Int32** blockData = null;

        public Block(int Width, int Height, double Tolerance)
        {
            this.Width = Width;
            this.Height = Height;
            this.Tolerance = Tolerance;

            unsafe
            {
                blockPointer = allocate_block(Width, Height, Tolerance);
            }
        }

        ~Block()
        {
            unsafe { free_block(blockPointer); }
        }

        public void GenerateData(ComplexNumber a, ComplexNumber b, int iterations)
        {
            unsafe
            {
                blockData = null;
                generator_generate(a.real, a.imag, b.real, b.imag, iterations, blockPointer);
                blockData = generator_read_block(blockPointer, out _, out _, out _);
            }
        }

        public unsafe int GetData(int i, int j)
        {
            if (blockData == null) throw new Exception("Data not generated");
            if (i < 0 || j < 0 || i >= Width || j >= Height) throw new IndexOutOfRangeException();
            return blockData[i][j];
        }

        [DllImport(@"GeneratorLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static unsafe extern void* allocate_block(Int32 width, Int32 height, double compare_tolerance);
        [DllImport(@"GeneratorLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static unsafe extern void free_block(void* block);
        [DllImport(@"GeneratorLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static unsafe extern void generator_generate(double ax, double ay, double cx, double cy, Int32 accuracy, void* block);
        [DllImport(@"GeneratorLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static unsafe extern Int32** generator_read_block(void* block, out Int32 width, out Int32 height, out double compare_tolerance);

    }

    internal static class DllWrapper
    {
        public static void LoadPolynomial(Polynomial p)
        {
            Int32[] coefficients = new Int32[p.terms.Count];
            Int32[] powers = new Int32[p.terms.Count];

            for (int i = 0; i < p.terms.Count; i++)
            {
                coefficients[i] = p.terms[i].coeff;
                powers[i] = p.terms[i].power;
            }

            if(!generator_load_polynomial(coefficients, powers, p.terms.Count))
            {
                throw new Exception("Failed to find roots for the polynomial");
            }
        }

        [DllImport(@"GeneratorLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static unsafe extern bool generator_load_polynomial(Int32[] coefficients, Int32[] powers, Int32 len);
    }
}
