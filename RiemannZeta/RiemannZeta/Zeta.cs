using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace RiemannZeta
{
    public class Zeta
    {
        public delegate double f(int n);
        public delegate double Slot(double one);
        public delegate double Template(List<Slot> slots, List<double> opt, int n);

        static List<double> zeros;
        static List<int> primes;
        static List<int> primecount;
        static List<double> o;
        static List<Slot> allSlots;
        static Dictionary<Template, int> slotCounts;
        static Dictionary<Template, int> optCounts;


        static Zeta()
        {
            zeros = LoadZeros();
            primes = Prime.GeneratePrimesNaive(100000);
            primecount = Prime.CalcPrimeCount(primes);
            o = new List<double>(new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
            PrepareSlotFunctions();
            slotCounts = new Dictionary<Template, int>();
            optCounts = new Dictionary<Template, int>();
            slotCounts[TemplateOne] = 2;
            optCounts[TemplateOne] = 2;
            slotCounts[TemplateTwo] = 2;
            optCounts[TemplateTwo] = 5;
            slotCounts[TemplateThree] = 6;
            optCounts[TemplateThree] = 10;
            slotCounts[TemplateFour] = 4;
            optCounts[TemplateFour] = 9;
        }

        public static double TemplateOne(List<Slot> slots, List<double> opt,  int n)
        {
            return opt[0]* slots[0](slots[1](opt[1]*primes[n]));
        }

        public static double TemplateTwo(List<Slot> slots, List<double> opt, int n)
        {
            return opt[0] 
                + opt[1]* slots[0](opt[2] *n)
                + opt[3] * slots[1](opt[4] * primes[n]);
        }

        public static double TemplateThree(List<Slot> slots, List<double> opt, int n)
        {
            return opt[0]
                + opt[1] * slots[0](opt[2] * n)
                + opt[3] * slots[1](opt[4] * primes[n])
                + opt[4] * slots[2](slots[3](opt[5] * n) + opt[6])
                + opt[7] * slots[4](slots[5](opt[8] * primes[n]) + opt[9]);

        }

        public static double TemplateFour(List<Slot> slots, List<double> opt, int n)
        {
            return opt[0]
                + opt[1] * slots[0](opt[2] * n)
                + opt[3] * slots[1](opt[4] * primes[n])
                + opt[5] * slots[2](opt[6] * n)
                + opt[7] * slots[3](opt[8] * primes[n]);
        }

        public static void PrintPolynomial()
        {
            Polynomial current = new Polynomial();
            current.Initiate(new Complex(-0.5,- zeros[0]));
            for(int i=0; i<zeros.Count-1; i++)
            {
                current = current.MultiplySimple(new Complex(-0.5, -zeros[i + 1]));
                current.TraceAllItems();
            }

        }

        public static void PrepareSlotFunctions()
        {
            allSlots = new List<Slot>();
            allSlots.Add(Math.Sin);
            allSlots.Add(x=>x<30&&x>=0? Math.Exp(Math.Abs(x)): 0);
            allSlots.Add(Math.Cos);
            allSlots.Add(x=>Math.Log(Math.Abs(x)+0.0000001));
        }



        public static void OptimizeOneTemplate(Template t, int start, int total)
        {
            int[] countByLevel = new int[slotCounts[t]];
            int totalScenarios = 1;
            for (int i = 0; i < slotCounts[t]; i++)
            {
                countByLevel[i] = totalScenarios;
                totalScenarios *= allSlots.Count;
            }

            double best = double.MaxValue;
            int bestIndex = 0;
            for (int i = 0; i < totalScenarios; i++)
            {
                
                List<Slot> inSlots = new List<Slot>();
                List<double> inOpt = new List<double>();

                int reminder = i;
                for (int k = 0; k < slotCounts[t]; k++)
                {
                    int order = slotCounts[t] - k - 1;
                    inSlots.Add(allSlots[reminder/ countByLevel[order]]);
                    reminder = reminder % countByLevel[order];
                }

                for (int k = 0; k < optCounts[t]; k++)
                {
                    inOpt.Add(1.0);
                }
                double currentBest = OptimizeSelectedTemplate(t, start, total, inSlots, inOpt);

                if (currentBest < best)
                {
                    best = currentBest;
                    bestIndex = i;
                }

                Trace.TraceInformation($"Scenario={i} Best={best/total} BestIndex={bestIndex}");
            }
        }

        private static double CalcTotalDiff(Template t, int start, int total, List<Slot> slots, List<double> opt)
        {
            double diff = 0;
            for (int i = start; i < start + total; i++)
            {
                double current = t(slots, opt, i);
                if (double.IsNaN(current))
                    throw new ApplicationException();
                diff += Math.Abs( current- zeros[i]);
                if(double.IsNaN(diff))
                    throw new ApplicationException();
            }

            if (double.IsNaN(diff))
                throw new ApplicationException();
            return diff;
        }

        public static double OptimizeSelectedTemplate(Template t, int start, int total, List<Slot> slots, List<double> opt)
        {
            double best = CalcTotalDiff(t, start, total, slots, opt);

            int n = 0;
            while (true )
            {
                n++;
                for (int i = 0; i < opt.Count; i++)
                {
                    double original = opt[i];
                    double originalDiff = CalcTotalDiff(t, start, total, slots, opt);
                    double delta = 100;

                    for (int k = 0; k < 6; k++)
                    {
                        delta = delta / 10;
                        double prev = CalcTotalDiff(t, start, total, slots, opt);
                        opt[i] += delta;
                        double after = CalcTotalDiff(t, start, total, slots, opt);

                        if (after > prev)
                            opt[i] -= delta;

                        prev = CalcTotalDiff(t, start, total, slots, opt);
                        opt[i] -= delta;
                        after = CalcTotalDiff(t, start, total, slots, opt);
                        if (after > prev)
                            opt[i] += delta;
                    }
                    double finish = CalcTotalDiff(t, start, total, slots, opt);
                    

                    //if (n % 500 == 0)
                    //    Trace.TraceInformation($"AvgDiff={finish / total} Round={n} Original={original} Last={opt[i]}");
                }

                double current = CalcTotalDiff(t, start, total, slots, opt);
                if (current > best - 0.01 / total)
                {
                    Trace.TraceInformation($"ExitOnNoImprovement AvgDiff={current / total} Round={n}");
                    break;
                }
                best = current;

            }

            //for (int i = 0; i < opt.Count; i++)
            //    Trace.TraceInformation($"opt[{i}]={opt[i]}");


            return CalcTotalDiff(t, start, total, slots, opt); 
        }

        

        public static void OptimizeSingleFunction(f f, int start,int total, int optNum, int round)
        {
            double best = CalcDiff(f, start, total);
            for (int n = 0; n < round; n++)
            {
                for (int i = 0; i < optNum; i++)
                {
                    double original = o[i];
                    double originalDiff = CalcDiff(f, start, total);
                    double delta = 100;

                    for (int k = 0; k < 6; k++)
                    {
                        delta = delta / 10;
                        double prev = CalcDiff(f, start, total);
                        o[i] += delta;
                        double after = CalcDiff(f, start, total);

                        if (after > prev)
                            o[i] -= delta;

                        prev = CalcDiff(f, start, total);
                        o[i] -= delta;
                        after = CalcDiff(f, start, total);
                        if (after > prev)
                            o[i] += delta;
                    }
                    double finish = CalcDiff(f, start, total);
                    if (n % 500 == 0)
                        Trace.TraceInformation($"Diff={finish / total} Round={n} Original={original} Last={o[i]}");
                }

                double current = CalcDiff(f, start, total);
                if (current > best - 0.01/total)
                {
                    Trace.TraceInformation($"Diff={current / total} Round={n}");
                    break;
                }
                best = current;
            }

            for (int i = 0; i < optNum; i++)
                Trace.TraceInformation($"O[{i}]={o[i]}");
        }

        public static double f4(int n)
        {
            return o[0] + o[1] * Math.Pow(n, o[2] / primes[n] + o[3]) + o[4] * Math.Pow(n, o[5] / (n+1) + o[6]);
        }

        public static double f3(int n)
        {
            return o[0] + o[1] * n*(1-o[3]/primes[n]) + o[2] * (primes[n + 1] - primes[n]);
        }

        public static double f2(int n)
        {
            return o[0]
                + o[1] * n
                //  + o[2] * primes[n]
                //  + o[3] * primes[n + 1]
                //  + o[4] * primes[n + 2]
                + o[2] *  (n + 1)/ primes[n];
          //      + o[3] * primes[n] * primes[n] / (n + 1) / (n + 1);
            //+o[4] * primes[n] * primes[n] * primes[n] / (n + 1) / (n + 1)/(n+1);

        }

        private static double CalcDiff(f f,int start, int total)
        {
            double diff = 0;
            for (int i = start; i < start+ total; i++)
                diff += Math.Abs(f(i) - zeros[i]);
            return diff;
        }

        public static void FindCoefficient(f f, int total)
        {
            List<double> x = new List<double>();
            List<double> y = new List<double>();
            List<double> d = new List<double>();
            double diff = 0;
            for (int i = 0; i < total; i++)
            {
                x.Add(f(i));
                y.Add(zeros[i]);
                d.Add(f(i) - zeros[i]);
                diff += Math.Abs(f(i) - zeros[i]);

            }

            double[] xVals = x.ToArray();
            double[] yVals = y.ToArray();
            double rSquared, intercept, slope;
            LinearRegression(xVals, yVals, out rSquared, out intercept, out slope);

            Trace.WriteLine(string.Format("rSquared={0} intercept={1} slope={2} diff={3}", rSquared, intercept, slope, diff));
        }

        public static double f1(int n)
        {
            // diff=1042929.3846148
            return n;
        }

        public static List<double> LoadZeros()
        {
            List<double> d = new List<double>();

            string[] lines = File.ReadAllLines(@"C:\Users\Zifeng\source\repos\ZifengHe\AutoSymbol\RiemannZeta\RiemannZeta\Data\zeros.txt");
            for (int i = 0; i < lines.Length; i++)
                d.Add(double.Parse(lines[i]));
            return d;
        }

        public static void LinearRegression(
            double[] xVals,
            double[] yVals,
            out double rSquared,
            out double yIntercept,
            out double slope)
        {
            if (xVals.Length != yVals.Length)
            {
                throw new Exception("Input values should be with the same length.");
            }

            double sumOfX = 0;
            double sumOfY = 0;
            double sumOfXSq = 0;
            double sumOfYSq = 0;
            double sumCodeviates = 0;

            for (var i = 0; i < xVals.Length; i++)
            {
                var x = xVals[i];
                var y = yVals[i];
                sumCodeviates += x * y;
                sumOfX += x;
                sumOfY += y;
                sumOfXSq += x * x;
                sumOfYSq += y * y;
            }

            var count = xVals.Length;
            var ssX = sumOfXSq - ((sumOfX * sumOfX) / count);
            var ssY = sumOfYSq - ((sumOfY * sumOfY) / count);

            var rNumerator = (count * sumCodeviates) - (sumOfX * sumOfY);
            var rDenom = (count * sumOfXSq - (sumOfX * sumOfX)) * (count * sumOfYSq - (sumOfY * sumOfY));
            var sCo = sumCodeviates - ((sumOfX * sumOfY) / count);

            var meanX = sumOfX / count;
            var meanY = sumOfY / count;
            var dblR = rNumerator / Math.Sqrt(rDenom);

            rSquared = dblR * dblR;
            yIntercept = meanY - ((sCo / ssX) * meanX);
            slope = sCo / ssX;
        }
    }
}
