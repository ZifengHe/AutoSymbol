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
        public void ProveExistenceMinimumInPositiveSubset()
        {
            /// Exist n in A, n belongs to N => Exist x, Any y in A y>=x
            /// Can above be proven without using And OR => logic operations?
            /// 1. Definition of Minimum : 
            /// 2. ExistAny(x,y) becomes a new operator
            /// 3. AnyX ExistY : Y=X+1 (aeems an axiom) => ExitX AnyY 
            /// 4. =>  Seems like ChildSet production operation, it cannot reverse because lost certain properties
            /// 5. ER format  A=True,B=True ===== A=True,B=True,C=True
            /// 6.  AND OR becomes one operator as well
            /// 
            /// 
        }
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
