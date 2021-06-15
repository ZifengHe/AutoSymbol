using System;
using System.Collections.Generic;
using System.Text;

namespace MathGen
{
    public class OpTree
    {
        public OpNode Root;
        public static List<OpTree> AllTemplates;
        public static void CreateTemplates()
        {
        }

        public static void CreateFirst()
        {
            OpTree ot = new OpTree();
            ot.Root = new OpNode();
            AllTemplates.Clear();
            AllTemplates.Add(ot);
        }
    }

    public class OpNode
    {
        public OpNode Left;
        public OpNode Right;
        public IOperator Operator;

        public OpNode Clone()
        {
            OpNode ret = new OpNode();
            if (this.Left != null)
                ret.Left = this.Left.Clone();
            if (this.Right != null)
                ret.Right = this.Right.Clone();
            if(this.Operator!= null)
            {
                ret.Operator = this.Operator;
            }

            return ret;
        }
    }
}
