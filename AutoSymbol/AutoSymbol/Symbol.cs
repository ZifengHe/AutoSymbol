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
        public string LevelTwoName;
        public Member(string shortName, string targetSetName) : base(shortName)
        {
            this.TargetSetName = targetSetName;
        }

        public string Sig
        {
            get {
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
        public SymStore<ER> ERStore = new SymStore<ER>();

        public Set(string name) : base(name)
        {
        }
    }

    public class Operator : Symbol
    {
        public string ResultSetName;
        public Operator(string name, Set targetSet) : base(name)
        {
            ResultSetName = targetSet.ShortName;
        }

        public Member Operate2(Member[] mems)
        {
            OpChain chain = new OpChain();
            chain.Operator = this;
            chain.Operands = mems;

            return chain.CreateMember("NotSet");
        }
        public OpChain Operate(Member[] mems)
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

        public string Sig
        {
            get
            {
                return PrintFull();
            }
        }

        public Member CreateMember(string shortName)
        {
            if (Operator == null || Operands.Length == 0)
                throw new ApplicationException();

            Member mem = new Member(shortName, this.Operator.ResultSetName);
            mem.FromChain = this;
            mem.LevelTwoName = PrintByDepth(2);
            return mem;
        }

        public string PrintFull()
        {
            string final = null;
            for(int i=0; i< 100;i++)
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

    public class OneTransform
    {
        public static Dictionary<string, OneTransform> All = new Dictionary<string, OneTransform>();
        public static Dictionary<string, Dictionary<string, Member>> Keymaps = new Dictionary<string, Dictionary<string, Member>>();
        public OpChain Src;
        public OpChain ToCopy;
        public OpChain ToChange;
        public OpChain Result;
        public string ResultSig;
    }
    public class ER : Symbol
    {
        public OpChain Left;
        public OpChain Right;
        public HashSet<string> Names = new HashSet<string>();

        public const string Match = "Match";

        public Dictionary<string, OpChain> BuildEquivalentChains(OpChain toChange)
        {
            Dictionary<string, OpChain> dict = new Dictionary<string, OpChain>();
            RecursiveAddEquivalentChain(this.Left, this.Right, toChange, dict);
            RecursiveAddEquivalentChain(this.Right, this.Left, toChange, dict);
            return dict;
        }

        private void RecursiveAddEquivalentChain(OpChain src, OpChain toCopy, OpChain toChange, Dictionary<string, OpChain> dict)
        {
            //Use clone, because transform can happen at branch level
            OpChain clone = toChange.Copy<OpChain>();
            string cloneSig = clone.PrintFull();
            if (dict.ContainsKey(cloneSig))
                return;
            else
            {
                dict[cloneSig] = clone;
            }

            for (int i = 0; i < clone.Operands.Length; i++)
            {
                OpChain current = clone.Operands[i].FromChain;
                if (current != null)
                    RecursiveAddEquivalentChain(src, toCopy, current, dict);
            }

            OpChain result =  TransformChain(src, toCopy, clone );

            if(result != null)
            {
                OneTransform one = new OneTransform
                {
                    Result = result,
                    ResultSig = result.PrintFull(),
                    Src = src,
                    ToChange = clone,
                    ToCopy = toCopy
                };
                OneTransform.All[one.ResultSig] = one;
                RecursiveAddEquivalentChain(src, toCopy, result, dict);
            } 
        }

        public static OpChain  TransformChain(OpChain src, OpChain toCopy, OpChain toChange)
        {
            Dictionary<string, Member> keyMap = new Dictionary<string, Member>();

            if (Match == RecursivePair(src, toChange, keyMap))
            {
                OpChain retChain =  toCopy.Copy<OpChain>();

                //toCopy is now the template, needs to be replaced with all the original member in toChange
                RecursiveReplace(retChain, keyMap);

                if (retChain != null)
                    OneTransform.Keymaps[retChain.Sig] = keyMap;

                return retChain;
            }
            return null;
        }

        private static void RecursiveReplace(OpChain chain, Dictionary<string, Member> keyMap)
        {
            for (int i = 0; i < chain.Operands.Length; i++)
            {
                if (chain.Operands[i].FromChain == null)
                {
                    if (!keyMap.ContainsKey(chain.Operands[i].ShortName))
                        throw new ApplicationException("The name should exist.");
                    chain.Operands[i] = keyMap[chain.Operands[i].ShortName];
                }
                else
                {
                    RecursiveReplace(chain.Operands[i].FromChain, keyMap);
                }
            }
        }

        private static string ErrStr(string hint, string s1, string s2)
        {
            return string.Format("{0} : {1}!={2}", hint, s1, s2);
        }
        public static string RecursivePair(OpChain src, OpChain toChange, Dictionary<string, Member> keyMap)
        {
            if (src.Operator.ShortName != toChange.Operator.ShortName)
                return ErrStr("C1", src.Operator.ShortName, toChange.Operator.ShortName);

            if (src.Operator.ResultSetName != toChange.Operator.ResultSetName)
                return ErrStr("C2", src.Operator.ResultSetName, toChange.Operator.ResultSetName);

            if (src.Operands.Length != toChange.Operands.Length)
                return ErrStr("C3", src.Operands.Length.ToString(), toChange.Operands.Length.ToString());

            for (int i = 0; i < src.Operands.Length; i++)
            {
                if (src.Operands[i].FromChain == null)
                {
                    string currentSrcKey = src.Operands[i].ShortName;
                    if (!keyMap.ContainsKey(currentSrcKey))
                    {
                        keyMap[currentSrcKey] = toChange.Operands[i];
                    }

                    if (keyMap[currentSrcKey].ShortName != toChange.Operands[i].ShortName)
                        return ErrStr("C4", keyMap[currentSrcKey].ShortName, toChange.Operands[i].ShortName);
                }
                else
                {
                    if (toChange.Operands[i].FromChain == null)
                        return ErrStr("C5", "toChange.Operands[i].FromChain", "null");

                    string childStatus = RecursivePair(src.Operands[i].FromChain, toChange.Operands[i].FromChain, keyMap);
                    if (childStatus != Match)
                        return ErrStr("C6", "ChildStatus", "Match");
                }
            }

            return Match;
        }



    }



}
