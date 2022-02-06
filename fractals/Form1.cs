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
using System.IO;



namespace fractals
{
    class root
    {
        public int x; // defines root color 
        public int[,] colors = new int[15, 3] { { 32, 132, 140 }, { 68, 148, 76 }, { 28, 148, 196 }, { 64, 84, 140 }, { 32, 20, 100 }, { 82, 20, 100 }, { 139, 0, 0 }, { 255, 215, 0 }, { 92, 192, 192 }, { 255, 127, 80 }, { 0, 132, 140 }, { 32, 55, 190 }, { 32, 0, 140 }, { 32, 200, 120 }, { 32, 32, 32 } };
        public double real, img;
        public root(int x, double real, double img)
        {
            this.real = real;
            this.img = img;
            this.x = x;
        }
    }
    class element
    {
        public int power;
        public double preFactor = 0; //default
        public bool has_x = false;
        double[] output = new double[2];
        int[] prefactors;
        public element(string s) //diassembles imput string into constituents, e.g. 5x^5
        {
            if (s.Contains('x'))
            {
                has_x = true;
                string prefactor = "";
                int i = 0;
                while (s[i] != 'x')
                {
                    prefactor += s[i];
                    i++;
                }
                if (prefactor == "") prefactor += '1';
                preFactor = double.Parse(prefactor);
                prefactor = "";
                i += 2;
                while (i < s.Length)
                {
                    prefactor += s[i];
                    i++;
                }
                if (prefactor != "")
                {
                    power = int.Parse(prefactor);
                }
            }
            else
            {
                preFactor = double.Parse(s);
            }
            prefactors = get_prefactors(power);
        }
        public element(double prf, int power, bool cont)
        {
            preFactor = prf;
            has_x = cont;
            this.power = power;
            prefactors = get_prefactors(power);


        }
        int[] get_prefactors(int degree)
        {
            int[] x = new int[degree + 1];
            for (int i = 0; i <= degree; i++)
            {
                x[i] = (factorial(degree) / (factorial(i) * factorial(degree - i)));
            }
            //Thread.Sleep(1000000000);
            return x;
        }
        int factorial(int x)
        {
            int a = 1;
            for (int i = x; i > 1; i--)
            {
                a *= i;
            }
            return a;
        }
    }
    class polynomial
    {

        public List<element> vals = new List<element>(); //defined as a list that stores main polynomial
        List<char> op = new List<char>(); // + - etc
        public List<element> dals = new List<element>(); // new polynomial ( derivative of vals)
        double[] pol_val = new double[2];
        double[] pol_derv_val = new double[2];
        double[] output = new double[2];
        double real;
        double img;
        public polynomial(string s)// in the form ax^b+cx^y...
        {
            string obj = "";
            for (int i = 0; i < s.Length; i++) //breaks up imput string into polynomial elements
            {
                switch (s[i])
                {
                    case '+':
                        op.Add(s[i]);
                        vals.Add(new element(obj));
                        obj = "";
                        break;
                    case '-':
                        op.Add(s[i]);
                        vals.Add(new element(obj));
                        obj = "";
                        break;
                    default:
                        obj += s[i];
                        break;
                }
            }
            if (obj != "") vals.Add(new element(obj)); // constructs polynomial components
            for (int i = 1; i < vals.Count; i++)
            {
                if (op[i - 1] == '-')
                {
                    vals[i].preFactor = vals[i].preFactor * -1; //assigns symbols
                }
            }
            foreach (var a in vals) //calculates derivative by forming a new polynomial of elements
            {
                switch (a.has_x)
                {
                    case true:
                        switch (a.power)
                        {
                            case 0:
                                dals.Add(new element(a.preFactor, 0, false));
                                break;
                            default:
                                dals.Add(new element(a.preFactor * a.power, a.power - 1, true));
                                break;
                        }
                        break;
                    case false:
                        break;
                }
            }
        }
        public int get_degree()
        {
            List<int> degrees = new List<int>();
            foreach (var a in vals)
            {
                degrees.Add(a.power);
            }
            degrees.Sort();
            return degrees[degrees.Count - 1];
        }
        public double[] get_aproximation(double a, double b, root[] rt, root[] drt, double prefact)
        {
            img = a;
            real = b;
            real = a - rt[0].real;
            img = b - rt[0].img;
            for (int i = 0; i < rt.Length - 1; i++)
            {
                double u, v;
                u = real * (a - rt[i + 1].real) - img * (b - rt[i + 1].img);
                v = img * (a - rt[i + 1].real) + real * (b - rt[i + 1].img);
                real = u;
                img = v;              
            }                           
            pol_val[0] = real;
            pol_val[1] = img;
            img = b;
            real = a;  //reset values        
            for (int i = 0; i < drt.Length - 1; i++)
            {
                double u, v;
                u = real * (a - drt[i + 1].real) - img * (b - drt[i + 1].img);
                v = img * (a - drt[i + 1].real) + real * (b - drt[i + 1].img);
                real = u;
                img = v;
            }         
            pol_derv_val[0] = real*prefact;
            pol_derv_val[1] = img*prefact;
            output[0] = a - ((pol_val[1] * pol_derv_val[1] + pol_val[0] * pol_derv_val[0]) / (pol_derv_val[0] * pol_derv_val[0] + pol_derv_val[1] * pol_derv_val[1]));
            output[1] = b - (pol_val[1] * pol_derv_val[0] - pol_val[0] * pol_derv_val[1]) / (pol_derv_val[0] * pol_derv_val[0] + pol_derv_val[1] * pol_derv_val[1]);
            return output;
        }     
       
    }
    public partial class Form1 : Form
    {
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
            Console.WriteLine("enter degree");
            int pow = int.Parse(Console.ReadLine());
            string func = "x^15-1";
            string[] s = File.ReadAllText("IN.txt").Split(" ");          
            Bitmap myBitmap = new Bitmap("intel.png");
            polynomial u = new polynomial(func);
            root[] rs = new root[u.get_degree()];           
            Thread.Sleep(1000);
            root[] drs = new root[pow - 1];
            HashSet<Tuple<double, double>> roots = new HashSet<Tuple<double, double>>();
            int ko = 0;
            foreach (var a in s)
            {
                string first = "";
                string second = "";
                int j = 0;
                if (a[j] == '-') { first += '-'; j++; }
                while(a[j] != '+' && a[j] != '-')
                {                              
                    first += a[j];
                    
                    j++;
                }            
                for(int i = j; i < a.Length; i++)
                {
                    second += a[i];
                }
                
                rs[ko] = new root(ko, double.Parse(first), double.Parse(second));
                ko++;
            }
            int h = 0;
            foreach(var al in rs)
            {
                Console.WriteLine(al.real);
                Console.WriteLine(al.img);
                Console.WriteLine(h);
                h++;
            }
            for(int i = 0; i < pow-1; i++)
            {
                drs[i] = new root(i, 0, 0);
            }                      
            int ab = myBitmap.Width / 2;
            int ba = myBitmap.Height / 2;
      
            for (int Xcount = 0; Xcount < ab * 2; Xcount++) //sets pixel colors
            {

                for (int Ycount = 0; Ycount < ba * 2; Ycount++)
                {

                    get_root((double)3 * Xcount / ab - 3, ((double)3 * Ycount / ba - 3), Xcount, Ycount, ref u, ref rs,ref drs, myBitmap, pow);

                }           
            }
            e.Graphics.DrawImage(myBitmap, 0, 0, myBitmap.Width,
          myBitmap.Height);  
            return;
        }
        static void get_root(double x, double y, int A, int B, ref polynomial u, ref root[] a,ref root[]dr, Bitmap map, double prefact)
        {
            double[] nauja = new double[2];
            nauja[0] = x;
            nauja[1] = y;
            int i = 0;
            bool br = false;

            while (i < 30 && br == false)
            {
                nauja = u.get_aproximation(nauja[0], nauja[1], a, dr, prefact);              
                i++;
            }
            List<double> dist = new List<double>(3);
            foreach (var b in a)
            {
                dist.Add((nauja[0]-b.real) * (nauja[0]-b.real) + (nauja[1]-b.img) * (nauja[1]-b.img));
            }
            double z = dist.Min();
            for(int j = 0; j < a.Length; j++)
            {
                if (dist[j] == z) map.SetPixel(A, B, Color.FromArgb(a[j].colors[a[j].x, 0], a[j].colors[a[j].x, 1], a[j].colors[a[j].x, 2]));
            }
        }
    }
}