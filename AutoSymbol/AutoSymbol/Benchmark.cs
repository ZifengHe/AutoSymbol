using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
            OneTransform.AllResult.Clear();
            OneTransform.Keymaps.Clear();
        }
        public void ProveComplementSetTheorem()
        { }

        public void ProveAPlusBSquareCrossSets()
        { }

        public void ProveAPlusBCubic()
        {
            OpChain target = OpChainHelper.For_ProveAPlusBCubic();
            OneTransform.AddTransformWithNoSource(target.Sig);
            StrToOp dict = ER.BuildERChainsForLevel(target, new N().ERStore.Values.ToList(), 10, 150);
            UIData.AllItems = OneTransform.AllResult.Keys.ToList();

            //AssertKeyInDictionary("N[((X×X)+(((X×Y)×2)+(Y×Y)))]", dict);
        }
        public void ProveAPlusBSquare()
        {
            N n = new N();
            Member x = new Member("X", n.ShortName, true);
            n.MemStore.Add(x);
            Member y = new Member("Y", n.ShortName, true);
            n.MemStore.Add(y);
            OpChain target = n.NMul.CreateOpChain(n.NPlus.Operate(x, y), n.NPlus.Operate(x, y));

            OneTransform.AddTransformWithNoSource(target.Sig);
            StrToOp dict = ER.BuildERChainsForLevel(target, n.ERStore.Values.ToList(), 10, 100);

            UIData.AllItems = OneTransform.AllResult.Keys.ToList();

            AssertKeyInDictionary("N[((X×X)+(((X×Y)×2)+(Y×Y)))]", dict);
        }

       

        public void ProveTwoPlusXPlusThree()
        {            
            N n = new N();
            Member x = new Member("X", n.ShortName, true);
            n.MemStore.Add(x);
            OpChain target = n.NPlus.CreateOpChain(n.MemStore["2"], n.NPlus.Operate(x, n.MemStore["3"]));

            OneTransform.AddTransformWithNoSource(target.Sig);
            StrToOp dict = ER.BuildERChainsForLevel(target, n.ERStore.Values.ToList(), 7, 20);

            UIData.AllItems = OneTransform.AllResult.Keys.ToList();

            AssertKeyInDictionary("N[(5+X)]", dict);
        }
        public void ProveTwoPlusThree()
        {
            N n = new N();
            OpChain target = n.OpStore["+"].CreateOpChain(new Member[] {
                n.MemStore["2"],
                n.MemStore["3"]
            });
            
            Member mem = target.CreateMember("Test", true);
            Trace.WriteLine(mem.FromChain.PrintByDepth(0));
            Trace.WriteLine(mem.FromChain.PrintByDepth(1));
            Trace.WriteLine(mem.FromChain.PrintByDepth(2));
            Assert(mem.FromChain.PrintByDepth(4), "+(+([1][1])+(+([1][1])[1]))");
            Trace.WriteLine(n.ERStore["NPlusAssoc"].Left.Sig);
            Trace.WriteLine(n.ERStore["NPlusAssoc"].Right.Sig);

            OneTransform.AllResult.Clear(); // Only for this test case.
            OneTransform.AddTransformWithNoSource(target.Sig);
            StrToOp dict = n.ERStore["NPlusAssoc"].BuildCompleteERChains(target);
           // UIData.ItemMap = dict;
            UIData.AllItems = OneTransform.AllResult.Keys.ToList();

            bool found = false;
            foreach (var one in dict)
                if (one.Key == n.MemStore["5"].Sig)
                    found = true;
            Assert(found, "Fail to find 5");
        }

        public void AssertKeyInDictionary(string key, StrToOp dict)
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
