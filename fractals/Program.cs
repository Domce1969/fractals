using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace fractals
{
    static class Program
    {
        public const double tolerance = 1e-2;
        public const int width = 2000;
        public const int height = 2000;
        public const int iterations = 1000;
        public const string savePath = @"R:\bitmap.png";


        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        //[STAThread]
        static void Main()
        {
            Polynomial p = Polynomial.ParseString("x^3-1");
            Console.WriteLine(p);

            int stepsize = 250 / (p.Degree + 1);
            Color[] colors = (from i in Enumerable.Range(0, p.Degree + 1) select Color.FromArgb(255 - i * stepsize, 255 - i * stepsize, 255 - i * stepsize)).ToArray();
            
            Bitmap img = new Bitmap(width, height);

            ComplexNumber topLeft = new ComplexNumber() { real = -3, imag = -3 };
            ComplexNumber bottomRight = new ComplexNumber() { real = 3, imag = 3 };

            Stopwatch sw = Stopwatch.StartNew();

            Console.WriteLine("Initializing block...");
            Block mb = new Block(width, height, tolerance);
            Console.WriteLine("Loading polynomial...");
            DllWrapper.LoadPolynomial(p);
            Console.WriteLine("Generating block...");
            mb.GenerateData(topLeft, bottomRight, iterations);

            sw.Stop();
            Console.WriteLine(sw.Elapsed);

            for (int i = 0; i < img.Width; i++)
            {
                for(int j = 0; j < img.Height; j++)
                {
                    img.SetPixel(i, j, colors[mb.GetData(i, j) + 1]);
                }
            }

            img.Save(savePath);
        }
    }
}
