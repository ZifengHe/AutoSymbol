using System;
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
        public static void RunOne()
        {
            Benchmark b = new Benchmark();
            b.ProveTwoPlusXPlusThree();
        }
        public static void RunAll()
        {
            Benchmark b = new Benchmark();
            b.ProveTwoPlusThree();
            b.ProveTwoPlusXPlusThree();
        }

        public void ProveComplementSetTheorem()
        { }

        public void ProveAPlusBSquareCrossSets()
        { }
        public void ProveAPlusBSquare()
        {
            /// Step 1   Build equivalent chains so that I can get a dictionary
            /// Step 2   Apply multiple ER and try to use simple symbol to replace longer chains
            /// Step 3   Search the target pattern.
        }

        public void ProveTopEquivalentSymbolsByLevel()
        {
            /// Step 1 BuildEquivalentChainByComplexityLevel
            /// Step 2 Keep updating the equivalent dictionary
            /// Step 3 Ignore subChain that is fully explored.
        }

        public void ProveTwoPlusXPlusThree()
        {            
            N n = new N();
            Member x = new Member("x", n.ShortName);
            n.MemStore.Add(x);
            OpChain start = n.NPlus.CreateOpChain(n.MemStore["2"], n.NPlus.Operate(x, n.MemStore["3"]));
            StrToOp dict = ER.BuildERChainsForLevel(start, n.ERStore.Values, maxLevel: 5, 20);
            UIData.AllItems = OneTransform.AllResult.Keys.ToList();

            bool found = false;
            foreach (var one in dict)
                if (one.Key == "+([5][x])")
                    found = true;
            Assert(found, "Fail to find 5");
        }
        public void ProveTwoPlusThree()
        {
            N n = new N();
            OpChain result = n.OpStore["+"].CreateOpChain(new Member[] {
                n.MemStore["2"],
                n.MemStore["3"]
            });
            
            Member mem = result.CreateMember("Test");
            Trace.WriteLine(mem.FromChain.PrintByDepth(0));
            Trace.WriteLine(mem.FromChain.PrintByDepth(1));
            Trace.WriteLine(mem.FromChain.PrintByDepth(2));
            Assert(mem.FromChain.PrintByDepth(4), "+(+([1][1])+(+([1][1])[1]))");
            Trace.WriteLine(n.ERStore["NPlusAssoc"].Left.PrintFull());
            Trace.WriteLine(n.ERStore["NPlusAssoc"].Right.PrintFull());

            StrToOp dict = n.ERStore["NPlusAssoc"].BuildCompleteERChains(result);
           // UIData.ItemMap = dict;
            UIData.AllItems = OneTransform.AllResult.Keys.ToList();

            bool found = false;
            foreach (var one in dict)
                if (one.Key == n.MemStore["5"].Sig)
                    found = true;
            Assert(found, "Fail to find 5");
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
