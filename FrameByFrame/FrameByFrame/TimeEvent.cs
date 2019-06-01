using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameByFrame
{
    public class TimeEvent
    {
        /// <summary>
        /// Step 1 produce raw value
        /// Step 2 sort for evey year find top 10
        /// Step 3 present 1 or 2 per year randomly
        /// </summary>

        public string CurrentYear;
        public string CompareYear;
        public string FocusCountry;
        public string CompareTo;
        public string CompareIndicator;

        public double  FocusValue;
        public double CompareToValue;

        public double FocusGrowth;
        public double CompareToGrowth;        
    }
}
