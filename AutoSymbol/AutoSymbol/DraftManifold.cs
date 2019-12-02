using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol.Draft.Manifold
{
    public class OneOneOp : Operator
    {
        public OneOneOp(string name, Set targetSet) :
            base(name, targetSet, true)
        { }
       
    }

    public class OperatorSet
    { }

    public class OpOnM : Set
    {
        public OpOnM() : base("OpOnM")
        { }
    }
    public class M:Set
    {
        public Operator MToRn;
        public M() : base("M")
        { }

        /// Map to Rn should be Operator
    }

    public class Rn : Set
    {
        public Rn() : base("Rn")
        { }

    }
}
