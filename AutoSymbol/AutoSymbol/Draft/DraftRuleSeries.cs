using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol.Draft
{
    class DraftRuleSeries
    {
    }
}

namespace AutoSymbol.Core
{
    //public enum RuleOption
    //{
    //    Replace_LtoR,
    //    Replace_RtoL,
    //    Merge_Const_Plus,
    //    Merge_Const_Multiply,
    //    Sort_Commute_Plus,
    //    Sort_Commute_Multiply
    //}

    public class RuleSeries
    {
        public List<RuleNode> RuleList = new List<RuleNode>();

        public List<OpNode> ApplyRules(OpNode start)
        {
            List<OpNode> list = new List<OpNode>();
            OpNode current = start;
            foreach (var one in RuleList)
            {
                OpNode next = one.ApplyRule(current);

                if (next != null)
                {
                    list.Add(next);
                    current = next;
                }
                else
                    continue;
            }

            return list;
        }

        public void AddOneRule(BaseRule rule)
        {
            RuleNode rn = new RuleNode();
            rn.Rule = rule;
            RuleList.Add(rn);
        }
    }

    public class SortCommuteRule : BaseRule
    {
        public override void ProcessSingleNode(ref Member mem)
        {
            NodeProc(ref mem, this.Op);
        }
    }

    public class MergeRule : BaseRule
    {
        public override void ProcessSingleNode(ref Member mem)
        {
            NodeProc(ref mem, this.Op);
        }
    }

    public partial class ReplaceRule : BaseRule
    {
        public override void ProcessSingleNode(ref Member mem)
        {
            NodeProc(ref mem, this.Op);
        }
    }

    //public abstract class RecursiveRule : BaseRule
    //{    
    //    public static OpNode Run(OpNode root, RecursiveRule rule)
    //    {
    //        for (int i = 0; i < root.Operands.Length; i++)
    //        {
    //            rule.RunRecursive(ref root.Operands[i]);
    //        }

    //        return null;
    //    }

    //    private void RunRecursive(ref Member mem)
    //    {
    //        if (mem.FromChain != null)
    //        {
    //            for (int i = 0; i < mem.FromChain.Operands.Length; i++)
    //            {
    //                RunRecursive(ref mem.FromChain.Operands[i]);
    //            }
    //            this.ProcessSingleNode(ref mem);
    //        }
    //    }

    //}

    



    public class RuleNode
    {
        public BaseRule Rule;

        public OpNode ApplyRule(OpNode start)
        {
            //TransformOneNodeInChain
            //ReplaceRule.TransformOneRoot()


            //if (this.Rule is RecursiveRule)
            //    return RecursiveRule.Run(start, this.Rule as RecursiveRule);
            if (this.Rule is ReplaceRule)
            {
                ReplaceRule rr = Rule as ReplaceRule;
                return ReplaceRule.TransformOneRoot(
                        rr.Left,
                        rr.Right,
                        start);
            }

            throw new ApplicationException("Must be missing rules");

        }
    }
}

