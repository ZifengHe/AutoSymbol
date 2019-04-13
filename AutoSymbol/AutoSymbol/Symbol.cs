using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public List<T> Values
        {
            get
            {
                return dictionary.Values.ToList<T>();
            }
        }

    }
    public class Symbol
    {
        public static int RandomCounter = 0;
        public string ShortName;

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
        public string TargetSetName;
        public OpChain FromChain;
        //public string LevelTwoName;
        public Member(string shortName, string targetSetName) : base(shortName)
        {
            this.TargetSetName = targetSetName;
        }

        public string Sig
        {
            get
            {
                if (this.FromChain == null)
                    return this.ShortName;
                return this.FromChain.Sig;
            }
        }
    }
    public class Set : Symbol
    {
        public SymStore<Operator> OpStore = new SymStore<Operator>();
        public SymStore<Member> MemStore = new SymStore<Member>();
        public SymStore<Member> ShortMemStore = new SymStore<Member>();
        public SymStore<ER> ERStore = new SymStore<ER>();
        public Dictionary<string, string> SigToShortName = new Dictionary<string, string>();
        public static Dictionary<string, Set> AllSets = new Dictionary<string, Set>();


        public Set(string name) : base(name)
        {
            AllSets[name] = this;
        }
    }

   
   




}
