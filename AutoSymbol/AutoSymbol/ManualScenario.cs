using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol
{
    public class ManualScenario
    {
        public OpChain StartChain;
        public List<OneTransform> IdealTransforms;
    }

    public class TrainingSet
    {
        public static Dictionary<string, ManualScenario> All = new Dictionary<string, ManualScenario>();

        static TrainingSet()
        {
            Add_ProveAPlusBCubic();
        }
        public static void Add_ProveAPlusBCubic()
        {
            ManualScenario ms = new ManualScenario();
            ms.StartChain = OpChainHelper.For_ProveAPlusBCubic();
            All["ProveAPlusBCubic"] = ms;
        }
    }

    public class OpChainHelper
    {
        public static OpChain For_ProveAPlusBCubic()
        {
            N n = new N();
            Member x = new Member("X", n.ShortName, true);
            n.MemStore.Add(x);
            Member y = new Member("Y", n.ShortName, true);
            n.MemStore.Add(y);
            OpChain target = N.NMul.CreateOpChain(N.NPlus.Operate(x, y),
                N.NMul.Operate(N.NPlus.Operate(x, y), N.NPlus.Operate(x, y)));
            return target;
        }
    }
}
