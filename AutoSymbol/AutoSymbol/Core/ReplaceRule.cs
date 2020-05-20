using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol.Core
{
    public enum ReplaceRuleDirection
    {
        Invalid,
        LeftSource,
        RightSource
    }
    public class ReplaceRule : Symbol
    {
        public OpChain Left;
        public OpChain Right;
        public string Name;

        public const string Match = "Match";

        private static HashSet<string> TriedBefore = new HashSet<string>();

        public override string ToString()
        {
            return string.Format("{0}={1}", Left.Sig, Right.Sig);
        }

        public static StrToOp BuildERChainsForLevel(
            OpChain toChange,
            List<ReplaceRule> multiER,
            int maxLevel,
            int maxSizePerGen,
            Optimizer optimizer = null)
        {
            TriedBefore.Clear();

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
                    item.Value.AssertChildrenWeightConsistency();
                    foreach (var er in multiER)
                    {
                        bool condition = item.Key == d.TrackingSig
                            && (d.TrackingER == null ||
                            (er.Left.Sig == d.TrackingER.Left.Sig && er.Right.Sig == d.TrackingER.Right.Sig));
                        d.StartTreeMessage("Before BuildERChainAtAllBranchOnce",
                            condition,
                            er,
                            item.Value);
                        er.BuildERChainAtAllBranchOnce(dict[i + 1], item.Value);

                        item.Value.AssertChildrenWeightConsistency();
                    }
                    List<string> keys = dict[i + 1].Keys.ToList();
                    foreach (var str in keys)
                    {
                        dict[i + 1][str].AssertChildrenWeightConsistency();
                        ShortenOneChain(dict[i + 1][str], dict[i + 1]);
                        //dict[i + 1][str].AssertChildrenWeightConsistency();
                    }
                }

                //foreach (var item in dict[i + 1])
                //{
                //    item.Value.AssertChildrenWeightConsistency();
                //}

                StrToOp limited = ReduceSize(maxSizePerGen, dict[i + 1], i, optimizer);
                dict[i + 1] = limited;

                foreach (var item in dict[i + 1])
                {
                    item.Value.AssertChildrenWeightConsistency();
                    if (!total.ContainsKey(item.Key))
                        total[item.Key] = item.Value;
                }
            }
            return total;
        }

        private static StrToOp ReduceSize(int maxSizePerGen, StrToOp src, int level, Optimizer optimizer)
        {
            StrToOp limited = new StrToOp();

            if (optimizer == null)
            {
                foreach (var one in src.OrderBy(x => x.Key.Length).Take(maxSizePerGen))
                {
                    limited[one.Key] = one.Value;
                }
            }
            else
            {
                int childCount;
               

                WeightFunction wf = optimizer.GetWeightFunction(level);
                foreach (var one in src)
                {
                    //one.Value.AssertChildrenWeightConsistency();
                    wf.CalcWeight(one.Value, 0, out childCount);
                    one.Value.AssertChildrenWeightConsistency();
                }
                foreach (var one in src)
                {
                    one.Value.AssertChildrenWeightConsistency();
                }

                var list = src.OrderBy(x => x.Value.lastTotalWeight).Take(maxSizePerGen).ToList();
                foreach (KeyValuePair<string, OpChain> one in list)
                {
                    one.Value.AssertChildrenWeightConsistency();
                    limited[one.Key] = one.Value;
                }
            }

            return limited;
        }

        public static void ShortenOneChain(OpChain chain, StrToOp dict)
        {
            // OneTransform one = new OneTransform();
            //OpChain after = chain.Copy<OpChain>();      
            OpChain after = chain.MakeCopy();
            RecursiveShorten(after);
            OpChain.InvalidateAllSignature(after);

            if (chain.Sig != after.Sig)
            {
                dict[after.Sig] = after;
                OneTransform one = OneTransform.CreateNew(chain.Sig, after.Sig, "Shorten");

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
                    SetBase s = SetBase.AllSets[current.TargetSetName];
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
            TriedBefore.Clear();
            StrToOp dict = new StrToOp();
            dict[toChange.Sig] = toChange;
            OpChain toChangeClone = toChange.MakeCopy();
            RecursiveAddEquivalentChain(true, this.Left, this.Right, toChangeClone, ref toChangeClone, dict);
            RecursiveAddEquivalentChain(true, this.Right, this.Left, toChangeClone, ref toChangeClone, dict);
            return dict;
        }

        private void RecursiveAddEquivalentChain(bool continueWithResult, OpChain src, OpChain toCopy, OpChain root, ref OpChain toChange, StrToOp dict)
        {
            for (int i = 0; i < toChange.Operands.Length; i++)
                if (toChange.Operands[i].FromChain != null)
                {
                    d.LogTreeMessage("Try child chain");
                    RecursiveAddEquivalentChain(continueWithResult, src, toCopy, root, ref toChange.Operands[i].FromChain, dict);
                }

            OpChain result = TransformOneNodeInChain(src, toCopy, root, this.Name, ref toChange);

            if (result != null)
            {
                string sig = result.Sig;
                if (dict.ContainsKey(sig))
                    return;
                else
                {
                    dict[sig] = result;

                    if (continueWithResult)
                    {
                        d.LogTreeMessage("Start recursive on result", result.Sig);
                        RecursiveAddEquivalentChain(continueWithResult, src, toCopy, result, ref result, dict);
                    }
                }
            }
        }

        public static OpChain TransformOneNodeInChain(OpChain erSrc, OpChain erTarget, OpChain root, string reason, ref OpChain toChange)
        {
            OpChain resultNode = TransformOneRoot(erSrc, erTarget, toChange);
            //   OneTransform one = new OneTransform();

            if (resultNode != null)
            {
                OpChain toResotre = toChange;
                toChange = resultNode;
                OneTransform one;
                OpChain newRootResult;
                if (toResotre != root)
                {
                    //newRootResult= root.Copy<OpChain>();
                    newRootResult = root.MakeCopy();
                }
                else
                {
                    newRootResult = resultNode;
                }

                toChange = toResotre;
                string rootSig = root.Sig;
                string newRootResultSig = newRootResult.Sig;
                one = OneTransform.CreateNew(rootSig, newRootResultSig, reason);

                if (one != null)
                {
                    one.ResultSig = newRootResult.Sig;
                    one.Original = root;
                    one.Result = newRootResult;

                    one.TransformType = TransformType.ERReplace;
                    one.BranchInOrigin = toChange;

                    one.TemplateSrc = erSrc;
                    one.TemplateTarget = erTarget;
                    one.BranchInResult = resultNode;
                    return newRootResult;
                }
                else
                {
                    return null;
                }
            }
            else
                return null;
        }
        public static OpChain TransformOneRoot(OpChain erSrc, OpChain erTarget, OpChain toChange)
        {
            Dictionary<string, Member> keyMap = new Dictionary<string, Member>();

            d.LogTreeMessage("Pairing and replace one branch", erSrc, toChange);
            if (Match == RecursiveDiscoverPair(erSrc, toChange, keyMap))
            {
                // OpChain retChain = erTarget.Copy<OpChain>();
                OpChain retChain = erTarget.MakeCopy();

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
            //chain.InvalidateSignature();

            for (int i = 0; i < chain.Operands.Length; i++)
            {
                if (chain.Operands[i].FromChain == null)
                {
                    if (keyMap.ContainsKey(chain.Operands[i].ShortName))
                        chain.Operands[i] = keyMap[chain.Operands[i].ShortName].MakeCopy();
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
        public static string RecursiveDiscoverPair(OpChain erSrc, OpChain toChange, Dictionary<string, Member> keyMap)
        {
            if (erSrc.Operator.ShortName != toChange.Operator.ShortName)
                return ErrStr("C1", erSrc.Operator.ShortName, toChange.Operator.ShortName);

            if (erSrc.Operator.ResultSetName != toChange.Operator.ResultSetName)
                return ErrStr("C2", erSrc.Operator.ResultSetName, toChange.Operator.ResultSetName);

            if (erSrc.Operands.Length != toChange.Operands.Length)
                return ErrStr("C3", erSrc.Operands.Length.ToString(), toChange.Operands.Length.ToString());

            for (int i = 0; i < erSrc.Operands.Length; i++)
            {
                if (erSrc.Operands[i].FromChain == null)
                {
                    if (erSrc.Operands[i].IsVariable == true)
                    {
                        string currentSrcKey = erSrc.Operands[i].ShortName;
                        if (!keyMap.ContainsKey(currentSrcKey))
                        {
                            keyMap[currentSrcKey] = toChange.Operands[i];
                        }
                        else
                        {
                            // If already matches a chain, then this chain must match the one in dictionary exactly
                            if (keyMap[currentSrcKey].Sig != toChange.Operands[i].Sig)
                                return ErrStr("C4", keyMap[currentSrcKey].ShortName, toChange.Operands[i].ShortName);
                        }

                        //if (keyMap[currentSrcKey].ShortName != toChange.Operands[i].ShortName)
                        //    return ErrStr("C4", keyMap[currentSrcKey].ShortName, toChange.Operands[i].ShortName);
                    }
                    else
                    {
                        if (erSrc.Operands[i].ShortName != toChange.Operands[i].ShortName)
                            return ErrStr("C7", "non-variable should match exactly", "null");
                    }
                }
                else
                {
                    if (toChange.Operands[i].FromChain == null)
                        return ErrStr("C5", "toChange.Operands[i].FromChain", "null");

                    string childStatus = RecursiveDiscoverPair(erSrc.Operands[i].FromChain, toChange.Operands[i].FromChain, keyMap);
                    if (childStatus != Match)
                        return ErrStr("C6", "ChildStatus", "Match");
                }
            }

            return Match;
        }



    }

    public abstract class RuleSet<T> where T : SetBase
    {
        public static void Subscribe(RuleSet<T> erSet, T t)
        {
            erSet.CreateAll(t);
        }

        public abstract void CreateAll(T t);
    }
}
