using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoSymbol.Core;

namespace AutoSymbol.Draft.Manifold
{
    public class OneOneOp : Operator
    {
        public OneOneOp(string name, SetBase targetSet) :
            base(name, targetSet, true)
        { }
       
    }

    public class OperatorSet
    { }

    public class OpOnM : SetBase
    {
        public OpOnM() : base("OpOnM")
        { }
    }
    public class M:SetBase
    {
        public Operator MToRn;
        public M() : base("M")
        { }

        /// Map to Rn should be Operator
    }

    public class Rn : SetBase
    {
        public Rn() : base("Rn")
        { }

    }
}
