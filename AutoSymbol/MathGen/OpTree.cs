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
            /// Step 1. Define generations
            /// Step 2. Each Generation, expand end node
            /// Step 3. Each Generation, reduce duplicate
        }

        public static void CreateFirst()
        {
            OpTree ot = new OpTree();
            ot.Root = new OpNode();
            AllTemplates.Clear();
            AllTemplates.Add(ot);
        }

        public string CreateHash()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("o");
            Root.CalcHash(sb);
            return sb.ToString();
        }
    }

    public class OpNode
    {
        public OpNode Left;
        public OpNode Right;
        public IOperator Operator;

        public void CalcHash(StringBuilder sb)
        {
            if(this.Left!=null)
            {
                sb.Append("L");
                this.Left.CalcHash(sb);
            }
            if(this.Right!=null)
            {
                sb.Append("R");
                this.Right.CalcHash(sb);
            }
        }

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
