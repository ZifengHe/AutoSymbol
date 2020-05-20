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
    class RealTest
    {
        public void ProveAPlusBSquare()
        {
            R r = new R();
            Member x = new Member("X", r.ShortName, true);
            r.MemStore.Add(x);
            Member y = new Member("Y", r.ShortName, true);
            r.MemStore.Add(y);
            // OpChain target = r.OpStore[]
            OpChain target=  r.OpStore["×"].CreateOpChain(
                r.OpStore["+"].Operate(x, y),
                r.OpStore["+"].Operate(x, y));

            OneTransform.AddTransformWithNoSource(target.Sig);
            StrToOp dict = ReplaceRule.BuildERChainsForLevel(target, r.RRStore.Values.ToList(), 10, 100);

            UIData.AllItems = OneTransform.AllResult.Keys.ToList();

            Assert.IsTrue(dict.ContainsKey("R[((X×X)+(((X×Y)×2)+(Y×Y)))]"));
        }
    }
}
