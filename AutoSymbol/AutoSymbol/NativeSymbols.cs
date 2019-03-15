using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol
{
    public class N : Set
    {
        public N() : base("N")
        {
            Member one = new Member("1", this);
            this.MemStore.Add(one);
            Operator NPlus = new Operator("+", this);
            this.OpStore[NPlus.ShortName] = NPlus;

            HydrateER();
            PopulateTopMembers();
        }

        public void HydrateER()
        {
            /// 1. Because of ER, member can have equivalent OpChain
            /// 2. Transforming branch, by comparing the chain signature first
            /// 3. Serialize, Replace, Deserialize as the transformation rule.

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
