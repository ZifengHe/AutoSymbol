using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol.GroupTheory
{
    public class G: Set
    {
        public Operator GMul;
        public Operator GId;
        public Operator GInv;
        public Operator GSelf;
        public G() : base("G")
        {
            /// How to represent closure for multiplication?
            /// If consider operator is the generator,
            /// No ER needed to satisfy this condition
            /// 

            /// How to represent existence like Id, Inverse?
            /// Id & Inverse are operator by themselves
            /// 
            /// Proof of Id operation unique?
            /// Id1(x)=Id2(x)
            /// 
            /// Proof of Inverse unique?
            /// Inv1(x)=Inv2(x)
            /// 
            /// Prove power relation Pow(x,n)*Pow(x,m) = Pow(x, m+n)?
            /// 
            /// 

            GMul = new Operator("×", this, false);
            this.OpStore[GMul.ShortName] = GMul;
            GId = new Operator("Id", this, true);
            this.OpStore[GId.ShortName] = GId;
            GInv = new Operator("Inv", this, true);
            this.OpStore[GInv.ShortName] = GInv;
            GSelf = new Operator("Self", this, true);
            this.OpStore[GSelf.ShortName] = GSelf;
        }

        public void HydrateER()
        {
            Member g = new Member("g", this.ShortName, true);
            Member h = new Member("h", this.ShortName, true);
            Member k = new Member("k", this.ShortName, true);

            ER er = new ER();
            er.Left = GMul.CreateOpChain(g, GMul.Operate(h, k));
            er.Right = GMul.CreateOpChain(GMul.Operate(g, h), k);
            this.ERStore["GMulAssoc"] = er;

            er = new ER();
            er.Left = GMul.CreateOpChain(g, GInv.Operate(g));
            er.Right = GMul.CreateOpChain(GInv.Operate(g), g);
            this.ERStore["GInvOne"] = er;

            er = new ER();
            er.Left = GMul.CreateOpChain(g, GInv.Operate(g));
            er.Right = GId.CreateOpChain(h);
            this.ERStore["GInvTwo"] = er;

            er = new ER();
            er.Left = GMul.CreateOpChain(g, GId.Operate(h));
            er.Right = GMul.CreateOpChain(g);
            this.ERStore["GIdLeft"] = er;

            er = new ER();
            er.Left = GMul.CreateOpChain(GId.Operate(h), g);
            er.Right = GMul.CreateOpChain(g);
            this.ERStore["GIdRight"] = er;

            er = new ER();
            er.Left = GSelf.CreateOpChain(GSelf.Operate(g));
            er.Right = GSelf.CreateOpChain(g);
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
        public void Construct(ER er, OpChain seed)
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

        public static List<OpChain> ExpandOne(OpChain incoming, params OpPair[] opPairs)
        {
            List<OpChain> list = new List<OpChain>();

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

        public static List<OpChain> ExpandAll(OpChain incoming)
        {
            List<OpChain> final = new List<OpChain>();
            Set target = Set.AllSets[incoming.Operator.ResultSetName];
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
