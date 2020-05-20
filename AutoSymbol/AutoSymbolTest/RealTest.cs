using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoSymbol.Core;
using AutoSymbol;
using AutoSymbol.Category;

namespace AutoSymbolTest
{
    [TestClass]
    public class RealTest
    {
        [TestMethod]
        public void ProveAPlusBSquare()
        {
            R r = new R();
            Member x = new Member("X", r.ShortName, true);
            r.MemStore.Add(x);
            Member y = new Member("Y", r.ShortName, true);
            r.MemStore.Add(y);
            OpChain target = N.NMul.CreateOpChain(N.NPlus.Operate(x, y), N.NPlus.Operate(x, y));

            OneTransform.AddTransformWithNoSource(target.Sig);
            StrToOp dict = ER.BuildERChainsForLevel(target, r.ERStore.Values.ToList(), 10, 100);

            UIData.AllItems = OneTransform.AllResult.Keys.ToList();

            Assert.IsTrue(dict.ContainsKey("r[((X×X)+(((X×Y)×2)+(Y×Y)))]"));
        }
    }
}
