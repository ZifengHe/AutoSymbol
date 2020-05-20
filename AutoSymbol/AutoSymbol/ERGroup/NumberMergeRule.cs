using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoSymbol.Core;

namespace AutoSymbol.ERGroup
{
    public class NumberMergeRule<T> : RuleSet<T> where T : NumberSet
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
            rr.Left = plus.CreateOpChain(a, a);
            rr.Right = mul.CreateOpChain(a, t.ConstPlus(t.One, t.One));
            t.RRStore["OnePlusOne"] = rr;

            rr = new ReplaceRule();
            rr.Left = plus.CreateOpChain(a, mul.Operate(n, a));
            rr.Right = mul.CreateOpChain(plus.Operate(n, t.One), a);
            t.RRStore["AnyPlusOne"] = rr;


            /// Below to review ER replacement logic completely.
            //rr = new ReplaceRule();
            //rr.Left = plus.CreateOpChain(a, mul.Operate(n, a));
            //rr.Right = mul.CreateOpChain(t.ConstPlus(n, t.One), a);
            //t.RRStore["ConstPlusConst"] = rr;
        }
    }
}
