using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol
{
    public class N : Set
    {
        public Operator NPlus;
        public Operator NMul;

        public N() : base("N")
        {
            Member one = new Member("1", this.ShortName);
            this.MemStore.Add(one);
            NPlus = new Operator("+", this);
            this.OpStore[NPlus.ShortName] = NPlus;
            NMul = new Operator("×", this);
            this.OpStore[NMul.ShortName] = NMul;

            HydrateER();
            PopulateTopMembers();
        }

        public void HydrateER()
        {
            Member a = new Member("a", this.ShortName);
            Member b = new Member("b", this.ShortName);
            Member c = new Member("c", this.ShortName);
            Member n = new Member("n", this.ShortName);

            ER er = new ER();
            er.Left = NPlus.CreateOpChain(a, NPlus.Operate(b, c));
            er.Right = NPlus.CreateOpChain(NPlus.Operate(a, b), c);
            this.ERStore["NPlusAssoc"] = er;

            er = new ER();
            er.Left = NPlus.CreateOpChain(a, b);
            er.Right = NPlus.CreateOpChain(b, a);
            this.ERStore["NPlusCommute"] = er;

            er = new ER();
            er.Left = NMul.CreateOpChain(a, b);
            er.Right = NMul.CreateOpChain(b, a);
            this.ERStore["NMulCommute"] = er;

            er = new ER();
            er.Left = NMul.CreateOpChain(NPlus.Operate(a, b), c);
            er.Right = NPlus.CreateOpChain(NMul.Operate(a, c), NMul.Operate(b, c));
            this.ERStore["NMulDistr"] = er;

            er = new ER();
            er.Left = NMul.CreateOpChain(a, NMul.Operate(b, c));
            er.Right = NMul.CreateOpChain(NMul.Operate(a, b), c);
            this.ERStore["NMulAssoc"] = er;

            er = new ER();
            er.Left = NPlus.CreateOpChain(a, NMul.Operate(n, a));
            er.Right = NMul.CreateOpChain(NPlus.Operate(n, this.MemStore["1"]), a);
            this.ERStore["Coefficience"] = er;
        }

        public void PopulateTopMembers()
        {
            Member one = this.MemStore["1"];
            Member lastOne = one;
            for (int i = 2; i < 7; i++)
            {
                OpChain current = this.OpStore["+"].CreateOpChain(lastOne, one);
                lastOne = current.CreateMember(i.ToString());
                this.MemStore.Add(lastOne);

                Member shortOne = lastOne.Copy<Member>();
                shortOne.FromChain = null;
                this.ShortMemStore.Add(shortOne);

                StrToOp dict = this.ERStore["NPlusAssoc"].BuildCompleteERChains(lastOne.FromChain);
                foreach (var item in dict)
                {
                    SigToShortName[item.Key] = i.ToString();
                    KnownOps[item.Key] = item.Value;
                }
            }
        }
    }
}
