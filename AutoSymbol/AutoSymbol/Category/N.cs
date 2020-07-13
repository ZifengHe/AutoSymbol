using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoSymbol.Core;

namespace AutoSymbol.Core
{
    public class N : SetBase
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

            ReplaceRule rr = new ReplaceRule();
            rr.Left = NPlus.CreateOpChain(a, NPlus.Operate(b, c));
            rr.Right = NPlus.CreateOpChain(NPlus.Operate(a, b), c);
            this.RuleStore["NPlusAssoc"] = rr;

            rr = new ReplaceRule();
            rr.Left = NPlus.CreateOpChain(a, b);
            rr.Right = NPlus.CreateOpChain(b, a);
            this.RuleStore["NPlusCommute"] = rr;

            rr = new ReplaceRule();
            rr.Left = NMul.CreateOpChain(a, b);
            rr.Right = NMul.CreateOpChain(b, a);
            this.RuleStore["NMulCommute"] = rr;

            rr = new ReplaceRule();
            rr.Left = NMul.CreateOpChain(NPlus.Operate(a, b), c);
            rr.Right = NPlus.CreateOpChain(NMul.Operate(a, c), NMul.Operate(b, c));
            this.RuleStore["NMulDistr"] = rr;

            rr = new ReplaceRule();
            rr.Left = NMul.CreateOpChain(a, NMul.Operate(b, c));
            rr.Right = NMul.CreateOpChain(NMul.Operate(a, b), c);
            this.RuleStore["NMulAssoc"] = rr;

            rr = new ReplaceRule();
            rr.Left = NPlus.CreateOpChain(a, NMul.Operate(n, a));
            rr.Right = NMul.CreateOpChain(NPlus.Operate(n, One), a);
            this.RuleStore["NAnyPlusOne"] = rr;

            rr = new ReplaceRule();
            rr.Left = NPlus.CreateOpChain(a, a);
            rr.Right = NMul.CreateOpChain(a, NPlus.Operate(One, One));
            this.RuleStore["NOnePlusOne"] = rr;
        }

        public void PopulateSeedMember()
        {            
            Member lastOne = One;
            for (int i = 2; i < 7; i++)
            {
                OpNode current = this.OpStore["+"].CreateOpChain(lastOne, One);
                lastOne = current.CreateMember(i.ToString(), false);
                this.MemStore.Add(lastOne);

                Member shortOne = lastOne.Copy<Member>();
                shortOne.FromChain = null;
                this.ShortMemStore.Add(shortOne.ShortName, shortOne);

                ReplaceRule rr = (ReplaceRule)this.RuleStore["NPlusAssoc"];

                OpByStr dict = rr.BuildCompleteERChains(lastOne.FromChain);
                foreach (var item in dict)
                {
                    SigToShortName[item.Key] = i.ToString();
                }
            }
        }
    }
}
