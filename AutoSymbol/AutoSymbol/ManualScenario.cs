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
            OpChain target = n.NMul.CreateOpChain(n.NPlus.Operate(x, y),
                n.NMul.Operate(n.NPlus.Operate(x, y), n.NPlus.Operate(x, y)));
            return target;
        }
    }
}
