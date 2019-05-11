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

            GMul = new Operator("×", this);
            this.OpStore[GMul.ShortName] = GMul;
            GId = new Operator("Id", this);
            this.OpStore[GId.ShortName] = GId;
            GInv = new Operator("Inv", this);
            this.OpStore[GInv.ShortName] = GInv;
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
        }
    }

    public class GroupBenchmark
    {
        public void ProofGroupIdUnique()
        {
            /// GId(h)=GId(k)
            /// Step 1  start from GId(h) as OpChain
            /// Step 2 
        }
    }
}
