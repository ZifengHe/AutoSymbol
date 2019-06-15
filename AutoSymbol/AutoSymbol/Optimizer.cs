using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol
{
    public enum OptimizerType
    {
        Invalid,
        DoPolynormialExpansion
    }
    public class Optimizer
    {
        private static Dictionary<OptimizerType, Optimizer> AllOptimizer = new Dictionary<OptimizerType, Optimizer>();
        Dictionary<int, WeightFunction> AllWF = new Dictionary<int, WeightFunction>();
        WeightFunction DefaultWF = new WeightFunction();

        public static Optimizer GetOptimizer(OptimizerType type)
        {
            if(AllOptimizer.Count ==0)
            {
                Optimizer optimizer;

                optimizer = new Optimizer();
                optimizer.AddWeightfunctionRange(WeightFunction.PolynormialExpansion, 0, 10);
                AllOptimizer[OptimizerType.DoPolynormialExpansion] = optimizer;
            }
            return AllOptimizer[type];
        }
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
        public bool ForNGroup;
        public double ForNPlus;
        public double ForNPlusBelowNMul;
        public double ForNPlusDepth;
        public double ForNPlusDepthDepth;
        public double ForNPlusDepthChildCount;
        public double CalcWeight(OpChain chain, int depth, out int childCount)
        {
            double sum = 0;

            childCount = 0;
            for(int i=0; i < chain.Operands.Length;i++)
            {
                if(chain.Operands[i].FromChain!= null)
                {
                    int currentChild = 0;
                    sum += CalcWeight(chain.Operands[i].FromChain, depth + 1, out currentChild);
                    childCount += currentChild;

                    if (chain.Operator.Sig == N.NMul.Sig && chain.Operands[i].FromChain.Operator.Sig == N.NPlus.Sig)
                        sum += ForNPlusBelowNMul / (depth+1);
                }               
            }

            if (ForNGroup)
            {
                if(chain.Operator == N.NPlus)
                {
                    sum += ForNPlus;
                    sum += ForNPlusDepth * depth;
                    sum += ForNPlusDepthDepth * depth * depth;
                    sum += ForNPlusDepthChildCount * depth * childCount;
                }
            }

            chain.lastWeight = sum;
            return sum;
        }

        public static WeightFunction PolynormialExpansion = new WeightFunction()
        {
            ForNGroup =true,
            ForNPlus = 0,
            ForNPlusBelowNMul =1,
            ForNPlusDepth = 0,
            ForNPlusDepthDepth =0,
            ForNPlusDepthChildCount =0
        };
    }

}
