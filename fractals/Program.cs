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
        public const int MaxFactorialValue = 12;
        public const double tolerance = 1e-2;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        //[STAThread]
        static void Main()
        {
            Polynomial p = Polynomial.ParseString("x^3-1");
            Console.WriteLine(p);
            Color[] colors = (from i in Enumerable.Range(0, 20) select Color.FromArgb(255 - i * 10, 255 - i * 10, 255 - i * 10)).ToArray();
            
            Bitmap img = new Bitmap(2000, 2000);

            ComplexNumber topLeft = new ComplexNumber() { real = -3, imag = -3 };
            ComplexNumber bottomRight = new ComplexNumber() { real = 3, imag = 3 };

            Stopwatch sw = Stopwatch.StartNew();

            Console.WriteLine("Initializing block...");
            Block mb = new Block(2000, 2000, 1e-2);
            Console.WriteLine("Loading polynomial...");
            DllWrapper.LoadPolynomial(p);
            Console.WriteLine("Generating block...");
            mb.GenerateData(topLeft, bottomRight, 1000);

            sw.Stop();
            Console.WriteLine(sw.Elapsed);

            for (int i = 0; i < img.Width; i++)
            {
                for(int j = 0; j < img.Height; j++)
                {
                    img.SetPixel(i, j, colors[mb.GetData(i, j) + 1]);
                }
            }

            img.Save("R:\\bitmap.png");
        }
    }
}
