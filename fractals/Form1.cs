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
        double pow(double x, int a)
        {        
            double b = 1;
            for(int i = 1; i <= a; i++)
            {
                b *= x;
            }
            return b;
        }
        public double[] get_values(double a, double b) // a => real, b=> complex
        {
            output[0] = 0;//real
            output[1] = 0; //complex       
            for (int i = 0; i <= power; i++)
            {
                if (i % 2 == 0) //based on property i*i = -1, basically checks the sign
                {
                    if (i % 4 != 0)
                    {
                         //output[0] -= preFactor *  prefactors[i] * Math.Pow(a, power - i) * Math.Pow(b, i);
                        output[0] -= preFactor * prefactors[i] * pow(a, power - i) * pow(b, i);
                    }
                    else
                    {
                        output[0] += preFactor * prefactors[i] * pow(a, power - i) * pow(b, i);
                    }
                }
                else
                {
                    if ((i - 1) % 4 != 0) // based on property i*i*i = -1i and i^4 = 1
                    {
                        //output[1] -= preFactor * prefactors[i] * Math.Pow(a, power - i) * Math.Pow(b, i);
                        output[1] -= preFactor * prefactors[i] * pow(a, power - i) * pow(b, i);

                    }
                    else
                    {
                        output[1] += preFactor * prefactors[i] * pow(a, power - i) * pow(b, i);
                        //output[1] += preFactor * prefactors[i] * Math.Pow(a, power - i) * Math.Pow(b, i);
                    }
                }

            }              
            return output;
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
        public double[] get_aproximation(double a, double b) //main function, employs x1 = x0-P(x)/P'(x)
        {
            img = 0;
            real = 0;
            foreach (var el in vals)
            {
                if (el.has_x)
                {
                    if (el.power > 1)
                    {
                        real += el.get_values(a, b)[0];
                        img += el.get_values(a, b)[1];
                    }
                    else
                    {
                        real += el.preFactor * a;
                        img += el.preFactor * b;
                    }
                }
                else
                {
                    real += el.preFactor;
                }
            }
            pol_val[0] = real;
            pol_val[1] = img;
            img = 0;
            real = 0;  //reset values            
            foreach (var el in dals)
            {
                if (el.has_x)
                {
                    if (el.power > 1)
                    {
                        double[] x = el.get_values(a, b);
                        real += x[0];
                        img += x[1];
                    }
                    else
                    {
                        real += el.preFactor * a;
                        img += el.preFactor * b;
                    }
                }
                else
                {
                    real += el.preFactor;
                }
            }
            pol_derv_val[0] = real;
            pol_derv_val[1] = img;
            output[0] = a - ((pol_val[1] * pol_derv_val[1] + pol_val[0] * pol_derv_val[0]) / (pol_derv_val[0] * pol_derv_val[0] + pol_derv_val[1] * pol_derv_val[1]));
            output[1] = b - (pol_val[1] * pol_derv_val[0] - pol_val[0] * pol_derv_val[1]) / (pol_derv_val[0] * pol_derv_val[0] + pol_derv_val[1] * pol_derv_val[1]);         
            return output;
        }
    }
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
            string s = "x^5-1";
            Console.WriteLine("lol");
            Bitmap myBitmap = new Bitmap(@"R:\imgsmall.png");
            polynomial u = new polynomial(s);
            List<root> roots = get_roots(u); //root finding algorithm
            if(roots.Count != u.get_degree())
            {
                Console.WriteLine($"Not all roots found! {roots.Count}/{u.get_degree()}");
            }
            Stopwatch stop = new Stopwatch();
            stop.Start();
            int ab = myBitmap.Width / 2;
            int ba = myBitmap.Height / 2;
            int nonSet = 0;
            for (int Xcount = 0; Xcount <ab*2; Xcount++) //sets pixel colors
            {
                
                for (int Ycount = 0; Ycount < ba*2; Ycount++)
                {
                    root g = get_root((double)3 * Xcount / ab - 3, ((double)3 * Ycount / ba - 3), Xcount, Ycount, u, roots);
                    if (g != null) myBitmap.SetPixel(Xcount, Ycount, Color.FromArgb(g.colors[g.x, 0], g.colors[g.x, 1], g.colors[g.x, 2])); // remove others for full precision    
                    else nonSet++;
                }
                e.Graphics.DrawImage(myBitmap, 0, 0, myBitmap.Width, myBitmap.Height);
            }
            Console.WriteLine($"Non set: {nonSet}");
            return;
        }
        static List<root> get_roots(polynomial u)
        {
            int degree = u.get_degree();
            List<root> roots = new List<root>();
            for (double i = 0; i < 300; i++)
            {
                for (double k = 0; k < 300; k++)
                {
                    double[] x = getroots((k - 150) / 75, (i - 150) / 75, u); //random gen prolly better

                    if(!roots.Any(r => doubles_equal(r.real, x[0]) && doubles_equal(r.img, x[1])))
                    {
                        roots.Add(new root(roots.Count, x[0], x[1]));
                    }
                    if (roots.Count >= degree) return roots;
                }
            }

            return roots;
        }
        static double[] getroots(double x, double y, polynomial u)
        {
            double[] nauja = new double[2];
            nauja[0] = x;
            nauja[1] = y;
            int i = 0;
            while (i < 500) //100 permutations should be enough
            {
                nauja = u.get_aproximation(nauja[0], nauja[1]);
                i++;
            }
            return nauja;
        }
        static bool doubles_equal(double a, double b)
        {
            return Math.Abs(a - b) <= tolerance;
        }
        static root get_root(double x, double y, int A, int B, polynomial u, List<root> a)
        {
            double[] nauja = new double[2];
            nauja[0] = x;
            nauja[1] = y;
            int i = 0;
           
            while (i < 1000)
            {
                nauja = u.get_aproximation(nauja[0], nauja[1]);
                foreach (var g in a)
                {
                    if (doubles_equal(nauja[0], g.real) && doubles_equal(nauja[1], g.img))
                    {
                        return g;
                    }
                } 
                i++;
            }
            return null;
        }
    }
 }


