using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathNet
{
    public class Prototype
    {
    }

    public class Two<X, Y> { }
    public class One<X> { }

    public class Morph<I,O> { }

    public class Member<T> { }
    public class Any<T> : Member<T> { }

    public class Statement { }

    public class Eq
    {
        public Statement Left;
        public Statement Right;
    }

    public class SetBase
    {
        public List<SetBase> Supoorted;
        public List<Eq> Relations = new List<Eq>();
        public SetBase()
        {
        }

        public void ApplyRelation()
        {
            /// Challenge:
            /// Symbol match among statement, relation and morphism
            /// One solution : define AnyA, AnyB, AnyC and use it as replacement unit, rely on external observer to sort out how to do replacement.
            /// Write Proof as : From  : Apply ER to Operator to Apply ER to a search criteria.
            /// 
            /// Question : how to author search criteria to template format? And limit combinations?
            /// The goal is to generate even longer EquivalentRelations and Store them as cache.
            /// Give high priority ER more bandwidth to proceed.
            /// Manually define the beautiful level of ER.
            /// 
        }
    }

    public class G : SetBase
    {
        public Morph<Two<Any<G>, Any<G>>, Member<G>> Mul;
        public Morph<One<Any<G>>, Member<G>> Inv;
        public Member<G> Id;

        public Member<G> Mul1 (Any<G> g, Any<G> h)
        {
            return new Member<G>();
        }
        public G() : base()
        {
            Relations.Add(new Eq
            {
                /// Make Statement work
                /// Left = 
            });
        }

    }

    public class Rn
    {

    }

    public class Manifold { }

    public class LieGroup : SetBase
    {
        public Morph<LieGroup, Rn> Charts;
    }

    public class ProofTemplate
    {
        ///
    }
}
