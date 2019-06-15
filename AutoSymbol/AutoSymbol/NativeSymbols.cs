using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol
{
    public class N : Set
    {
        public static Operator NPlus;
        public static Operator NMul;
        public Member One;

        public N() : base("N")
        {
            One = new Member("1", this.ShortName, false);
            this.MemStore.Add(One);
            NPlus = new Operator("+", this, false);
            this.OpStore[NPlus.ShortName] = NPlus;
            NMul = new Operator("×", this, false);
            this.OpStore[NMul.ShortName] = NMul;

            HydrateER();
            PopulateSeedMember();
        }

        public void HydrateER()
        {
            Member a = new Member("a", this.ShortName, true);
            Member b = new Member("b", this.ShortName, true);
            Member c = new Member("c", this.ShortName, true);
            Member n = new Member("n", this.ShortName, true);

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
            er.Right = NMul.CreateOpChain(NPlus.Operate(n, One), a);
            this.ERStore["NAnyPlusOne"] = er;

            er = new ER();
            er.Left = NPlus.CreateOpChain(a, a);
            er.Right = NMul.CreateOpChain(a, NPlus.Operate(One, One));
            this.ERStore["NOnePlusOne"] = er;
        }

        public void PopulateSeedMember()
        {            
            Member lastOne = One;
            for (int i = 2; i < 7; i++)
            {
                OpChain current = this.OpStore["+"].CreateOpChain(lastOne, One);
                lastOne = current.CreateMember(i.ToString(), false);
                this.MemStore.Add(lastOne);

                Member shortOne = lastOne.Copy<Member>();
                shortOne.FromChain = null;
                this.ShortMemStore.Add(shortOne.ShortName, shortOne);

                StrToOp dict = this.ERStore["NPlusAssoc"].BuildCompleteERChains(lastOne.FromChain);
                foreach (var item in dict)
                {
                    SigToShortName[item.Key] = i.ToString();
                }
            }
        }
    }
}
