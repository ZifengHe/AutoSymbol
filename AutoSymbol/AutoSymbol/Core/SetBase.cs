using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol.Core
{
    public class SetBase : Symbol
    {
        public Dictionary<string, Operator> OpStore = new Dictionary<string, Operator>();
        public MemberStore MemStore = new MemberStore();
        public Dictionary<string, Member> ShortMemStore = new Dictionary<string, Member>();
        public EquivalentRelationStore ERStore = new EquivalentRelationStore();
        public Dictionary<string, string> SigToShortName = new Dictionary<string, string>();
        public static Dictionary<string, SetBase> AllSets = new Dictionary<string, SetBase>();


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
