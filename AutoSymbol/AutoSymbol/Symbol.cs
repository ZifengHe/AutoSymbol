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
        public SymStore<ER> ERStore = new SymStore<ER>();
        public Dictionary<string, string> SigToShortName = new Dictionary<string, string>();
        public static Dictionary<string, Set> AllSets = new Dictionary<string, Set>();


        public Set(string name) : base(name)
        {
            AllSets[name] = this;
        }
    }

    public class Operator : Symbol
    {
        public string ResultSetName;
        public Operator(string name, Set targetSet) : base(name)
        {
            ResultSetName = targetSet.ShortName;
        }

        public Member Operate(Member[] mems)
        {
            OpChain chain = new OpChain();
            chain.Operator = this;
            chain.Operands = mems;

            return chain.CreateMember("NotSet");
        }
        public OpChain CreateOpChain(Member[] mems)
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
            mem.LevelTwoName = PrintByDepth(2);
            return mem;
        }

        public string PrintFull()
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

    public class OneTransform
    {
        public static Dictionary<string, OneTransform> All = new Dictionary<string, OneTransform>();
        public static Dictionary<string, Dictionary<string, Member>> Keymaps = new Dictionary<string, Dictionary<string, Member>>();
        public OpChain Src;
        public OpChain ToCopy;
        public OpChain ChangeRoot;
        public OpChain Result;
        public OpChain ChangePartTo;
        public OpChain ChangePartFrom;
        public string ResultSig;
    }
    public class ER : Symbol
    {
        public OpChain Left;
        public OpChain Right;
        public HashSet<string> Names = new HashSet<string>();

        public const string Match = "Match";

        public static Dictionary<string, OpChain> BuildERChainsForLevel(OpChain toChange, List<ER> multiER, int maxLevel)
        {
            Dictionary<string, OpChain>[] dict = new Dictionary<string, OpChain>[maxLevel];
            Dictionary<string, OpChain> total = new Dictionary<string, OpChain>();
            dict[0] = new Dictionary<string, OpChain>();
            dict[0][toChange.Sig] = toChange;
            total[toChange.Sig] = toChange;
            for (int i = 0; i < maxLevel; i++)
            {
                dict[i + 1] = new Dictionary<string, OpChain>();
                foreach (var item in dict[i])
                {
                    foreach (var er in multiER)
                    {
                        er.BuildERChainOnce(dict[i + 1], item.Value);
                    }
                }

                foreach (var item in dict[i + 1])
                {
                    if (!total.ContainsKey(item.Key))
                        total[item.Key] = item.Value;
                }
            }
            return total;
        }

        public void ShortenOneChain(OpChain chain, Dictionary<string, OpChain> dict)
        {
            string sigBefore = chain.Sig;
            RecursiveShorten(chain);
            string sigAfter = chain.Sig;
            dict.Remove(sigBefore);
            dict[sigAfter] = chain;

            OneTransform.All[sigAfter] = OneTransform.All[sigBefore];
            OneTransform.All.Remove(sigBefore);
        }

        public void RecursiveShorten(OpChain chain)
        {
            /// This is the key to avoid expanding similar ones.
            /// 1. try to replace every child branch
            /// 2. Recalculate the sig and update dictionary
            /// 3. Update the corresponding transform
            /// 

            for (int i = 0; i < chain.Operands.Length; i++)
            {
                Member current = chain.Operands[i];
                if (current.FromChain != null)
                {
                    Set s = Set.AllSets[current.TargetSetName];
                    string longSig = current.FromChain.Sig;
                    if (s.SigToShortName.ContainsKey(longSig))
                    {
                        chain.Operands[i] = s.MemStore[s.SigToShortName[longSig]];
                       
                        continue;
                    }
                    else
                        RecursiveShorten(current.FromChain);
                }

            }
        }
        public void BuildERChainOnce(Dictionary<string, OpChain> dict, OpChain toChange)
        {
            RecursiveAddEquivalentChain(false, this.Left, this.Right, toChange, ref toChange, dict);
            RecursiveAddEquivalentChain(false, this.Right, this.Left, toChange, ref toChange, dict);
        }

        public Dictionary<string, OpChain> BuildCompleteERChains(OpChain toChange)
        {
            Dictionary<string, OpChain> dict = new Dictionary<string, OpChain>();
            RecursiveAddEquivalentChain(true, this.Left, this.Right, toChange, ref toChange, dict);
            RecursiveAddEquivalentChain(true, this.Right, this.Left, toChange, ref toChange, dict);
            return dict;
        }

        private void RecursiveAddEquivalentChain(bool continueWithResult, OpChain src, OpChain toCopy, OpChain root, ref OpChain toChange, Dictionary<string, OpChain> dict)
        {
            for (int i = 0; i < toChange.Operands.Length; i++)
                if (toChange.Operands[i].FromChain != null)
                {
                    RecursiveAddEquivalentChain(continueWithResult, src, toCopy, root, ref toChange.Operands[i].FromChain, dict);
                }

            OpChain result = TransformOneNodeInChain(src, toCopy, root, ref toChange);

            if (result != null)
            {
                string sig = result.PrintFull();
                if (dict.ContainsKey(sig))
                    return;
                else
                {
                    dict[sig] = result;

                    if (continueWithResult)
                        RecursiveAddEquivalentChain(continueWithResult, src, toCopy, result, ref result, dict);
                }
            }
        }

        public static OpChain TransformOneNodeInChain(OpChain src, OpChain toCopy, OpChain root, ref OpChain toChange)
        {
            OpChain result = TransformOneRoot(src, toCopy, toChange);
            OneTransform one = new OneTransform();

            if (result != null)
            {
                one.ChangePartFrom = toChange;
                OpChain toResotre = toChange;
                toChange = result;
                one.Src = src;
                one.ToCopy = toCopy;
                one.ChangePartTo = result;


                if (toResotre != root)
                {
                    one.Result = root.Copy<OpChain>();      // This one is patched with result
                    one.ResultSig = root.PrintFull();
                }
                else
                {
                    one.Result = result;
                    one.ResultSig = one.Result.Sig;
                }
                // Now fix the root to its original branch
                toChange = toResotre;
                one.ChangeRoot = root;

                OneTransform.All[one.ResultSig] = one;
                return one.Result;
            }

            else
                return null;
        }
        public static OpChain TransformOneRoot(OpChain src, OpChain toCopy, OpChain toChange)
        {
            Dictionary<string, Member> keyMap = new Dictionary<string, Member>();

            if (Match == RecursivePair(src, toChange, keyMap))
            {
                OpChain retChain = toCopy.Copy<OpChain>();

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
