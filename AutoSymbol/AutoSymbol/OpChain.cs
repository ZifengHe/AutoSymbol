using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol
{
    public class OpDict<T> : Dictionary<OpChain, T>
    { }
    public class Operator : Symbol
    {
        public string ResultSetName;
        public Operator(string name, Set targetSet) : base(name)
        {
            ResultSetName = targetSet.ShortName;
        }

        public Member Operate(params Member[] mems)
        {
            OpChain chain = new OpChain();
            chain.Operator = this;
            chain.Operands = mems;

            return chain.CreateMember("NotSet");
        }
        public OpChain CreateOpChain(params Member[] mems)
        {
            OpChain chain = new OpChain();
            chain.Operator = this;
            chain.Operands = mems;

            return chain;
        }
    }


    public class OpChain
    {
        public Operator Operator;
        public Member[] Operands;

        public OpChain() : base()
        {
        }

        public OpChain MakeCopy()
        {
            OpChain ret = new OpChain();
            ret.Operator = this.Operator;
            ret.Operands = new Member[this.Operands.Length];

            for(int i=0; i < this.Operands.Length; i++)
            {
                if (this.Operands[i] != null)
                {
                    ret.Operands[i] = new Member(this.Operands[i].ShortName,this.Operands[i].TargetSetName);
                    //ret.Operands[i].LevelTwoName = this.Operands[i].LevelTwoName;
                    if (this.Operands[i].FromChain != null)
                        ret.Operands[i].FromChain = this.Operands[i].FromChain.MakeCopy();
                }
            }

            return ret;
        }

        public string Sig
        {
            get
            {
                return PrintFull();
            }
        }

        public List<OpChain> GetAllChildren()
        {
            List<OpChain> list = new List<OpChain>();
            list.Add(this);
            for (int i = 0; i < Operands.Length; i++)
                if (Operands[i].FromChain != null)
                    list.AddRange(Operands[i].FromChain.GetAllChildren());
            return list;
        }

        public Member CreateMember(string shortName)
        {
            if (Operator == null || Operands.Length == 0)
                throw new ApplicationException();

            Member mem = new Member(shortName, this.Operator.ResultSetName);
            mem.FromChain = this;
            //mem.LevelTwoName = PrintByDepth(2);
            return mem;
        }

        public string PrintFull()
        {
            return string.Format("{0}[{1}]", this.Operator.ResultSetName, RecursivePrint());
        }

        private string RecursivePrint()
        {
            string final = null;
            for (int i = 0; i < 100; i++)
            {
                string current = PrintByDepth(i);
                if (final == current)
                    return final;
                else
                    final = current;
            }
            throw new ApplicationException("too deep");
        }

        public string PrintByDepth(int depth)
        {
            StringBuilder sb = new StringBuilder();
            VisitOpChain(this, sb, depth);
            return sb.ToString();
        }

        private void VisitOpChain(OpChain chain, StringBuilder sb, int depth)
        {
            sb.AppendFormat("{0}(", chain.Operator.ShortName);
            for (int i = 0; i < chain.Operands.Length; i++)
            {

                if (depth <= 0 || chain.Operands[i].FromChain == null)
                {
                    sb.AppendFormat("[{0}]", chain.Operands[i].ShortName);
                }
                else
                {
                    VisitOpChain(chain.Operands[i].FromChain, sb, depth - 1);
                }
            }

            sb.Append(")");
        }
    }

}
