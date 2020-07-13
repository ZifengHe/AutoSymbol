using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol.Core
{
    public class Member : Symbol
    {
        public string TargetSetName;
        public OpNode FromChain;
        public bool IsVariable = false;

        public Member MakeCopy()
        {
            Member ret = new Member(this.ShortName, this.TargetSetName, this.IsVariable);
            if (FromChain != null)
                ret.FromChain = this.FromChain.MakeCopy();
            return ret;
        }

        public Member(string shortName, string targetSetName, bool isVar) : base(shortName)
        {
            this.TargetSetName = targetSetName;
            this.IsVariable = isVar;
        }

        public string Sig
        {
            get
            {
                if (this.FromChain == null)
                    return string.Format("{0}[{1}]", this.TargetSetName, this.ShortName);
                return this.FromChain.Sig;
            }
        }

        public string shortSig
        {
            get
            {
                return string.Format("{0}[{1}]", this.TargetSetName, this.ShortName);
            }
        }
    }

    public class ConstMember : Member
    {
        public ConstMember(string shortName, string targetSetName, bool isVar) : base(shortName, targetSetName, isVar)
        {
        }

    }
}
