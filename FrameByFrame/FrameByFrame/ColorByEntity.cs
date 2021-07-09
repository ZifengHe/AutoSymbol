using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FrameByFrame
{
    public static class ColorByEntity
    {
        public static Dictionary<string, Color> All = new Dictionary<string, Color>();

        static ColorByEntity()
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
            All["ID"] = Color.FromRgb(214, 23, 13);
            All["IT"] = Color.FromRgb(34, 139, 34);
            All["MY"] = Color.FromRgb(227, 240, 10);
            All["MX"] = Color.FromRgb(14, 171, 24);
            All["FR"] = Color.FromRgb(0, 0, 255);
            All["PH"] = Color.FromRgb(20, 66, 245);
            All["US"] = Color.FromRgb(242, 19, 19);
            All["GB"] = Color.FromRgb(247, 16, 16);
            All["RU"] = Color.FromRgb(71, 36, 235);
        }
    }
}
