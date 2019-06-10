using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol
{
    public class Optimizer
    {
        Dictionary<int, WeightFunction> AllWF = new Dictionary<int, WeightFunction>();
        WeightFunction DefaultWF;
        public void AddWeightfunctionRange(WeightFunction wf, int start, int end)
        {
            for (int i = start; i <= end; i++)
                AllWF[i] = wf;

        }
        public WeightFunction GetWeightFunction(int step)
        {
            if (AllWF.ContainsKey(step) == false)
                return DefaultWF;
            return AllWF[step];
        }
    }

    public class WeightFunction
    {
        public double ForNPlus;
        public double ForNPlusDepth;
        public double ForNPlusDepthDepth;
        public double ForNPlusDepthChildCount;
        public double CalcWeight(OpChain chain, int depth, int childCount)
        {
            return 0;
        }
    }

}
