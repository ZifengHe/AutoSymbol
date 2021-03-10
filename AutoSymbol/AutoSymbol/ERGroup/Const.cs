using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol.ERGroup
{
    public partial class C
    {
        // Section::BasicPlusMulRule
        public const string PlusAssocOne = "PlusAssocOne";
        public const string PlusAssocTwo = "PlusAssocTwo";
        public const string PlusCommute = "PlusCommute";
        public const string MulCommute = "MulCommute";
        public const string MulDistrOne = "MulDistrOne";
        public const string MulDistrTwo = "MulDistrTwo";
        public const string MulAssocOne = "MulAssocOne";
        public const string MulAssocTwo = "MulAssocTwo";

        // Section: NumberMergeRule
        public const string OnePlusOne = "OnePlusOne";
        public const string AnyConstPlusOne = "AnyConstPlusOne";
        public const string AnyPlusOne = "AnyPlusOne";
        public const string MergeConstPlus = "MergeConstPlus";
        public const string MergeConstMultiply = "MergeConstMultiply";

        // Section GroupRule
        public const string GNormalChildDef = "GNormalChildDef";
        public const string GMulCommute = "GMulCommute";
        public const string GMulAssoc = "GMulAssoc";
        public const string GInvOne = "GInvOne";
        public const string GInvTwo = "GInvTwo";
        public const string GIdLeft = "GIdLeft";
        public const string GIdRight = "GIdRight";
    }
}
