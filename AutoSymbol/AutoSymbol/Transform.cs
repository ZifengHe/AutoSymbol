using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol
{
    public class StrToOp : Dictionary<string, OpChain>
    { }

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
        /// 

        public OneTransform(bool isRoot, string sig)
        {
            if (isRoot)
            {
                this.Gen = 0;
                AllResult[sig] = this;
            }
        }      
        
        public static OneTransform CreateNew(string original, string result)
        {
            if(AllResult.ContainsKey(original)== false || AllResult.ContainsKey(result) == true)
                return null;

            OneTransform ret = new OneTransform(false, result);
            ret.Gen = AllResult[original].Gen + 1;
            AllResult[result] = ret;

            return ret;
        }

        public int Gen = -1;

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
