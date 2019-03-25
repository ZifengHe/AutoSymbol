﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol
{
    public class Benchmark
    {
        public static void RunAll()
        {
            Benchmark b = new Benchmark();
            b.ProveTwoPlusThree();
            b.ProveAPlusBSquare();
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
            /// 1. N.SigToShortName for OpChain
            /// 2. Visit all subChain that contains ShortName and 
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

            Dictionary<string, OpChain> dict = n.ERStore["NPlusAssoc"].BuildCompleteERChains(result);
            UIData.ItemMap = dict;
            UIData.AllItems = OneTransform.All.Keys.ToList();

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
