using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiemannZeta
{
    public class Operation
    {
        public delegate double Calc(OpNode node);
       

        public Calc MyCalc;
        public static List<Operation> All = new List<Operation>();

        public int Weight;

        public static void Init()
        {
            Operation feedN = new Operation();
            Operation feedPrime = new Operation();

            //Operation opt = new Operation();
            //opt.MyCalc = new Calc((OpNode node) =>{ return node.Self; });
            //opt.Weight

            Operation plus = new Operation();
            plus.MyCalc = new Calc((OpNode node) => { return node.Left + node.Right; });
            plus.Weight = 5;
            All.Add(plus);
        }

       
    }

   


    public class OpTree
    {
        public List<OpNode> AllRoot = new List<OpNode>();
        public static void BuildAllTree(int maxDepth)
        {
            /// Cannot be wild guess.  Need to be guided guess. And full coverage
            /// Question : What is the guided template
            /// 
            // Every Operation deservs one optimizer.
            // Figure out, how to put OptimizationParameter there.
            // how to generate random binary tree

        }
    }

    public class OpNode
    {
        public OpNode LeftNode;
        public OpNode RightNode;
        public double Left;
        public double Right;

        public double Self;

        public bool IsInputN;
        public bool IsInputPrime;
        public bool IsOptimizer;
        public int OptimizerIndex;


        public Operation Op;

        public void Calc()
        {

        }
    }
}
