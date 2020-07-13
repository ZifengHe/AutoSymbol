using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol.Core
{
    public delegate void ProcessNode(ref Member mem, Operator op);

    public abstract class NumberSet : SetBase
    {
        public ConstMember One;

        public NumberSet(string name) : base(name)
        {
            One = new ConstMember("1", this.ShortName, false);
            this.MemStore.Add(One);


        }

        public void SortPlus(ref Member mem, Operator op)
        {
        }


        public void ConstPlus(ref Member mem, Operator op)
        {
            if (mem.FromChain == null)
                return;

            OpNode node = mem.FromChain;

            if (node.Operator != op)
                return;

            int result = 0;
            for (int i = 0; i < node.Operands.Length; i++)
            {
                if ((node.Operands[i] is ConstMember) == false)
                    return;

                result += int.Parse(node.Operands[i].ShortName);
            }

            string re = result.ToString();
            ConstMember cm = new ConstMember(re, this.ShortName, false);

            if (MemStore.ContainsKey(re))
            {
                this.MemStore.Add(cm);
            }
            else
                cm = MemStore[re] as ConstMember;

            mem = cm;
        }


        public void ConstMultiply(ref Member mem, Operator op)
        {
            if (mem.FromChain == null)
                return;

            OpNode node = mem.FromChain;

            if (node.Operator != op)
                return;

            int result = 0;
            for (int i = 0; i < node.Operands.Length; i++)
            {
                if ((node.Operands[i] is ConstMember) == false)
                    return;

                result *= int.Parse(node.Operands[i].ShortName);
            }

            string re = result.ToString();
            ConstMember cm = new ConstMember(re, this.ShortName, false);

            if (MemStore.ContainsKey(re))
            {
                this.MemStore.Add(cm);
            }
            else
                cm = MemStore[re] as ConstMember;

            mem = cm;

        }

        //public ConstMember ConstPlus(params ConstMember [] members)
        //{           
        //    int result = 0;
        //    for(int i=0; i < members.Length; i++)
        //    {
        //        if (members[i].FromChain != null || members[i].FromChain.Operator != null)
        //            throw new ApplicationException();

        //        result += int.Parse(members[i].ShortName);
        //    }

        //    string re = result.ToString();

        //    if(MemStore.ContainsKey(re))
        //    {
        //        this.MemStore.Add(new ConstMember(re, this.ShortName, false));
        //    }

        //    return (ConstMember) this.MemStore[re];
        //}

    }
}
