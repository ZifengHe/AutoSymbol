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

        public Dictionary<string, OperatorSymbol> Operators = new Dictionary<string, OperatorSymbol>();
        public Dictionary<string, Symbol> AllocatedMember = new Dictionary<string, Symbol>();
        public Dictionary<string, ER> AllER = new Dictionary<string, ER>();
        

        public Symbol(string name)
        {
            this.Name = name;
        }
       
        public virtual Symbol FindMember()
        {
            for(int i=1; i < 100; i++)
            {
                string key = "s" + i.ToString();
                if(!AllocatedMember.ContainsKey(key))
                {
                    Symbol sym = InsertMemberByKey(key);
                    return sym;
                }
            }

            throw new ApplicationException("key depleted");
        }
        public virtual Symbol FindMemberByKey(string key)
        {
            if (AllocatedMember.ContainsKey(key))
            {
                return AllocatedMember[key];
            }
            else
                return null;
        }

        public virtual Symbol InsertMemberByKey(string key)
        {
            Symbol sym = new Symbol(string.Format("{{0}}[{1}]", key, this.Name));
            AllocatedMember[key] = sym;
            return sym;
        }
    }

    public abstract class OperatorSymbol : Symbol
    {
        public OperatorSymbol(string name) : base(name)
        {
        }

        public abstract Symbol Operate(Symbol [] symbols);
    }

    public class ER
    {
        public OpChain Left;
        public OpChain Right;

    }

    public class OpChain
    {

    }
}
