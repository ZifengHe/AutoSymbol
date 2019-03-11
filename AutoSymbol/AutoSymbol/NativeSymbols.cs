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
        }

    }

    public class NPlus : OperatorSymbol
    {
        public NPlus() : base("+")
        { }

        public override Symbol Operate(Symbol[] symbols)
        {
            throw new NotImplementedException();
        }
    }

}
