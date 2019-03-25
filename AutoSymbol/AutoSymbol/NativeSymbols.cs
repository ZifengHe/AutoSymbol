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
            er.Left = NPlus.CreateOpChain(new Member[] { a, NPlus.Operate(new Member[] { b, c }) });
            er.Right = NPlus.CreateOpChain(new Member[] { NPlus.Operate(new Member[] { a, b }), c });
            this.ERStore["NPlusAssoc"] = er;

            er = new ER();
            er.Left = NPlus.CreateOpChain(new Member[] { a, b });
            er.Right = NPlus.CreateOpChain(new Member[] { b, a });
            this.ERStore["NPlusCommute"] = er;

            er = new ER();
            er.Left = NMul.CreateOpChain(new Member[] { a, b });
            er.Right = NMul.CreateOpChain(new Member[] { b, a });
            this.ERStore["NMulCommute"] = er;

            er = new ER();
            er.Left = NMul.CreateOpChain(new Member[] { NPlus.Operate(new Member[] { a, b }), c});
            er.Right = NPlus.CreateOpChain(new Member[] { NMul.Operate(new Member[] { a, c }), NMul.Operate(new Member[] { b, c }) });
            this.ERStore["NMulDistr"] = er;

            er = new ER();
            er.Left = NMul.CreateOpChain(new Member[] { a, NMul.Operate(new Member[] { b, c }) });
            er.Right = NMul.CreateOpChain(new Member[] { NMul.Operate(new Member[] { a, b }), c });
            this.ERStore["NMulAssoc"] = er;

            er = new ER();
            er.Left = NPlus.CreateOpChain(new Member[] { a, NMul.Operate(new Member[] { n, a }) });
            er.Right = NMul.CreateOpChain(new Member[] { NPlus.Operate(new Member[] { n, this.MemStore["1"] }), a });
            this.ERStore["Coefficience"] = er;
        }

        public void PopulateTopMembers()
        {           
            Member one = this.MemStore["1"];
            Member lastOne = one;
            for(int i=2; i< 7;i++)
            {
                OpChain current = this.OpStore["+"].CreateOpChain(new Member [] { lastOne,one});
                lastOne = current.CreateMember(i.ToString());
                this.MemStore.Add(lastOne);
                Dictionary<string, OpChain> dict = this.ERStore["NPlusAssoc"].BuildCompleteERChains(lastOne.FromChain);
                foreach (var item in dict)
                    SigToShortName[item.Key] = i.ToString();
            }
        }
    }
}
