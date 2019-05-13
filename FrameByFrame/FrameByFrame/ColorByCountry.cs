using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FrameByFrame
{
    public static class ColorByCountry
    {
        public static Dictionary<string, Color> All = new Dictionary<string, Color>();

        static ColorByCountry()
        {
            All["CN"] = Color.FromRgb(255, 0, 0);
            All["AR"] = Color.FromRgb(173, 216, 230);
            All["AU"] = Color.FromRgb(107, 142, 35);
            All["BR"] = Color.FromRgb(46, 139, 87);
            All["CA"] = Color.FromRgb(219, 11, 11);
            All["DE"] = Color.FromRgb(0, 0, 0);
            All["IN"] = Color.FromRgb(255, 165, 0);
            All["ID"] = Color.FromRgb(247, 26, 26);
            All["JP"] = Color.FromRgb(0, 0, 139);
        }
    }
}
