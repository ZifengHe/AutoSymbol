using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol
{
    public class Symbol
    {
        public  string Name;
        public string FullName;

        public static int RandomCounter = 0;

        public Dictionary<string, Operator> Operators = new Dictionary<string, Operator>();
        public Dictionary<string, Symbol> DerivedMember = new Dictionary<string, Symbol>();
        public Dictionary<string, ER> AllER = new Dictionary<string, ER>();

        public Symbol ()
        {
            // Temporary name
            this.Name = "" + RandomCounter.ToString();
            RandomCounter++;
        }

        public Symbol(string name)
        {
            this.Name = name;
        }

        public virtual Symbol FindMember()
        {
            for(int i=1; i < 100; i++)
            {
                string key = "s" + i.ToString();
                if(!DerivedMember.ContainsKey(key))
                {
                    Symbol sym = InsertMemberByKey(key);
                    return sym;
                }
            }

            throw new ApplicationException("key depleted");
        }
        public virtual Symbol FindMemberByKey(string key)
        {
            if (DerivedMember.ContainsKey(key))
            {
                return DerivedMember[key];
            }
            else
                return null;
        }

        public virtual Symbol InsertMemberByKey(string key)
        {
            Symbol sym = new Symbol(string.Format("{{0}}[{1}]", key, this.Name));
            DerivedMember[key] = sym;
            return sym;
        }
    }

    public class Operator : Symbol
    {
        public Symbol ResultSet;
        public Operator(string name, Symbol sym) : base(name)
        {
            ResultSet = sym;
        }

        public OpChain Operate(Symbol [] symbols)
        {
            OpChain chain = new OpChain();
            chain.Operator = this;
            chain.Operands = symbols;

            return chain;
        }
    }

    public class ER
    {
        public OpChain Left;
        public OpChain Right;

    }

    public class OpChain : Symbol
    {
        public Operator Operator;
        public Symbol[] Operands;

        public  OpChain (): base ()
        {
        }

        public Symbol CreateSymbol(string shortName)
        {
            if(Operator == null || Operands.Length ==0)
                return null;

            StringBuilder sb = new StringBuilder();
            VisitOpChain(this, sb);          

            // TODO: process short name and long name better
            Symbol sym = Operator.ResultSet.InsertMemberByKey(sb.ToString());

            return sym;
        }

        private void VisitOpChain(OpChain chain, StringBuilder sb)
        {
            sb.AppendFormat("{0}(", chain.Operator.Name);
            for(int i=0; i< this.Operands.Length; i++)
            {
                if (this.Operands[i] is OpChain)
                {
                    VisitOpChain((OpChain)this.Operands[i], sb);
                }
                else
                    sb.AppendFormat("[{0}]", this.Operands[i].Name);
            }

            sb.Append(")");
        }

        
       
    }
}
