using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol
{
    using OpDict = Dictionary<string, OpChain>;

    public enum TransformType
    {
        Invalid,
        ERReplce,
        Shorten
    }
    public class OneTransform
    {
        public TransformType TransformType;
        public static Dictionary<string, OneTransform> AllResult = new Dictionary<string, OneTransform>();
        public static Dictionary<string, Dictionary<string, Member>> Keymaps = new Dictionary<string, Dictionary<string, Member>>();

        /// <summary>
        ///  Step 1 Use OpChain as key, not sig anymore.
        ///  Step 2 add a dictionary for all the shorten operations
        /// </summary>
        public OpChain TemplateSrc;
        public OpChain TemplateTarget;
        public OpChain Original;
        public OpChain Result;
        public OpChain BranchInResult;
        public OpChain BranchInOrigin;
        public string ResultSig;
    }

    public class OneProof
    {
        public static Dictionary<string, OneTransform> All = new Dictionary<string, OneTransform>();
        public List<OneTransform> Transforms = new List<OneTransform>();

        public OneProof Append(OneTransform one)
        {
            OneProof ret = new OneProof();
            for (int i = 0; i < Transforms.Count; i++)
                ret.Transforms.Add(Transforms[i]);
            ret.Transforms.Add(one);
            return ret;
        }
    }

}
