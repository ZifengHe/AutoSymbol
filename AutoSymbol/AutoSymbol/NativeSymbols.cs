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
