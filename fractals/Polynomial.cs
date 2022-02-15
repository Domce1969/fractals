using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;

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
        public static Term ParseString(string s)
        {
            int coeff, power;
            int xpos = s.IndexOf('x');
            if(xpos == -1)
            {
                coeff = int.Parse(s);
                power = 0;
            }
            else if(xpos == s.Length - 1)
            {
                coeff = int.Parse(s.Substring(0, s.Length - 1));
                power = 1;
            }
            else
            {
                string coeffstr = s.Substring(0, xpos);
                if (!coeffstr.Any(c => char.IsDigit(c))) coeffstr += "1";
                coeff = int.Parse(coeffstr);
                power = int.Parse(s.Substring(xpos + 2));
            }
            return new Term(coeff, power);
        }
        public Term(int coeff, int power)
        {
            this.coeff = coeff;
            this.power = power;
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
        public List<Term> terms = new List<Term>();

        public int Degree { get { return terms.Max(v => v.power); } }

        private Polynomial() { }

        public static Polynomial ParseString(string s)
        {
            Polynomial ans = new Polynomial();
            s = s.Replace(" ", string.Empty);
            if (s[0] != '+' && s[0] != '-') s = '+' + s;

            int lastIndex = 0;
            for (int i = 1; i < s.Length; i++)
            {
                if(s[i] == '+' || s[i] == '-')
                {
                    ans.terms.Add(Term.ParseString(s.Substring(lastIndex, i - lastIndex)));
                    lastIndex = i;
                }
            }
            ans.terms.Add(Term.ParseString(s.Substring(lastIndex)));
            return ans;
        }

        public override string ToString()
        {
            return string.Join("", from t in terms select t.ToString());
        }
    }
}
