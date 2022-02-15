using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;



namespace fractals
{    
    public partial class Form1 : Form
    {
        public static double tolerance = 1e-2;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            AllocConsole();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
        private void Form1_Paint(object sender, PaintEventArgs e)
        {       
             
            AllocConsole();

            //Console.WriteLine("lol");
            //Bitmap myBitmap = new Bitmap(200, 200);
            //Polynomial u = new Polynomial("x^5-1");
            //List<root> roots = get_roots(u); //root finding algorithm
            //foreach(var r in roots)
            //{
            //    Console.WriteLine($"Root {r.real} {r.img}");
            //}
            //if(roots.Count != u.get_degree())
            //{
            //    Console.WriteLine($"Not all roots found! {roots.Count}/{u.get_degree()}");
            //}
            //Stopwatch stop = new Stopwatch();
            //stop.Start();
            //int ab = myBitmap.Width / 2;
            //int ba = myBitmap.Height / 2;
            //int nonSet = 0;
            //for (int Xcount = 0; Xcount <ab*2; Xcount++) //sets pixel colors
            //{
                
            //    for (int Ycount = 0; Ycount < ba*2; Ycount++)
            //    {
            //        root g = get_root((double)3 * Xcount / ab - 3, ((double)3 * Ycount / ba - 3), u, roots);
            //        if (g != null) myBitmap.SetPixel(Xcount, Ycount, Color.FromArgb(g.colors[g.x, 0], g.colors[g.x, 1], g.colors[g.x, 2])); // remove others for full precision    
            //        else nonSet++;
            //    }
            //    e.Graphics.DrawImage(myBitmap, 0, 0, myBitmap.Width, myBitmap.Height);
            //}
            //Console.WriteLine($"Non set: {nonSet}");
            //return;
        }
    }
 }


