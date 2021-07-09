using System;
using System.Collections.Generic;
using System.Text;

namespace MathGen
{
    public interface IOperator
    {
        public ISet GetResultSet();
    }
    
    public class Operator : IOperator
    {
        public static List<IOperator> OpDepot;
        public TargetSet TargetSet;

        public List<int> FirstSlotMap = new List<int>();
        public List<int> SecondSlotMap = new List<int>();

        public static List<IOperator> CreateOperators(int totalOperators)
        {
            return null;
        }
        public ISet GetResultSet()
        {
            return TargetSet;
        }
    }
}
