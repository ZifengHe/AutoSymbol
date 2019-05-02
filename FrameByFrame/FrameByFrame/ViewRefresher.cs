using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FrameByFrame
{
    public partial class MainWindow : Window
    {
        const int AxTop = 200;
        const int AxBottom = 800;
        const int RowMargin = 5;
        const int AxLeft = 200;
        private void RefreshView()
        {
            MainCanvas.Children.Clear();

            int totalRows = MyProj.Rows.Count(x => x.RowNumber != -1);
            int rowHeight = (AxBottom - AxTop) / totalRows - RowMargin;

            foreach (var row in MyProj.Rows.Where(x=>x.RowNumber!=-1))
            {
                int rowTop = AxTop + rowHeight * row.RowNumber;
                TextBlock tb = new TextBlock();
                tb.Text = row.CountryName;
                tb.Foreground = new SolidColorBrush(row.TextColor);
                tb.Height = rowHeight;
                Canvas.SetRight(tb, AxLeft);
                Canvas.SetTop(tb, rowTop);
                MainCanvas.Children.Add(tb);

                Line line = new Line();
                line.Stroke = new SolidColorBrush(row.LineColor);
                line.StrokeThickness = rowHeight;
                line.X1 = AxLeft;
                line.X1 = 600;
                line.Y1 = rowTop;
                line.Y2 = rowTop + rowHeight;
                MainCanvas.Children.Add(line);
            }

            cbEdit.Items.Clear();
            foreach (var rtc in MyProj.RichTexts)
            {
                cbEdit.Items.Add(rtc.Title);
                RichTextBox one = (RichTextBox)XamlReader.Load(Helper.GenerateStreamFromString(rtc.xamlStr));
                MainCanvas.Children.Add(one);
            }
        }
    }
}
