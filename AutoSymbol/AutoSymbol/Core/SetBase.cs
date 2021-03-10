using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol.Core
{
    public static class GlobalRules
    {
        public static RuleStore RuleStore = new RuleStore();
    }
    public class SetBase : Symbol
    {
        public Dictionary<string, Operator> OpStore = new Dictionary<string, Operator>();
        public MemberStore MemStore = new MemberStore();
        public Dictionary<string, Member> ShortMemStore = new Dictionary<string, Member>();
        //public RuleStore RuleStore = new RuleStore();
        public Dictionary<string, string> SigToShortName = new Dictionary<string, string>();
        public static Dictionary<string, SetBase> AllSets = new Dictionary<string, SetBase>();

        public SetBase Parent = null;


        static SetBase()
        {
        }
        public SetBase(string name) : base(name)
        {
            AllSets[name] = this;
        }

        public static SetBase GetSetByName(string name)
        {
            return AllSets[name];
        }

        public static List<ManualTransform> GetAllManualTransform()
        {
            List<ManualTransform> ret = new List<ManualTransform>();
            foreach (var one in AllSets)
            {
                ManualTransform s = new ManualTransform();
                s.MyType = TransformType.Shorten;
                s.TargetSet = one.Value;
                ret.Add(s);

                foreach (var er in GlobalRules.RuleStore)
                {
                    ManualTransform left = new ManualTransform();
                    left.MyType = TransformType.ERReplace;
                    left.TargetSet = one.Value;
                    left.ER = (ReplaceRule)er.Value;
                    left.Direction = ReplaceRuleDirection.LeftSource;
                    ret.Add(left);

                    ManualTransform right = new ManualTransform();
                    right.MyType = TransformType.ERReplace;
                    right.TargetSet = one.Value;
                    right.ER = (ReplaceRule)er.Value;
                    right.Direction = ReplaceRuleDirection.RightSource;
                    ret.Add(right);
                }
            }
            return ret;
        }

        public Operator CreateOperatorIfNotExist(string name, bool bSingleOperation)
        {
            if (this.OpStore.ContainsKey(name))
                return this.OpStore[name];
            else
            {
                Operator op = new Operator(name, this, bSingleOperation);
                this.OpStore[name] = op;
                return op;
            }
        }

       

    }

  
}
