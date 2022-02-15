using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace fractals
{
    struct RenderChunk
    {
        public int x, y;
        public ComplexNumber a;
        public ComplexNumber b;
    }
    static class Program
    {
        public const double tolerance = 1e-2;
        public const int width = 2048;
        public const int height = 2048;
        public const int iterations = 1000;
        public const string savePath = @"R:\bitmap.png";

        public static readonly int threadCount = Environment.ProcessorCount;
        public const int blockDivision = 8;


        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        //[STAThread]
        static void Main()
        {
            Console.WriteLine($"Running with {threadCount} threads");

            Polynomial p = Polynomial.ParseString("x^21+17x^14-23x^9+15x^2-2x-1");
            Console.WriteLine(p);

            int stepsize = 250 / (p.Degree + 1);
            Color[] colors = (from i in Enumerable.Range(0, p.Degree + 1) select Color.FromArgb(255 - i * stepsize, 255 - i * stepsize, 255 - i * stepsize)).ToArray();
            
            Bitmap img = new Bitmap(width, height);

            ComplexNumber topLeft = new ComplexNumber() { real = -3, imag = -3 };
            ComplexNumber bottomRight = new ComplexNumber() { real = 3, imag = 3 };

            Stopwatch sw = Stopwatch.StartNew();

            Console.WriteLine("Loading polynomial...");
            DllWrapper.LoadPolynomial(p);

            Console.WriteLine("Initializing blocks...");
            Block[] blocks = (from _ in Enumerable.Range(0, threadCount) select new Block(width / blockDivision, height / blockDivision, tolerance)).ToArray();

            Console.WriteLine("Preparing jobs...");
            int[,] outarr = new int[width, height];

            Queue<RenderChunk> jobs = new Queue<RenderChunk>();
            double xsize = (bottomRight.real - topLeft.real) / blockDivision;
            double ysize = (bottomRight.imag - topLeft.imag) / blockDivision;
            for(int i = 0; i < blockDivision; i++)
            {
                for (int j = 0; j < blockDivision; j++)
                {
                    jobs.Enqueue(new RenderChunk()
                    {
                        x = i,
                        y = j,
                        a = new ComplexNumber() { real = topLeft.real + xsize * i, imag = topLeft.imag + ysize * j },
                        b = new ComplexNumber() { real = topLeft.real + xsize * (i + 1), imag = topLeft.imag + ysize * (j + 1) }
                    });
                }
            }

            Console.WriteLine("Generating blocks...");
            Parallel.ForEach(blocks, delegate (Block b)
            {
                while(true)
                {
                    RenderChunk job;
                    lock (jobs)
                    {
                        if (jobs.Count == 0) break;
                        job = jobs.Dequeue();
                    }

                    b.GenerateData(job.a, job.b, iterations);
                    for (int i = 0; i < b.Width; i++)
                    {
                        for (int j = 0; j < b.Height; j++)
                        {
                            outarr[job.x * (width / blockDivision) + i, job.y * (height / blockDivision) + j] = b.GetData(i, j);
                        }
                    }
                }
                
            });

            sw.Stop();
            Console.WriteLine(sw.Elapsed);

            for (int i = 0; i < img.Width; i++)
            {
                for(int j = 0; j < img.Height; j++)
                {
                    img.SetPixel(i, j, colors[outarr[i, j] + 1]);
                }
            }

            img.Save(savePath);
        }
    }
}
