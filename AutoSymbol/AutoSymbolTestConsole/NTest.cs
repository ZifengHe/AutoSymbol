using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoSymbol.Core;
using AutoSymbol;
using AutoSymbol.Category;

namespace AutoSymbolTestConsole
{
    public class NTest
    {
        public void ProveTwoPlusXPlusThree()
        {
            N n = new N();
            Member x = new Member("X", n.ShortName, true);
            n.MemStore.Add(x);
            OpNode target = N.NPlus.CreateOpChain(n.MemStore["2"], N.NPlus.Operate(x, n.MemStore["3"]));

            RuleSeries rs = new RuleSeries();
            //TransformRecord.AddTransformWithNoSource(target.Sig);
            //OpByStr dict = ReplaceRule.BuildERChainsForLevel(target, n.RRStore.Values.ToList(), 7, 20);

            //UIData.AllItems = TransformRecord.AllRecordBySig.Keys.ToList();

            //Assert.IsTrue(dict.ContainsKey("N[(5+X)]"));
        }
    }
}
