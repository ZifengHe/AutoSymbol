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
        public N() : base("N")
        {
            Member one = new Member("1", this.ShortName);
            this.MemStore.Add(one);
            NPlus = new Operator("+", this);
            this.OpStore[NPlus.ShortName] = NPlus;

            HydrateER();
            PopulateTopMembers();
        }

        public void HydrateER()
        {
            /// 1. Because of ER, member can have equivalent OpChain
            /// 2. Transforming branch, by comparing the chain signature first
            /// 3. Serialize, Replace, Deserialize as the transformation rule.
            Member a = new Member("a", this.ShortName);
            Member b = new Member("b", this.ShortName);
            Member c = new Member("c", this.ShortName);
            ER er = new ER();
            er.Left = NPlus.Operate(new Member[] { a, NPlus.Operate2(new Member[] { b, c }) });
            er.Right = NPlus.Operate(new Member[] { NPlus.Operate2(new Member[] { a, b }), c });
            this.ERStore["NPlusAssoc"] = er;            
        }

        public void PopulateTopMembers()
        {           
            Member one = this.MemStore["1"];
            Member lastOne = one;
            for(int i=2; i< 10;i++)
            {
                OpChain current = this.OpStore["+"].Operate(new Member [] { lastOne,one});
                lastOne = current.CreateMember(i.ToString());
                this.MemStore.Add(lastOne);            
            }
        }

    }
}
