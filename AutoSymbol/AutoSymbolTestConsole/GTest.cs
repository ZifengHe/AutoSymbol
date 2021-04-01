using AutoSymbol.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbolTestConsole
{
    class GTest : TestBase
    {
        public void ProveAbelianGroupNormalChildGroup()
        {
            /// 1. how to handle child set?
            /// Inherit rules, or copy rules
            /// 
            /// 2. Set operation completeness rule?
            /// Operator needs to specify target set
            /// 
            /// 3. Existence of e rule?
            /// Special member Id
            /// a.Inverse(a)=Id=Inverse(a).a=Id
            /// a.Id=a
            /// 
            /// 4. ChildGroup, cross group operators?
            /// ahInv(a)=h
            /// 
            /// 
            ///

        }

        public void ProveGropHomeomorphismTheorem()
        {
            ///  G/Ker(Sigma) =Iso= Im(Sigma)  for any Sigma : G->G'
            ///  1. Draft CrossProduct
            ///  2. Draft Morphism ER from CrossProduct Set
            ///  3. Draft GroupHomeo ER from Morphism
            ///  
            /// Next Step : figure out if we need a static G
            /// 1. Any set initialization is responsible to add ER
            /// 2. RuleStore : No need to answer this now. Will learn more later.
            /// 
            /// Is that possible to define ER without variable pros and cons?
            /// 1. Same member shows up multiple place in ER, clearly needs one instance
            /// 2. Cross ER passing scenario : member represents one open slot, that other tree can drop in
            /// 3. Hence ER needs a open slot function. F(o1) = G(o1)
            /// 4. Above should make the replacement work easier
            /// 5. Above should support at any time abstract one slot representation if others are given
            /// 6. Above 5 means like a Tensor.
            /// 
            /// Rebuild ReplaceRule logic so that no hard code name but clear intend and operation?
            /// Not only manual one, but also can include derived ones?
            /// 1. Add rule directly, signature as default name
            /// 2. Not Left and Right, but a equivalent list
            /// 3. The list needs to support,slot swap operation, one or many
            /// 4. Every children node is a child F(slot)
            /// 
            /// How to represent the structure without member name? So that it is easier to identify in hash?
            /// 1. Extend the abstract structure to remove-name for both set and operation
            /// 2. If abstract with above, it could potentially apply category
            /// 3. Group= Category(set, op, rules)
            /// 4. Explore this one deeper enough to change the way to code this
            /// 
            ///  Next step : Define the ER for morphism and GroupHomeo
            ///  1. ER for morphism :  Any(a) Exist(x, x.First=a)
            ///  
            /// The utility of morphism f is the ability to be applied to the both sides of ER
            /// It is possible morphism belongs to SetBase only
            /// Question : what about the set of morphism?
            ///  
        }
    }

    class CrossProduct<T, U> : SetBase where T : SetBase where U : SetBase
    {
        public CrossProduct(T t, U u) : base($"CrossProductOf^{t.ShortName}^{u.ShortName}")
        {
        }
        //public SetBase First;
        //public SetBase Second;
    }

    class Endomorphism { }

    class Morphism<T, U> where T : SetBase where U : SetBase
    {
        public static Operator Any;

        public SetBase Domain;
        public SetBase Codomain;


        public Morphism()
        {
            
        }
    }

    class GroupHomeomorphism<T, U> : Morphism<T, U> where T : AutoSymbol.Draft.GroupTheory.G
        where U : AutoSymbol.Draft.GroupTheory.G
    {
        public GroupHomeomorphism()
        {


        }
    }


}
