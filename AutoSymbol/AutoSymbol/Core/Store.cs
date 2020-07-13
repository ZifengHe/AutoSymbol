using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol.Core
{

    public class MemberStore : Dictionary<string, Member>
    {
        public void Add(Member mem)
        {
            base.Add(mem.ShortName, mem);

            if (TransformRecord.AllRecordBySig.ContainsKey(mem.Sig) == false)
            {
                if (mem.FromChain == null)
                    TransformRecord.AddTransformWithNoSource(mem.Sig);
                else
                {
                    TransformRecord small = TransformRecord.AddTransformWithNoSource(mem.shortSig);
                    small.Result = mem.FromChain;
                    small.ResultSig = mem.shortSig;

                    TransformRecord big = TransformRecord.AddTransformWithNoSource(mem.Sig);
                    big.Result = mem.FromChain;
                    big.ResultSig = mem.Sig;
                }
            }
        }
    }

    public class RuleStore : Dictionary<string, BaseRule>
    {
        public new BaseRule this[string key]
        {
            get
            {
                return base[key];
            }
            set
            {
                value.Name = key;
                base[key] = value;
            }
        }
    }

}
