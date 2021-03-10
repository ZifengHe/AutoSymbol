using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Diagnostics;

namespace RiemannZeta
{
    public class Polynomial
    {
        public List<Complex> Items = new List<Complex>();

        public void Initiate(Complex num)
        {
            Items.Clear();
            Items.Add(num);
            Items.Add(new Complex(1, 0));
        }

        public Polynomial MultiplySimple(Complex one)
        {
            Polynomial ret = new Polynomial();

            ret.Items.Add(Items[0] * one);

            for(int i=0; i < Items.Count;i++)
            {
                Complex current = Items[i];
                if (i != Items.Count - 1)
                    current += one * Items[i + 1];
                ret.Items.Add(current);
            }

            return ret;
        }

        public void TraceAllItems()
        {
            for (int i = 0; i < Items.Count; i++)
                Trace.TraceInformation($"{i}  Real={Items[i].Real} Imginary={Items[i].Imaginary}");
        }

    }
}
