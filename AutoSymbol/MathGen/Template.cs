using System;
using System.Collections.Generic;

namespace MathGen
{
    public class Template
    {
    }

    public class SelfMap : Template
    {
        public MorphismSet Create(Set a, Set b)
        {
            return new MorphismSet(a, b);
        }
    }

    public class MorphismSet : Set
    {
        public MorphismSet(Set a, Set b)
        {
        }
    }

    public class Operator
    { }
    public class Set :R
    {

    }

    public class N { }
    public class R : N { }

    public class Actor
    {
        public List<Set> SetDepot;
        public List<Template> TemplateDepot;

        public void Expand()
        { }
    }
}
