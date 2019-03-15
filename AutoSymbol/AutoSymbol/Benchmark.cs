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
        }
        public void ProveAPlusBSquare()
        { }
        public void ProveTwoPlusThree()
        {
            N n = new N();
            OpChain result = n.OpStore["+"].Operate(new Member[] {
                n.MemStore["2"],
                n.MemStore["3"]
            });

            Member mem = result.CreateMember("Test");
            Trace.WriteLine(mem.FromChain.ExpandChainByDepth(0));
            Trace.WriteLine(mem.FromChain.ExpandChainByDepth(1));
            Trace.WriteLine(mem.FromChain.ExpandChainByDepth(2));
            Trace.WriteLine(mem.FromChain.ExpandChainByDepth(3));
            Trace.WriteLine(mem.FromChain.ExpandChainByDepth(4));

            Assert(mem.FromChain.ExpandChainByDepth(4), "+(+([1][1])+(+([1][1])[1]))");

        }

        public void Assert(string str1, string str2)
        {
            if (str1 != str2)
                throw new ApplicationException("Assert failed");
            else
                Trace.WriteLine("Assert Success");
        }
    }
}
