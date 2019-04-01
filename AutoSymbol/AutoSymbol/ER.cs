using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol
{
    using OpDict = Dictionary<string, OpChain>;
    public class ER : Symbol
    {
        public OpChain Left;
        public OpChain Right;
        public HashSet<string> Names = new HashSet<string>();

        public const string Match = "Match";

        public static OpDict BuildERChainsForLevel(OpChain toChange, List<ER> multiER, int maxLevel)
        {
            OpDict[] dict = new OpDict[maxLevel + 1];
            OpDict total = new OpDict();
            dict[0] = new OpDict();
            dict[0][toChange.Sig] = toChange;
            total[toChange.Sig] = toChange;
            for (int i = 0; i < maxLevel; i++)
            {
                dict[i + 1] = new OpDict();
                foreach (var item in dict[i])
                {
                    foreach (var er in multiER)
                    {
                        er.BuildERChainAtAllBranchOnce(dict[i + 1], item.Value);
                    }
                    List<string> keys = dict[i + 1].Keys.ToList();
                    foreach (var str in keys)
                    {
                        ShortenOneChain(dict[i + 1][str], dict[i + 1]);
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

        public static void ShortenOneChain(OpChain chain, OpDict dict)
        {
            string sigBefore = chain.Sig;
            RecursiveShorten(chain);
            string sigAfter = chain.Sig;

            if (sigAfter != sigBefore)
            {
                dict.Remove(sigBefore);
                dict[sigAfter] = chain;
                OneTransform.AllResult[sigAfter] = OneTransform.AllResult[sigBefore];
                OneTransform.AllResult.Remove(sigBefore);
            }
        }

        private static void RecursiveShorten(OpChain chain)
        {
            for (int i = 0; i < chain.Operands.Length; i++)
            {
                Member current = chain.Operands[i];
                if (current.FromChain != null)
                {
                    Set s = Set.AllSets[current.TargetSetName];
                    string longSig = current.FromChain.Sig;
                    if (s.SigToShortName.ContainsKey(longSig))
                    {
                        chain.Operands[i] = s.ShortMemStore[s.SigToShortName[longSig]];
                        continue;
                    }
                    else
                        RecursiveShorten(current.FromChain);
                }
            }
        }
        public void BuildERChainAtAllBranchOnce(OpDict dict, OpChain toChange)
        {
            RecursiveAddEquivalentChain(false, this.Left, this.Right, toChange, ref toChange, dict);
            RecursiveAddEquivalentChain(false, this.Right, this.Left, toChange, ref toChange, dict);
        }

        public OpDict BuildCompleteERChains(OpChain toChange)
        {
            OpDict dict = new OpDict();
            dict[toChange.Sig] = toChange;
            RecursiveAddEquivalentChain(true, this.Left, this.Right, toChange, ref toChange, dict);
            RecursiveAddEquivalentChain(true, this.Right, this.Left, toChange, ref toChange, dict);
            return dict;
        }

        private void RecursiveAddEquivalentChain(bool continueWithResult, OpChain src, OpChain toCopy, OpChain root, ref OpChain toChange, OpDict dict)
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
                one.TransformType = TransformType.ERReplce;
                one.BranchInOrigin = toChange;
                OpChain toResotre = toChange;
                toChange = result;
                one.TemplateSrc = src;
                one.TemplateTarget = toCopy;
                one.BranchInResult = result;


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
                one.Original = root;

                OneTransform.AllResult[one.ResultSig] = one;
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
