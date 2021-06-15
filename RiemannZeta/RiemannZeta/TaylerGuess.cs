using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiemannZeta
{
    class TaylerGuess
    {
        static List<double> zeros;
        static List<double> coe; // Coefficient
        static TaylerGuess()
        {
            zeros = Zeta.LoadZeros();
        }

        public static double CalcOneDiff(int num)
        {
            double sum = 0;
            for(int i=0; i< coe.Count;i++)
            {
                sum += coe[i] * Math.Pow(num, i);
            }
            return Math.Abs(sum - zeros[num]);
        }

        public static double SumTrainDiff()
        {
            return 0;
        }

        public static double SumTestDiff()
        {
            return 0;
        }
    }
}
