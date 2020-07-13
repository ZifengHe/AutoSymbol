using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoSymbol.Core;
using AutoSymbol.ERGroup;

namespace AutoSymbol.Draft.GroupTheory
{
    public class G: SetBase
    {
        public Operator GMul;
        public Operator GId;
        public Operator GInv;
        public Operator GSelf;
        public G() : base("G")
        {
            GroupRule<G>.CreateAll(this);
        }       
    }

    public class AbG : SetBase  // Abelian Group
    {
        public AbG() : base("AbG")
        {
            GroupRule<AbG>.CreateAll(this);
            GroupRule<AbG>.CreateForAbelian(this);
        }
    }

    public class NcG : SetBase // Normal Child Group
    {
        public NcG() : base("NcG")
        {
            G G = new G();
            this.Parent = G;
            GroupRule<NcG>.CreateForNormalChild(this);
        }
    }

    public class GroupBenchmark
    {
        public void ProofGroupIdUnique()
        {
            /// GId(h)=GId(k)
            /// Step 1  start from GId(h) as OpChain
            /// Step 2 
            /// Question : need a process to multiply GInv(g)
            /// Answer 1 : Need a new way of transformation (not starting from the root of the ER
            /// Answer 2 : Use ER beyond replacement, use it for construction
        }
    }

    public class ERConstructor
    {
        public void Construct(ReplaceRule er, OpNode seed)
        {
            /// 1. Find variables in ER, one of them represent the seed
            /// 2. Others will need to pick from top branches of the seed (and corresponding supported operations)
            /// 3. This process will rely heavily on the beauty thing to prune as well
            /// 4. Each ER can have multiple construction, every construction will have a transformation set
        }
    }

    public class OpChainConstruct
    {
        public enum OperateTargetType
        {
            Main,
            FirstChild,
            LastChild
        }

        public class OpPair
        {
            public OperateTargetType TargetType;
            public Operator Operator;
        }

        public static List<OpNode> ExpandOne(OpNode incoming, params OpPair[] opPairs)
        {
            List<OpNode> list = new List<OpNode>();

            if (opPairs.Length == 2)
            {
                if(opPairs[0].TargetType== OperateTargetType.FirstChild 
                    && opPairs[1].TargetType == OperateTargetType.Main)
                {
                    list.Add(opPairs[1].Operator.CreateOpChain(opPairs[0].Operator.Operate(incoming.Operands[0],
                        incoming.CreateMember("Constructed", false))));
                }             
            }
            return list;
        }

        public static List<OpNode> ExpandAll(OpNode incoming)
        {
            List<OpNode> final = new List<OpNode>();
            SetBase target = SetBase.AllSets[incoming.Operator.ResultSetName];
            foreach(var op1 in target.OpStore.Values)
                foreach(var op2 in target.OpStore.Values)
                {
                    var r1 = ExpandOne(incoming,
                        new OpPair() { TargetType = OperateTargetType.FirstChild, Operator = op1 },
                        new OpPair() { TargetType = OperateTargetType.Main, Operator = op2 });

                    final.AddRange(r1);
                }
            return final;
        }
    }
}
