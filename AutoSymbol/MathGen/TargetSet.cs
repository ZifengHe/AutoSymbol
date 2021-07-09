using System;
using System.Collections.Generic;
using System.Text;

namespace MathGen
{   
    public interface ISet
    { }

    public class SlotOpBinding
    { }

    public class TargetSet: ISet
    {
        public static Dictionary<string, ISet> KnownSets = new Dictionary<string, ISet>();
        public static Random RandomGen = new Random();

        public List<ISet> Slots = new List<ISet>();
        public List<IOperator> OpCandidates = new List<IOperator>();
        public List<SlotOpBinding> BindingCandidates = new List<SlotOpBinding>();

        public static void CreateNSlotsSetFromKnownSets(int totalSlots, int totalOperators)
        {
        }
        public TargetSet(int totalSlots)
        {
            for (int i = 0; i < totalSlots; i++)
            {
                Slots[i] = PickRandomSet();
            }
        }

        public TargetSet(ISet first, ISet second)
        {
            Slots[0] = first;
            Slots[2] = second;
        }

        private ISet PickRandomSet()
        {
            List<ISet> list = new List<ISet>();
            list.AddRange(KnownSets.Values);
            return list[RandomGen.Next(list.Count)];
        }
    }
}
