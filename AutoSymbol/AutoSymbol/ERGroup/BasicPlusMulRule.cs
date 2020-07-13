using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoSymbol.Core;

namespace AutoSymbol.ERGroup
{
    
    public class BasicPlusMulRule<T> where T : SetBase
    {
        
        public static void CreateAll(T t)
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
            t.RuleStore[C.PlusAssocOne] = rr;

            rr = new ReplaceRule();
            rr.Right = plus.CreateOpChain(a, plus.Operate(b, c));
            rr.Left = plus.CreateOpChain(plus.Operate(a, b), c);
            t.RuleStore[C.PlusAssocTwo] = rr;

            rr = new ReplaceRule();
            rr.Left = plus.CreateOpChain(a, b);
            rr.Right = plus.CreateOpChain(b, a);
            t.RuleStore[C.PlusCommute] = rr;

            rr = new ReplaceRule();
            rr.Left = mul.CreateOpChain(a, b);
            rr.Right = mul.CreateOpChain(b, a);
            t.RuleStore[C.MulCommute] = rr;

            rr = new ReplaceRule();
            rr.Left = mul.CreateOpChain(plus.Operate(a, b), c);
            rr.Right = plus.CreateOpChain(mul.Operate(a, c), mul.Operate(b, c));
            t.RuleStore[C.MulDistrOne] = rr;

            rr = new ReplaceRule();
            rr.Left = mul.CreateOpChain(a, plus.Operate(b, c));
            rr.Right = plus.CreateOpChain(mul.Operate(a, b), mul.Operate(a, c));
            t.RuleStore[C.MulDistrTwo] = rr;

            //rr = new ReplaceRule();
            //rr.Right = mul.CreateOpChain(plus.Operate(a, b), c);
            //rr.Left = plus.CreateOpChain(mul.Operate(a, c), mul.Operate(b, c));
            //t.RuleStore[C.MulDistrTwo] = rr;


            rr = new ReplaceRule();
            rr.Left = mul.CreateOpChain(a, mul.Operate(b, c));
            rr.Right = mul.CreateOpChain(mul.Operate(a, b), c);
            t.RuleStore[C.MulAssocOne] = rr;

            rr = new ReplaceRule();
            rr.Right = mul.CreateOpChain(a, mul.Operate(b, c));
            rr.Left = mul.CreateOpChain(mul.Operate(a, b), c);
            t.RuleStore[C.MulAssocTwo] = rr;
        }


    }
}
