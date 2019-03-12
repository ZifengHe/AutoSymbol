using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol
{
    public class SymStore<T> where T : Symbol
    {
        Dictionary<string, T> dictionary = new Dictionary<string, T>();

        public void Add(T t)
        {
            dictionary[t.ShortName] = t;
        }

        public T this[string key]
        {
            get
            {
                return dictionary[key];
            }        
            set
            {
                dictionary[key] = value;
            }
        }

        public bool ContainsKey(string key)
        {
            return dictionary.ContainsKey(key);
        }

    }
    public class Symbol
    {
        public static int RandomCounter = 0;
        public  string ShortName;
        public string FullName;

        public Symbol()
        {
            // Temporary name
            this.ShortName = NextRandomName();
        }

        public Symbol(string name)
        {
            this.ShortName = name;
        }

        public static string NextRandomName()
        {
            string ret = "T" + RandomCounter.ToString();
            RandomCounter++;
            return ret;
        }
    }

    public class Member : Symbol
    {
        public Set TargetSet;
        public Member(string shortName, Set targetSet) : base(shortName)
        {
            this.TargetSet = targetSet;
        }
    }
    public class Set : Symbol
    {
        public SymStore<Operator> OpStore = new SymStore<Operator>();
        public SymStore<Member> MemStore = new SymStore<Member>();
        public SymStore<ER> ERStore = new SymStore<ER>();

        public Set(string name) : base(name)
        {
        }
    }

    public class Operator : Symbol
    {
        public Set ResultSet;
        public Operator(string name, Set targetSet) : base(name)
        {
            ResultSet = targetSet;
        }

        public OpChain Operate(Symbol [] symbols)
        {
            OpChain chain = new OpChain();
            chain.Operator = this;
            chain.Operands = symbols;

            return chain;
        }
    }

    public class ER : Symbol
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

        public  Member CreateMember(string shortName)
        {
            if (Operator == null || Operands.Length == 0)
                throw new ApplicationException();

            StringBuilder sb = new StringBuilder();
            VisitOpChain(this, sb);

            Member mem = new Member(shortName, this.Operator.ResultSet);
            mem.FullName = sb.ToString();
            return mem;
        }

        private void VisitOpChain(OpChain chain, StringBuilder sb)
        {
            sb.AppendFormat("{0}(", chain.Operator.ShortName);
            for(int i=0; i< this.Operands.Length; i++)
            {
                if (this.Operands[i] is OpChain)
                {
                    VisitOpChain((OpChain)this.Operands[i], sb);
                }
                else
                    sb.AppendFormat("[{0}]", this.Operands[i].ShortName);
            }

            sb.Append(")");
        }   
    }
}
