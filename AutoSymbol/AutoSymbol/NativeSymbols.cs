using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol
{
    public class N : Symbol
    {
        public N () : base("N")
        {
            this.InsertMemberByKey("1");
            Operator NPlus = new Operator("+", this);
            this.Operators[NPlus.Name] = NPlus;

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
            for(int i=1; i< 5;i++)
            {
                OpChain two = this.Operators["+"].Operate(new Symbol [] { this.FindMemberByKey("1"), this.FindMemberByKey("1")});
            }
        }

    }
}
