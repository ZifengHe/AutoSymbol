using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol.Core
{
    public abstract class NumberSet : SetBase
    {
        public ConstMember One;

        public NumberSet(string name) : base(name)
        {
            One = new ConstMember("1", this.ShortName, false);
            this.MemStore.Add(One);


        }

        public ConstMember ConstPlus(params ConstMember [] members)
        {
            /// Need to figure out the logic here.
            /// 
            /// ER s
            /// 
            int result = 0;
            for(int i=0; i < members.Length; i++)
            {
                if (members[i].FromChain != null || members[i].FromChain.Operator != null)
                    throw new ApplicationException();

                result += int.Parse(members[i].ShortName);
            }

            string re = result.ToString();

            if(MemStore.ContainsKey(re))
            {
                this.MemStore.Add(new ConstMember(re, this.ShortName, false));
            }

            return (ConstMember) this.MemStore[re];
        }
       
    }
}
