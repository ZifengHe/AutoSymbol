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

            PopulateTopMembers();
        }

        public void PopulateTopMembers()
        {
            /// 1, 1+1, 1+1+1 etc
            /// 2,3,4 as alias
            /// Step 1. Operate means concatenation
            /// Step 2. Generate non-existent member
            /// Step 3. Enable searchable Name-Alias coorelation
            /// Step 4. Answer the benchmark question
            /// Test a change
            /// 

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
