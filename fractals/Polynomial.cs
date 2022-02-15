using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace fractals
{
    public struct ComplexNumber
    {
        public double real;
        public double imag;
    }

    public class Term
    {
        public int power;
        public int coeff;
        public Term(string s) //diassembles imput string into constituents, e.g. 5x^5
        {
            int xpos = s.IndexOf('x');
            if(xpos == -1)
            {
                power = 0;
                coeff = int.Parse(s);
            }else if(xpos == s.Length - 1)
            {
                power = 1;
                coeff = int.Parse(s.Substring(0, s.Length - 1));
            }
            else
            {
                coeff = int.Parse(s.Substring(0, xpos));
                power = int.Parse(s.Substring(xpos + 2));
            }
        }
        public Term(int coeff, int power)
        {
            this.coeff = coeff;
            this.power = power;
        }

        public void Evaluate(ref ComplexNumber cn)
        {
            double r = cn.real;
            double im = cn.imag;
            if (power == 0)
            {
                cn.real = 1;
                cn.imag = 0;
            }
            else
            {
                for (int i = 1; i < power; i++)
                {
                    double newr = (r * cn.real - im * cn.imag);
                    double newi = (r * cn.imag + im * cn.real);
                    cn.real = newr;
                    cn.imag = newi;
                }
            }
            cn.real *= coeff;
            cn.imag *= coeff;
        }

        public override string ToString()
        {
            string coeffstr = (coeff < 0 ? "" : "+") + coeff.ToString();
            if (power == 0) return coeffstr;
            else if (power == 1) return coeffstr + "x";
            else return coeffstr + "x^" + power.ToString();
        }
    }

    public class Polynomial
    {
        private List<Term> terms = new List<Term>();

        public int Degree { get { return terms.Max(v => v.power); } }
        private Polynomial derivative = null;
        private List<ComplexNumber> roots;

        private Polynomial() { }

        public Polynomial(string s) // in the form ax^b+cx^y...
        {
            if (s[0] != '+' && s[0] != '-') s = '+' + s;

            int lastIndex = 0;
            for (int i = 1; i < s.Length; i++)
            {
                if(s[i] == '+' || s[i] == '-')
                {
                    terms.Add(new Term(s.Substring(lastIndex, i - lastIndex)));
                    lastIndex = i;
                }
            }
            terms.Add(new Term(s.Substring(lastIndex)));
        }

        public List<ComplexNumber> GetRoots()
        {
            if (roots != null) return roots;
            roots = new List<ComplexNumber>();
            for (double i = -2; i <= 2; i += 0.05)
            {
                for(double j = -2; j <= 2; j += 0.05)
                {
                    ComplexNumber cn = new ComplexNumber() { real = i, imag = j };
                    for(int s = 0; s < 500; s++)
                    {
                        ApproximationStep(ref cn);
                    }

                    if (!roots.Any(r => Program.ComplexNumbersEqual(r, cn)))
                        roots.Add(cn);
                }
            }

            return roots;
        }

        public Polynomial TakeDerivative()
        {
            if(derivative != null) return this.derivative;

            derivative = new Polynomial();
            foreach(Term t in terms)
            {
                if(t.power > 0)
                {
                    derivative.terms.Add(new Term(t.coeff * t.power, t.power - 1));
                }
            }
            return derivative;
        }
        public override string ToString()
        {
            return string.Join("", from t in terms select t.ToString());
        }

        public void Evaluate(ref ComplexNumber cn)
        {
            ComplexNumber outn = new ComplexNumber() { real = 0, imag = 0 };
            foreach(Term t in terms)
            {
                ComplexNumber add = new ComplexNumber() { real = cn.real, imag = cn.imag };
                t.Evaluate(ref add);
                outn.real += add.real;
                outn.imag += add.imag;
            }
            cn.real = outn.real;
            cn.imag = outn.imag;
        }

        public void ApproximationStep(ref ComplexNumber number)
        {
            ComplexNumber a = new ComplexNumber() { real = number.real, imag = number.imag };
            ComplexNumber b = new ComplexNumber() { real = number.real, imag = number.imag };

            Evaluate(ref a);
            TakeDerivative().Evaluate(ref b);

            double denom = (b.real * b.real + b.imag * b.imag);

            number.real -= (a.real * b.real + a.imag * b.imag) / denom;
            number.imag -= (a.imag * b.real - a.real * b.imag) / denom;
        }
    }
}
