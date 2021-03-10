using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiemannZeta
{
    public class Prime
    {
        public static List<int> GeneratePrimesNaive(int n)
        {
            List<int> primes = new List<int>();
            primes.Add(2);
            int nextPrime = 3;
            while (primes.Count < n)
            {
                int sqrt = (int)Math.Sqrt(nextPrime);
                bool isPrime = true;
                for (int i = 0; (int)primes[i] <= sqrt; i++)
                {
                    if (nextPrime % primes[i] == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }
                if (isPrime)
                {
                    primes.Add(nextPrime);
                }
                nextPrime += 2;
            }
            return primes;
        }


        public static List<int> CalcPrimeCount(List<int> primes)
        {
            List<int> ret = new List<int>();
            ret.Add(0);
            ret.Add(0);
            for (int i = 0; i < primes.Count - 1; i++)
            {
                for (int j = primes[i]; j < primes[i + 1]; j++)
                    ret.Add(i + 1);
            }
            return ret;
        }
    }
}
