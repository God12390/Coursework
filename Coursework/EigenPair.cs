using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework
{
    internal class EigenPair
    {
        public double EigenValue { get; set; }
        public double[] EigenVector { get; set; }
        public string EigenVectorString => string.Join(", ", EigenVector);
    }
}
