using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol.Core
{
    public class OpDict<T> : Dictionary<OpChain, T>
    { }
    public class Operator : Symbol
    {
        public string ResultSetName = string.Empty;
        public string FirstInputSetName = string.Empty;
        public string SecondInputSetName = string.Empty;
        public bool IsSingleOperand = false;
        public Operator(string name, SetBase targetSet, bool bSingleOperand) : base(name)
        {
            ResultSetName = targetSet.ShortName;
            FirstInputSetName = ResultSetName;
            SecondInputSetName = ResultSetName;
            IsSingleOperand = bSingleOperand;
        }
       
        public Member Operate(params Member[] mems)
        {
            d.BreakOnCondition(IsSingleOperand != (mems.Length == 1));
            OpChain chain = new OpChain();
            chain.Operator = this;
            chain.Operands = mems;

            return chain.CreateMember("NotSet", true);
        }
        public OpChain CreateOpChain(params Member[] mems)
        {
            OpChain chain = new OpChain();
            chain.Operator = this;
            chain.Operands = mems;

            return chain;
        }

        public string Sig
        {
            get
            {
                return string.Format("{0}:={1}{2}{3}", ResultSetName, FirstInputSetName, this.ShortName, SecondInputSetName);
            }
        }
    }

    public class OpChain
    {
        /// !!!  Any new member needs to be included in MakeCopy()
        public Operator Operator;
        public Member[] Operands;
        //public ReadOnlyCollection<Member> Operands = new ReadOnlyCollection<Member>();
        public double lastTotalWeight = 0;
        public double lastLocalWeight = 0;
        private string sig = null;

        public OpChain() : base()
        {
        }

        public OpChain MakeCopy()
        {
            OpChain ret = new OpChain();
            ret.Operator = this.Operator;
            ret.Operands = new Member[this.Operands.Length];
            ret.lastLocalWeight = this.lastLocalWeight;
            ret.lastTotalWeight = this.lastTotalWeight;            

            for(int i=0; i < this.Operands.Length; i++)
            {
                if (this.Operands[i] != null)
                {
                    ret.Operands[i] = new Member(this.Operands[i].ShortName,
                        this.Operands[i].TargetSetName,
                        this.Operands[i].IsVariable);
                    //ret.Operands[i].LevelTwoName = this.Operands[i].LevelTwoName;
                    if (this.Operands[i].FromChain != null)
                        ret.Operands[i].FromChain = this.Operands[i].FromChain.MakeCopy();
                }
            }

            ret.sig = null;
            return ret;
        }

        public static void InvalidateAllSignature(OpChain chain)
        {
            chain.sig = null;

            foreach(var one in chain.Operands)
            {
                if (one.FromChain != null)
                    InvalidateAllSignature(one.FromChain);
            }
        }

        public string Sig
        {
            get
            {
              //  d.BreakOnCondition(sig != null && sig != NormalSig);

                if (sig == null)
                    sig = NormalSig;

                return sig;
                //return PrintFull();
                //return NormalSig;
            }
        }

        public string NormalSig
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0}[", this.Operator.ResultSetName);
                RecursiveLeftChildFirstPrint(this,sb);
                sb.Append("]");
                return sb.ToString();
            }
        }

        public string SigWithWeight
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0}[", this.Operator.ResultSetName);
                RecursivePrintWithWeight(this, sb);
                sb.Append("]TotalWeight=" + this.lastTotalWeight.ToString());
                return sb.ToString();
            }
        }

        public void RecursivePrintWithWeight(OpChain chain, StringBuilder sb)
        {
            double sumChildren = 0;
            sb.Append("(");
            for (int i = 0; i < chain.Operands.Length; i++)
            {
                if (chain.Operands[i].FromChain == null)
                {
                    sb.Append(chain.Operands[i].ShortName);
                }
                else
                {
                    RecursivePrintWithWeight(chain.Operands[i].FromChain, sb);
                    sumChildren += chain.Operands[i].FromChain.lastTotalWeight;
                }
                if (i != chain.Operands.Length - 1)
                    sb.AppendFormat("{0}[{1} {2}]", 
                        chain.Operator.ShortName, 
                        chain.lastTotalWeight.ToString(),
                        chain.lastLocalWeight.ToString());
            }

            double diff = Math.Abs(sumChildren + chain.lastLocalWeight - chain.lastTotalWeight);
            d.BreakOnCondition(diff > 0.01);
            sb.Append(")");
        }

        public void RecursiveLeftChildFirstPrint(OpChain chain, StringBuilder sb)
        {
            sb.Append("(");
            for (int i = 0; i < chain.Operands.Length; i++)
            {
                if (chain.Operands[i].FromChain == null)
                {
                    sb.Append(chain.Operands[i].ShortName);
                }
                else
                {
                    RecursiveLeftChildFirstPrint(chain.Operands[i].FromChain, sb);
                }
                if (i != chain.Operands.Length -1)
                    sb.Append(chain.Operator.ShortName);
            }
            sb.Append(")");
        }

    

        public void AssertChildrenWeightConsistency()
        {
            double testChildrenSum = 0;
            for (int i = 0; i < this.Operands.Length; i++)
            {
                if (this.Operands[i].FromChain != null)
                    testChildrenSum += this.Operands[i].FromChain.lastTotalWeight;
            }
            double diff = Math.Abs(testChildrenSum + this.lastLocalWeight - this.lastTotalWeight);
            d.BreakOnCondition(diff > 0.01);
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

        public Member CreateMember(string shortName, bool isVar)
        {
            if (Operator == null || Operands.Length == 0)
                throw new ApplicationException();

            Member mem = new Member(shortName, this.Operator.ResultSetName, isVar);
            mem.FromChain = this;
            //mem.LevelTwoName = PrintByDepth(2);
            return mem;
        }

        private string PrintFull()
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
