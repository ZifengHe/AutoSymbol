using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoSymbol.Core;
using AutoSymbol.Category;

namespace AutoSymbol.Draft.PrimeNumber
{

    public class ConditionBase { }
    public class CoPrime <SetBase>: ConditionBase
    {

    }

    public class ConditionalER : ReplaceRule
    {
        public static ReplaceRule GenCoPrimeER(Member a, Member b)
        {
            ReplaceRule er = new ReplaceRule();
            er.Left = NArray.Dim.CreateOpChain(NArray.NArrayMul.Operate(a, b));
            er.Right = N.NPlus.CreateOpChain(NArray.Dim.Operate(a), NArray.Dim.Operate(b));
            return er;
        }
    }

    public class FuncSet: SetBase
    {
        public SetBase SourceSet;
        public SetBase TargetSet;
        public FuncSet() : base("FuncSet")
        { }
    }

    //public class MappedOperator : Operator
    //{
    //  //  public MappedOperator(string name) : base()
    //}
    public class NArray : SetBase
    {
        public static Operator NArrayMul;
        public static Operator Dim;
        //public Dictionary<int, int> PrimePower = new Dictionary<int, int>();
        public NArray() : base("NArray")
        {
            NArrayMul = new Operator("NArrayMul", this, false);
            this.OpStore[NArrayMul.ShortName] = NArrayMul;

            Dim = new Operator("Dim", this, true);

            HydrateER();
        }

        public void HydrateER()
        {
            Member a = new Member("a", this.ShortName, true);
            Member b = new Member("b", this.ShortName, true);
            Member c = new Member("c", this.ShortName, true);
            Member n = new Member("n", this.ShortName, true);

            
        }

    }
}
