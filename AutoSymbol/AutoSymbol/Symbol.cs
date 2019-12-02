using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol
{
    
    //public class SymStore<T> where T : Symbol
    //{
    //    Dictionary<string, T> dictionary = new Dictionary<string, T>();

    //    public void Add(T t)
    //    {
    //        dictionary[t.ShortName] = t;
    //    }

    //    public T this[string key]
    //    {
    //        get
    //        {
    //            return dictionary[key];
    //        }
    //        set
    //        {
    //            dictionary[key] = value;
    //        }
    //    }

    //    public bool ContainsKey(string key)
    //    {
    //        return dictionary.ContainsKey(key);
    //    }

    //    public List<T> Values
    //    {
    //        get
    //        {
    //            return dictionary.Values.ToList<T>();
    //        }
    //    }

    //}
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
        public bool IsVariable = false;

        public Member MakeCopy()
        {
            Member ret = new Member(this.ShortName,this.TargetSetName,this.IsVariable);
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

    public class MemberStore : Dictionary<string, Member>
    {
        public void Add(Member mem)
        {
            base.Add(mem.ShortName, mem);

            if (OneTransform.AllResult.ContainsKey(mem.Sig) == false)
            {
                if (mem.FromChain == null)
                    OneTransform.AddTransformWithNoSource(mem.Sig);
                else
                {
                    OneTransform small = OneTransform.AddTransformWithNoSource(mem.shortSig);
                    small.Result = mem.FromChain;
                    small.ResultSig = mem.shortSig;

                    OneTransform big = OneTransform.AddTransformWithNoSource(mem.Sig);
                    big.Result = mem.FromChain;
                    big.ResultSig = mem.Sig;
                }
            }
        }
    }

    public class EquivalentRelationStore : Dictionary<string, ER>
    {
        public new ER this[string key]
        {
            get
            {
                return base[key];
            }
            set
            {
                value.Name = key;
                base[key] = value;
            }
        }
    }

    public enum ERDirection
    {
        Invalid,
        LeftSource,
        RightSource
    }
    public class Set : Symbol
    {
        public Dictionary<string, Operator> OpStore = new Dictionary<string, Operator>();
        public MemberStore MemStore = new MemberStore();
        public Dictionary<string, Member> ShortMemStore = new Dictionary<string, Member>();
        public EquivalentRelationStore ERStore = new EquivalentRelationStore();
        public Dictionary<string, string> SigToShortName = new Dictionary<string, string>();
        public static Dictionary<string, Set> AllSets = new Dictionary<string, Set>();


        static Set()
        {
        }
        public Set(string name) : base(name)
        {
            AllSets[name] = this;
        }

        public static Set GetSetByName(string name)
        {
            return AllSets[name];
        }

        public static List<ManualTransform> GetAllManualTransform()
        {
            List<ManualTransform> ret = new List<ManualTransform>();
            foreach(var one in AllSets)
            {
                ManualTransform s = new ManualTransform();
                s.MyType = TransformType.Shorten;
                s.TargetSet = one.Value;             
                ret.Add(s);

                foreach (var er in one.Value.ERStore)
                {
                    ManualTransform left = new ManualTransform();
                    left.MyType = TransformType.ERReplace;
                    left.TargetSet = one.Value;
                    left.ER = er.Value;
                    left.Direction = ERDirection.LeftSource;
                    ret.Add(left);

                    ManualTransform right = new ManualTransform();
                    right.MyType = TransformType.ERReplace;
                    right.TargetSet = one.Value;
                    right.ER = er.Value;
                    right.Direction = ERDirection.RightSource;
                    ret.Add(right);
                }
            }
            return ret;
        }


    }

   
   




}
