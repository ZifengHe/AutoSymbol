﻿using System;
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
        public OpChain FromChain;
        public string LevelTwoName;
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

        public OpChain Operate(Member [] mems)
        {
            OpChain chain = new OpChain();
            chain.Operator = this;
            chain.Operands = mems;

            return chain;
        }
    }

    public class ER : Symbol
    {
        public OpChain Left;
        public OpChain Right;
    }

    public class OpChain 
    {
        public Operator Operator;
        public Member [] Operands;

        public  OpChain (): base ()
        {
        }

        public  Member CreateMember(string shortName)
        {
            if (Operator == null || Operands.Length == 0)
                throw new ApplicationException();

            Member mem = new Member(shortName, this.Operator.ResultSet);
            mem.FromChain = this;
            mem.LevelTwoName = ExpandChainByDepth(2);
            return mem;
        }

        public string ExpandChainByDepth(int depth)
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
