using System;
using System.Collections.Generic;
using System.Text;

namespace MathGen
{
    public class NPlus : IOperator
    {
        public ISet GetResultSet()
        {
            return TargetSet.KnownSets["N"];
        }
    }

    public class NMulti : IOperator
    {
        public ISet GetResultSet()
        {
            return TargetSet.KnownSets["N"];
        }
    }
}
