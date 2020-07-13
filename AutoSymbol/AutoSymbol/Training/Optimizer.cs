using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoSymbol.Core;
using AutoSymbol.Category;

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
        public double CalcWeight(OpNode chain, int depth, out int childCount)
        {
            double localSum = 0;
            double childrenSum = 0;

            childCount = 0;
            for(int i=0; i < chain.Operands.Length;i++)
            {
                if(chain.Operands[i].FromChain!= null)
                {
                    int currentChild;
                    double result = CalcWeight(chain.Operands[i].FromChain, depth + 1, out currentChild);
                    childrenSum += result;
                    childCount += currentChild;
                }               
            }

            if (ForNGroup)
            {
                for (int i = 0; i < chain.Operands.Length; i++)
                {
                    if (chain.Operands[i].FromChain != null)
                    {
                        if (chain.Operator.Sig == N.NMul.Sig && chain.Operands[i].FromChain.Operator.Sig == N.NPlus.Sig)
                            localSum += ForNPlusBelowNMul / (depth + 1);
                    }
                }

                if (chain.Operator == N.NPlus)
                {
                    localSum += ForNPlus;
                    localSum += ForNPlusDepth * depth;
                    localSum += ForNPlusDepthDepth * depth * depth;
                    localSum += ForNPlusDepthChildCount * depth * childCount;
                }
            }

            double sum = localSum + childrenSum;
            chain.lastLocalWeight = localSum;
            chain.lastTotalWeight = sum;

            chain.AssertChildrenWeightConsistency();
            return sum;
        }

        public static WeightFunction PolynormialExpansion = new WeightFunction()
        {
            ForNGroup =true,
            ForNPlus = 0,
            ForNPlusBelowNMul =1.0,
            ForNPlusDepth = 0,
            ForNPlusDepthDepth =0,
            ForNPlusDepthChildCount =0
        };
    }

}
