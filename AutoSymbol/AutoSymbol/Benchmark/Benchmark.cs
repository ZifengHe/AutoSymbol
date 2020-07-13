using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoSymbol.Core;
using AutoSymbol.Category;


namespace AutoSymbol
{
   // using StrToOp = Dictionary<string, OpChain>;
    public class Benchmark
    {
        public Benchmark()
        {
            StartNewCase();
        }
        public static void RunOne()
        {
            Benchmark b = new Benchmark();
            b.ProveAPlusBCubic();
            //b.ProveAPlusBSquare();
            //b.ProveTwoPlusThree();
            //b.ProveTwoPlusXPlusThree();
        }
        public static void RunAll()
        {
            new Benchmark().ProveTwoPlusThree();
            new Benchmark().ProveTwoPlusXPlusThree();
            new Benchmark().ProveAPlusBSquare();
        }

        public static void StartNewCase()
        {
            TransformRecord.AllRecordBySig.Clear();
            TransformRecord.Keymaps.Clear();
        }
        public void ProveComplementSetTheorem()
        { }

        public void ProveAPlusBSquareCrossSets()
        { }

        public void ProveMulticativeFunction()
        {
            /*
             * 
             */
        }

        public void ProveOneOneOntoChainTransfer()
        {
            /// B=f(A) Inv(f)(B)=A,  C=g(B) Inv(g)(C)=B
            /// => Inv(f)(Inv(g)(C))=A    C=g(f(A))
            /// 
        }
        public void ProveAPlusBCubic()
        {
            //OpNode target = OpChainHelper.For_ProveAPlusBCubic();
            //TransformRecord.AddTransformWithNoSource(target.Sig);
            //OpByStr dict = ReplaceRule.BuildERChainsForLevel(
            //    target, 
            //    new N().RuleStore.Values.ToList(),
            //    10, 
            //    150,
            //    Optimizer.GetOptimizer(OptimizerType.DoPolynormialExpansion));
            //UIData.AllItems = TransformRecord.AllRecordBySig.Keys.ToList();

            //AssertKeyInDictionary("N[((X×X)+(((X×Y)×2)+(Y×Y)))]", dict);
        }
        public void ProveAPlusBSquare()
        {
            //N n = new N();
            //Member x = new Member("X", n.ShortName, true);
            //n.MemStore.Add(x);
            //Member y = new Member("Y", n.ShortName, true);
            //n.MemStore.Add(y);
            //OpNode target = N.NMul.CreateOpChain(N.NPlus.Operate(x, y), N.NPlus.Operate(x, y));

            //TransformRecord.AddTransformWithNoSource(target.Sig);
            //OpByStr dict = ReplaceRule.BuildERChainsForLevel(target, n.RuleStore.Values.ToList(), 10, 100);

            //UIData.AllItems = TransformRecord.AllRecordBySig.Keys.ToList();

            //AssertKeyInDictionary("N[((X×X)+(((X×Y)×2)+(Y×Y)))]", dict);
        }       

        public void ProveTwoPlusXPlusThree()
        {            
            //N n = new N();
            //Member x = new Member("X", n.ShortName, true);
            //n.MemStore.Add(x);
            //OpNode target = N.NPlus.CreateOpChain(n.MemStore["2"], N.NPlus.Operate(x, n.MemStore["3"]));

            //TransformRecord.AddTransformWithNoSource(target.Sig);
            //OpByStr dict = ReplaceRule.BuildERChainsForLevel(target, n.RuleStore.Values.ToList(), 7, 20);

            //UIData.AllItems = TransformRecord.AllRecordBySig.Keys.ToList();

            //AssertKeyInDictionary("N[(5+X)]", dict);
        }
        public void ProveTwoPlusThree()
        {
           // N n = new N();
           // OpNode target = n.OpStore["+"].CreateOpChain(new Member[] {
           //     n.MemStore["2"],
           //     n.MemStore["3"]
           // });
            
           // Member mem = target.CreateMember("Test", true);
           // Trace.WriteLine(mem.FromChain.PrintByDepth(0));
           // Trace.WriteLine(mem.FromChain.PrintByDepth(1));
           // Trace.WriteLine(mem.FromChain.PrintByDepth(2));
           // Assert(mem.FromChain.PrintByDepth(4), "+(+([1][1])+(+([1][1])[1]))");
           // //Trace.WriteLine(n.RuleStore["NPlusAssoc"].Left.Sig);
           // //Trace.WriteLine(n.RuleStore["NPlusAssoc"].Right.Sig);

           // TransformRecord.AllRecordBySig.Clear(); // Only for this test case.
           // TransformRecord.AddTransformWithNoSource(target.Sig);
           // OpByStr dict = n.RuleStore["NPlusAssoc"].BuildCompleteERChains(target);
           //// UIData.ItemMap = dict;
           // UIData.AllItems = TransformRecord.AllRecordBySig.Keys.ToList();

           // bool found = false;
           // foreach (var one in dict)
           //     if (one.Key == n.MemStore["5"].Sig)
           //         found = true;
           // Assert(found, "Fail to find 5");
        }

        public void AssertKeyInDictionary(string key, OpByStr dict)
        {
            bool found = false;
            foreach (var one in dict)
                if (one.Key == key)
                    found = true;
            Assert(found, "Fail to find " + key);
        }
        public void Assert(string str1, string str2)
        {
            if (str1 != str2)
                throw new ApplicationException("Assert failed");
            else
                Trace.WriteLine("Assert Success");
        }
        public void Assert(bool b, string err)
        {
            if (!b)
                throw new ApplicationException(err);
            else
                Trace.WriteLine("Assert Success");
        }
    }
}
