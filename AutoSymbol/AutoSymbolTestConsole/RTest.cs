using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoSymbol.Core;
using AutoSymbol;
using AutoSymbol.Category;
using AutoSymbol.ERGroup;
using System.Diagnostics;

namespace AutoSymbolTestConsole
{
    class RTest : TestBase
    {
        public void ProveAPlusBSquare()
        {
            R r = new R();
            Member x = new Member("X", r.ShortName, true);
            r.MemStore.Add(x);
            Member y = new Member("Y", r.ShortName, true);
            r.MemStore.Add(y);
            // OpChain target = r.OpStore[]
            OpNode target=  r.OpStore["×"].CreateOpChain(
                r.OpStore["+"].Operate(x, y),
                r.OpStore["+"].Operate(x, y));

            RuleSeries rs = new RuleSeries();
            rs.AddOneRule(GlobalRules.RuleStore[C.MulDistrOne]);
            rs.AddOneRule(GlobalRules.RuleStore[C.MulDistrTwo]);
            //rs.AddOneRule(G.RuleStore[C.MulDistrTwo]);
            rs.AddOneRule(GlobalRules.RuleStore[C.MulCommute]);


            List<OpNode> result = rs.ApplyRules(target);

            Log(result);

            Assert.IsTrue(result.Last().Sig == "R[((X×X)+(((X×Y)×2)+(Y×Y)))]");



            //TransformRecord.AddTransformWithNoSource(target.Sig);
            //OpByStr dict = ReplaceRule.BuildERChainsForLevel(target, r.RRStore.Values.ToList(), 10, 100);

            //UIData.AllItems = TransformRecord.AllRecordBySig.Keys.ToList();

            //Assert.IsTrue(dict.ContainsKey("R[((X×X)+(((X×Y)×2)+(Y×Y)))]"));
        }
    }
}
