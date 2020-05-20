using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoSymbol.Core;
using AutoSymbol.ERGroup;

namespace AutoSymbol.Category
{
    public class R : NumberSet
    {
        public R() : base("R")
        {
            HydrateER();
        }

        public void HydrateER()
        {
            RuleSet<R>.Subscribe(new NumberMergeRule<R>(), this);
            RuleSet<R>.Subscribe(new BasicPlusMulRule<R>(), this);
        }
    }
}
