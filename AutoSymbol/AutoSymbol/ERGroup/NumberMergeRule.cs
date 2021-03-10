using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoSymbol.Core;

namespace AutoSymbol.ERGroup
{
    public class NumberMergeRule<T>  where T : NumberSet
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
            rr.Left = plus.CreateOpChain(a, a);
            rr.Right = mul.CreateOpChain(a, plus.Operate(t.One, t.One));
            GlobalRules.RuleStore[C.OnePlusOne] = rr;            

            rr = new ReplaceRule();
            rr.Left = plus.CreateOpChain(a, mul.Operate(n, a));
            rr.Right = mul.CreateOpChain(plus.Operate(n, t.One), a);
            GlobalRules.RuleStore[C.AnyConstPlusOne] = rr;

            rr = new ReplaceRule();
            rr.Left = plus.CreateOpChain(a, mul.Operate(n, a));
            rr.Right = mul.CreateOpChain(plus.Operate(n, t.One), a);
            GlobalRules.RuleStore[C.AnyPlusOne] = rr;

            MergeRule mr = new MergeRule();
            mr.NodeProc = t.ConstPlus; //NumberSet.ConstPlus;
            mr.Op = plus;
            GlobalRules.RuleStore[C.MergeConstPlus] = mr;

            mr = new MergeRule();
            mr.NodeProc = t.ConstMultiply;
            mr.Op = mul;
            GlobalRules.RuleStore[C.MergeConstMultiply] = mr;
            /// How to pass the appropriate method here without the cost?



            /// Below to review ER replacement logic completely.
            //rr = new ReplaceRule();
            //rr.Left = plus.CreateOpChain(a, mul.Operate(n, a));
            //rr.Right = mul.CreateOpChain(t.ConstPlus(n, t.One), a);
            //t.RRStore["ConstPlusConst"] = rr;


            /// 1. OpChain add merge step, it is defintely not rule
            /// 2. RecursiveShorten  ---- Remove the dictionary based solution
            /// 3. OpChain ->Operator -> Set -> SpecialLogic (ConstMembers)
            /// 4. Future special merger? 
            /// 4. set Set->DoPostReplace (base. DoPostReplace)
        }
    }
}
