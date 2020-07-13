using AutoSymbol.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol.ERGroup
{
    static class GroupRule<T>  where T : SetBase
    {
        public static void CreateForNormalChild(T t)
        {
            Operator GMul = t.Parent.CreateOperatorIfNotExist("×", false);
            Operator GInv = t.Parent.CreateOperatorIfNotExist("Inv", false);

            Member h = new Member("h", t.ShortName, true);
            Member g = new Member("g", t.Parent.ShortName, true);

            ReplaceRule er = new ReplaceRule();
            er.Left = GMul.CreateOpChain(g, GMul.Operate(h,GInv.Operate(g)));
            er.Right = new OpNode(h);
            t.RuleStore["GNormalChildDef"] = er;
        }

        public static void CreateForAbelian(T t)
        {
            Operator GMul = t.CreateOperatorIfNotExist("×", false);
            
            Member h = new Member("h", t.ShortName, true);
            Member k = new Member("k", t.ShortName, true);

            ReplaceRule er = new ReplaceRule();
            er.Left = GMul.CreateOpChain(h,k);
            er.Right = GMul.CreateOpChain(k,h);
            t.RuleStore["GMulCommute"] = er;
        }


        public static void CreateAll(T t)
        {            
            Operator GMul = t.CreateOperatorIfNotExist("×", false);
            Operator GId = t.CreateOperatorIfNotExist("Id", false);
            Operator GInv = t.CreateOperatorIfNotExist("Inv", false);
            Operator GSelf = t.CreateOperatorIfNotExist("Self", false);

            Member g = new Member("g", t.ShortName, true);
            Member h = new Member("h", t.ShortName, true);
            Member k = new Member("k", t.ShortName, true);

            ReplaceRule er = new ReplaceRule();
            er.Left = GMul.CreateOpChain(g, GMul.Operate(h, k));
            er.Right = GMul.CreateOpChain(GMul.Operate(g, h), k);
            t.RuleStore["GMulAssoc"] = er;

            er = new ReplaceRule();
            er.Left = GMul.CreateOpChain(g, GInv.Operate(g));
            er.Right = GMul.CreateOpChain(GInv.Operate(g), g);
            t.RuleStore["GInvOne"] = er;

            er = new ReplaceRule();
            er.Left = GMul.CreateOpChain(g, GInv.Operate(g));
            er.Right = GId.CreateOpChain(h);
            t.RuleStore["GInvTwo"] = er;

            er = new ReplaceRule();
            er.Left = GMul.CreateOpChain(g, GId.Operate(h));
            er.Right = GMul.CreateOpChain(g);
            t.RuleStore["GIdLeft"] = er;

            er = new ReplaceRule();
            er.Left = GMul.CreateOpChain(GId.Operate(h), g);
            er.Right = GMul.CreateOpChain(g);
            t.RuleStore["GIdRight"] = er;

            er = new ReplaceRule();
            er.Left = GSelf.CreateOpChain(GSelf.Operate(g));
            er.Right = GSelf.CreateOpChain(g);
        }
    }
}
