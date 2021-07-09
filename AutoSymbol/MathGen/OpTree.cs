using System;
using System.Collections.Generic;
using System.Text;

namespace MathGen
{
    public class OpTree
    {
        public OpNode Root;
        public int GenNum;

        private List<OpNode> allEndNode=  new List<OpNode>();

        public static List<OpTree> AllTemplates = new List<OpTree>();
        public static void CreateTemplates(int totalGen)
        {
            /// Step 1. Define generations
            /// Step 2. Each Generation, expand end node
            /// Step 3. Each Generation, reduce duplicate

            OpTree.CreateGenZero();
            for(int i=1; i <totalGen; i++)
            {
                List<OpTree> candidates = new List<OpTree>();
                foreach(var one in AllTemplates)
                {
                    if(one.GenNum == (i-1))
                    {
                        candidates.AddRange(one.ExpandAllEndNodeByOne());
                    }
                }
                Dictionary<string, OpTree> dict = new Dictionary<string, OpTree>();
                foreach(var one in candidates)
                {
                    string key = one.CreateHash();
                    dict[key] = one;
                }
                AllTemplates.AddRange(dict.Values);
            }
        }

        public static void CreateGenZero()
        {
            OpTree ot = new OpTree();
            ot.Root = new OpNode();
            ot.GenNum = 0;
            AllTemplates.Clear();
            AllTemplates.Add(ot);
        }

        public void AssignVisualId()
        {
            OpNode.globalVisualId = 0;
            OpNode.RecursiveAssignVisualId(this.Root);
            this.Root.VisualId = 0;
        }
        public List<OpTree> ExpandAllEndNodeByOne()
        {
            List<OpTree> list = new List<OpTree>();
            allEndNode.Clear();
            PopulateEndNode(this.Root);

            foreach(var one in allEndNode)
            {
                OpTree temp = new OpTree();
                one.Left = new OpNode();
                one.Right = new OpNode();
                temp.Root = this.Root.Clone();
                temp.GenNum = this.GenNum + 1;
                list.Add(temp);
                one.Left = null;
                one.Right = null;
            }

            return list;
        }
        public string CreateHash()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("o");
            Root.CalcHash(sb);
            return sb.ToString();
        }

        private void PopulateEndNode(OpNode node)
        {
            if(node.Left== null || node.Right== null)
            {
                allEndNode.Add(node);
            }
            else
            {
                PopulateEndNode(node.Left);
                PopulateEndNode(node.Right);
            }
        }
    }

    public class OpNode
    {
        public OpNode Left;
        public OpNode Right;
        public IOperator Operator;
        
        public int VisualId;
        public  static int globalVisualId = 0;

       
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

        public static void RecursiveAssignVisualId(OpNode node)
        {
            
            if(node.Left != null)
            {
                globalVisualId++;
                node.Left.VisualId = globalVisualId;
                RecursiveAssignVisualId(node.Left);
            }
            if(node.Right != null)
            {
                globalVisualId++;
                node.Right.VisualId = globalVisualId;
                RecursiveAssignVisualId(node.Right);
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
