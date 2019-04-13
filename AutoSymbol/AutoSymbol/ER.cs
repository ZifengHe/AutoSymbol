using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol
{
    public class ER : Symbol
    {
        public OpChain Left;
        public OpChain Right;
        public HashSet<string> Names = new HashSet<string>();

        public const string Match = "Match";

       

        public static StrToOp BuildERChainsForLevel(
            OpChain toChange,
            List<ER> multiER, 
            int maxLevel,
            int maxSizePerGen)
        {
            StrToOp[] dict = new StrToOp[maxLevel + 1];
            StrToOp total = new StrToOp();
            dict[0] = new StrToOp();
            dict[0][toChange.Sig] = toChange;
            total[toChange.Sig] = toChange;
            for (int i = 0; i < maxLevel; i++)
            {
                dict[i + 1] = new StrToOp();
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

                    StrToOp limited = new StrToOp();

                    foreach(var one in dict[i + 1].OrderBy(x => x.Key.Length).Take(maxSizePerGen))
                    {
                        limited[one.Key] = one.Value;
                    }
                    dict[i + 1] = limited;
                }

                foreach (var item in dict[i + 1])
                {
                    if (!total.ContainsKey(item.Key))
                        total[item.Key] = item.Value;
                }
            }
            return total;
        }

        public static void ShortenOneChain(OpChain chain, StrToOp dict)
        {
           // OneTransform one = new OneTransform();
            //OpChain after = chain.Copy<OpChain>();      
            OpChain after = chain.MakeCopy();
            RecursiveShorten(after);

            if(chain.Sig != after.Sig)
            {
                dict[after.Sig] = after;
                OneTransform one = OneTransform.CreateNew(chain.Sig, after.Sig);

                if (one != null)
                {
                    one.Original = chain;
                    one.Result = after;
                    one.TransformType = TransformType.Shorten;
                }
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
        public void BuildERChainAtAllBranchOnce(StrToOp dict, OpChain toChange)
        {
            RecursiveAddEquivalentChain(false, this.Left, this.Right, toChange, ref toChange, dict);
            RecursiveAddEquivalentChain(false, this.Right, this.Left, toChange, ref toChange, dict);
        }

        public StrToOp BuildCompleteERChains(OpChain toChange)
        {
            StrToOp dict = new StrToOp();
            dict[toChange.Sig] = toChange;
            RecursiveAddEquivalentChain(true, this.Left, this.Right, toChange, ref toChange, dict);
            RecursiveAddEquivalentChain(true, this.Right, this.Left, toChange, ref toChange, dict);
            return dict;
        }

        private void RecursiveAddEquivalentChain(bool continueWithResult, OpChain src, OpChain toCopy, OpChain root, ref OpChain toChange, StrToOp dict)
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
            OpChain nodeResult = TransformOneRoot(src, toCopy, toChange);
         //   OneTransform one = new OneTransform();

            if (nodeResult != null)
            {
                OpChain toResotre = toChange;
                toChange = nodeResult;
                OneTransform one;
                OpChain newRootResult;
                if (toResotre != root)
                {
                     newRootResult= root.Copy<OpChain>();
                   
                }
                else
                {
                    newRootResult = nodeResult;
                }

                toChange = toResotre;
                one = OneTransform.CreateNew(root.Sig, newRootResult.Sig);

                if (one != null)
                {
                    one.ResultSig = newRootResult.Sig;
                    one.Original = root;
                    one.Result = newRootResult;

                    one.TransformType = TransformType.ERReplce;
                    one.BranchInOrigin = toChange;

                    one.TemplateSrc = src;
                    one.TemplateTarget = toCopy;
                    one.BranchInResult = nodeResult;
                    return newRootResult;
                }
                return null;
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
