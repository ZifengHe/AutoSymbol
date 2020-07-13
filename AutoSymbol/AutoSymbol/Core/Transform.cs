using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol.Core
{
    public class OpByStr  : Dictionary<string, OpNode>
    { }

    public class ManualTransform
    {
        public TransformType MyType;
        public SetBase TargetSet;
        public ReplaceRule ER;
        public ReplaceRuleDirection Direction;

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
    public class TransformRecord
    {
        public TransformType TransformType;
        public static Dictionary<string, TransformRecord> AllRecordBySig = new Dictionary<string, TransformRecord>();
        public static Dictionary<string, Dictionary<string, Member>> Keymaps = new Dictionary<string, Dictionary<string, Member>>();
        public  static int GlobalSequenceNum = 0;

        public int Gen = -1;
        public int SequenceNumber = 0;

        public OpNode TemplateSrc;
        public OpNode TemplateTarget;
        public OpNode Original;
        public OpNode Result;
        public OpNode BranchInResult;
        public OpNode BranchInOrigin;
        public string ResultSig;
        public string Reason;

        public static TransformRecord AddTransformWithNoSource(string sig)
        {
            return new TransformRecord(true, sig);
        }

        public static void Reset()
        {
            AllRecordBySig.Clear();
            Keymaps.Clear();
            GlobalSequenceNum = 0;
        }

        public TransformRecord(bool IsNoSource, string sig)
        {
            lock(AllRecordBySig)
            {
                GlobalSequenceNum++;
                this.SequenceNumber = GlobalSequenceNum;
            }

            if (IsNoSource)
            {
                this.Gen = 0;

                if (AllRecordBySig.ContainsKey(sig))
                    throw new ApplicationException("Duplicate GenZero signature");
                AllRecordBySig[sig] = this;
            }
        }      

        public static TransformRecord CreateNew(string original, string result, string reason)
        {
          //  d.BreakOnSequence(137);
                
            if(AllRecordBySig.ContainsKey(original)== false) 
                return null;

            if(AllRecordBySig.ContainsKey(result) == true)
            {
                if (AllRecordBySig[result].Original == null)
                    return null;

                /// Below logic avoids circular reference so that we can always find the root
                string existingOriginal = AllRecordBySig[result].Original.Sig;
                if (existingOriginal.Length <= original.Length 
                    || AllRecordBySig[original].SequenceNumber >= AllRecordBySig[result].SequenceNumber)
                    return null;
            }

            TransformRecord ret = new TransformRecord(false, result);
            ret.Reason = reason;
            ret.Gen = AllRecordBySig[original].Gen + 1;
            AllRecordBySig[result] = ret;

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
