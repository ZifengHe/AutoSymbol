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

            if (OneTransform.AllResult.ContainsKey(mem.Sig) == false)
            {
                if (mem.FromChain == null)
                    OneTransform.AddTransformWithNoSource(mem.Sig);
                else
                {
                    OneTransform small = OneTransform.AddTransformWithNoSource(mem.shortSig);
                    small.Result = mem.FromChain;
                    small.ResultSig = mem.shortSig;

                    OneTransform big = OneTransform.AddTransformWithNoSource(mem.Sig);
                    big.Result = mem.FromChain;
                    big.ResultSig = mem.Sig;
                }
            }
        }
    }

    public class ReplaceRuleStore : Dictionary<string, ReplaceRule>
    {
        public new ReplaceRule this[string key]
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
