using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol
{
    public class StrToOp : Dictionary<string, OpChain>
    { }

    public class ManualTransform
    {
        public TransformType MyType;
        public Set TargetSet;
        public ER ER;
        public ERDirection Direction;

        public string Key
        {
            get
            {
                if (MyType == TransformType.Shorten)
                    return string.Format("{0} {1}", TargetSet.ShortName, MyType.ToString());
                else
                    return string.Format("{0} {1} {2} {3}", 
                        TargetSet.ShortName, 
                        MyType.ToString(),
                        ER.ToString(),
                        Direction.ToString());
            }
        }
    }
    public enum TransformType
    {
        Invalid,
        ERReplace,
        Shorten
    }
    public class OneTransform
    {
        public TransformType TransformType;
        public static Dictionary<string, OneTransform> AllResult = new Dictionary<string, OneTransform>();
        public static Dictionary<string, Dictionary<string, Member>> Keymaps = new Dictionary<string, Dictionary<string, Member>>();
        public  static int GlobalSequenceNum = 0;

        public int Gen = -1;
        public int SequenceNumber = 0;

        public OpChain TemplateSrc;
        public OpChain TemplateTarget;
        public OpChain Original;
        public OpChain Result;
        public OpChain BranchInResult;
        public OpChain BranchInOrigin;
        public string ResultSig;
        public string Reason;

        public static OneTransform AddTransformWithNoSource(string sig)
        {
            return new OneTransform(true, sig);
        }

        public static void Reset()
        {
            AllResult.Clear();
            Keymaps.Clear();
            GlobalSequenceNum = 0;
        }

        public OneTransform(bool IsNoSource, string sig)
        {
            lock(AllResult)
            {
                GlobalSequenceNum++;
                this.SequenceNumber = GlobalSequenceNum;
            }

            if (IsNoSource)
            {
                this.Gen = 0;

                if (AllResult.ContainsKey(sig))
                    throw new ApplicationException("Duplicate GenZero signature");
                AllResult[sig] = this;
            }
        }      

        public static OneTransform CreateNew(string original, string result, string reason)
        {
          //  d.BreakOnSequence(137);
                
            if(AllResult.ContainsKey(original)== false) 
                return null;

            if(AllResult.ContainsKey(result) == true)
            {
                if (AllResult[result].Original == null)
                    return null;

                /// Below logic avoids circular reference so that we can always find the root
                string existingOriginal = AllResult[result].Original.Sig;
                if (existingOriginal.Length <= original.Length 
                    || AllResult[original].SequenceNumber >= AllResult[result].SequenceNumber)
                    return null;
            }

            OneTransform ret = new OneTransform(false, result);
            ret.Reason = reason;
            ret.Gen = AllResult[original].Gen + 1;
            AllResult[result] = ret;

            return ret;
        }

        
    }

    //public class OneProof
    //{
    //    public static Dictionary<string, OneTransform> All = new Dictionary<string, OneTransform>();
    //    public List<OneTransform> Transforms = new List<OneTransform>();

    //    public OneProof Append(OneTransform one)
    //    {
    //        OneProof ret = new OneProof();
    //        for (int i = 0; i < Transforms.Count; i++)
    //            ret.Transforms.Add(Transforms[i]);
    //        ret.Transforms.Add(one);
    //        return ret;
    //    }
    //}

}
