using System;
using System.Collections.Generic;

namespace MathGen
{

    public class SetEx : ISet
    {
        List<ISet> Slots;
    }
    public class N : ISet
    {
    }

       

    public class OpEx
    {
        public List<ISet> Sources;
        public List<ISet> Target;
    }
    
    public class Actor
    {
        public List<ISet> SetDepot;

        public Actor()
        {
            SetDepot = new List<ISet>();
            SetDepot.Add(new N());
        }

        public void CreateRationalNumber()
        {
            /// Goal: generate rational number
            /// Step 1 Create tree templates
            /// Step 2 Use tree to construct TargetSet
            /// Step 3 Random set template input and generate TargetSet
            /// Step 4 Use TargetSet to generate self-map operator
            /// Step 5 Use sample number to generate sample result
            /// Step 6 Sample result can be used to construct ER
            /// Step 7 If different tree satisfies ER, that will be interest set and op
            /// 
        }

        public void CreateSlotOperator()
        {
            /// Step 1 SlotOperator can be for any tree and any slot
            /// Step 2 Meaningful operator forms short ER
            /// Step 3
        }
    }

   

    public class OperatorTemplate
    {
        /// Sample generation logic
        /// Observer criteria
        /// Allowed Symmetry 
        /// 
    }
}
