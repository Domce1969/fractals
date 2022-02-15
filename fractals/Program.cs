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

        public static bool DoublesEqual(double a, double b)
        {
            return Math.Abs(a - b) <= tolerance;
        }

        public static bool ComplexNumbersEqual(ComplexNumber a, ComplexNumber b)
        {
            return DoublesEqual(a.real, b.real) && DoublesEqual(a.imag, b.imag);
        }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        //[STAThread]
        static void Main()
        {
            Polynomial p = new Polynomial("1x^3-1");
            Color[] colors = new Color[] { Color.Black, Color.Red, Color.Green, Color.Blue, Color.Purple };

            var roots = p.GetRoots();
            foreach(var root in roots)
            {
                Console.WriteLine($"{root.real} + {root.imag}i");
            }

            foreach(var root in roots)
            {
                ComplexNumber cpy = new ComplexNumber() { real = root.real, imag = root.imag };
                p.Evaluate(ref cpy);
                Console.WriteLine(cpy.real * cpy.real + cpy.imag * cpy.imag);
            }

            Bitmap img = new Bitmap(2000, 2000);

            ComplexNumber topLeft = new ComplexNumber() { real = -3, imag = -3 };
            ComplexNumber bottomRight = new ComplexNumber() { real = 3, imag = 3 };

            ComplexNumber diff = new ComplexNumber() { 
                real = (bottomRight.real - topLeft.real) / img.Width,
                imag = (bottomRight.imag - topLeft.imag) / img.Height
            };

            Stopwatch sw = Stopwatch.StartNew();

            for(int i = 0; i < img.Width; i++)
            {
                for(int j = 0; j < img.Height; j++)
                {
                    ComplexNumber cn = new ComplexNumber() { real = topLeft.real + diff.real * i, imag = topLeft.imag + diff.imag * j };
                    int color = FractalGenerator.GeneratePixel(p, ref cn, 1000) + 1;
                    img.SetPixel(i, j, colors[color]);
                }
            }

            sw.Stop();
            Console.WriteLine(sw.Elapsed);

            img.Save("R:\\bitmap.png");

            //Application.SetHighDpiMode(HighDpiMode.SystemAware);
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
        }
    }
}
