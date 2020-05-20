using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoSymbol.Core;

namespace AutoSymbol.ERGroup
{
    public class BasicPlusMulRule<T> : RuleSet<T> where T : SetBase
    {

        public override void CreateAll(T t)
        {
            Operator plus = t.CreateOperatorIfNotExist("+", false);
            Operator mul = t.CreateOperatorIfNotExist("×", false);
            Member a = new Member("a", t.ShortName, true);
            Member b = new Member("b", t.ShortName, true);
            Member c = new Member("c", t.ShortName, true);
            Member n = new Member("n", t.ShortName, true);

            ReplaceRule rr = new ReplaceRule();
            rr.Left = plus.CreateOpChain(a, plus.Operate(b, c));
            rr.Right = plus.CreateOpChain(plus.Operate(a, b), c);
            t.RRStore["PlusAssoc"] = rr;

            rr = new ReplaceRule();
            rr.Left = plus.CreateOpChain(a, b);
            rr.Right = plus.CreateOpChain(b, a);
            t.RRStore["PlusCommute"] = rr;

            rr = new ReplaceRule();
            rr.Left = mul.CreateOpChain(a, b);
            rr.Right = mul.CreateOpChain(b, a);
            t.RRStore["MulCommute"] = rr;

            rr = new ReplaceRule();
            rr.Left = mul.CreateOpChain(plus.Operate(a, b), c);
            rr.Right = plus.CreateOpChain(mul.Operate(a, c), mul.Operate(b, c));
            t.RRStore["MulDistr"] = rr;

            rr = new ReplaceRule();
            rr.Left = mul.CreateOpChain(a, mul.Operate(b, c));
            rr.Right = mul.CreateOpChain(mul.Operate(a, b), c);
            t.RRStore["MulAssoc"] = rr;
        }


    }
}
