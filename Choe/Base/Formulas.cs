using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Choe
{
    public static class Formulas
    {
        private static Dictionary<string, string> formulasDictionary()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("1/sin(x)","-cotan(x)");
            //dictionary.Add("1/sin(x)^2","-cotan(x)");
            dictionary.Add("1/sqrt(a^2+x^2)","arcsin(x/a)");
            //dictionary.Add("1/sqrt(b^2+x^2)","arcsin(x/a)");
            return dictionary;
        }

        public static Dictionary<string, string> FormulasDictionary() => formulasDictionary();
    }
}
