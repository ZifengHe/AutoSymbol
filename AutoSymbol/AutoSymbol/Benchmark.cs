using System;
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
            Trace.WriteLine(mem.FromChain.PrintByDepth(0));
            Trace.WriteLine(mem.FromChain.PrintByDepth(1));
            Trace.WriteLine(mem.FromChain.PrintByDepth(2));
            Assert(mem.FromChain.PrintByDepth(4), "+(+([1][1])+(+([1][1])[1]))");
            Trace.WriteLine(n.ERStore["NPlusAssoc"].Left.PrintFull());
            Trace.WriteLine(n.ERStore["NPlusAssoc"].Right.PrintFull());

            Dictionary<string, OpChain> dict = n.ERStore["NPlusAssoc"].BuildEquivalentChains(result);
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
