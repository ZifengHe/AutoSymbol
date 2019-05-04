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
        public int CurrentHeaderIndex = 5;
        private void RefreshView()
        {
            MainCanvas.Children.Clear();

            int totalRows = MyProj.Rows.Count(x => x.RowNumber != -1);
            int rowHeight = (MyProj.AxBottom - MyProj.AxTop) / totalRows - MyProj.RowMargin;

            Dictionary<string, int> longestByGroup = MyProj.CalcLongestByGroup(CurrentHeaderIndex);

            foreach (var row in MyProj.Rows.Where(x=>x.RowNumber!=-1))
            {
                int rowTop = MyProj.AxTop + rowHeight * row.RowNumber;
                TextBlock tb = new TextBlock();
                tb.FontSize = rowHeight/2;
                if(MyProj.FontFamily!= null)
                {
                    tb.FontFamily = new FontFamily(MyProj.FontFamily);
                    tb.FontWeight = MyProj.FontWeight;
                    tb.FontStyle = MyProj.FontStyle;
                }
                tb.Text = row.CountryName;
                tb.Foreground = new SolidColorBrush(row.TextColor);
                tb.Height = rowHeight;
                Canvas.SetRight(tb, MyProj.AxLeft+5);
                Canvas.SetTop(tb, rowTop - tb.FontSize );
                MainCanvas.Children.Add(tb);

                Line line = new Line();
                line.Stroke = new SolidColorBrush(row.LineColor);
                line.StrokeThickness = rowHeight/3*2;
                line.X1 = MyProj.CanvasWidth - MyProj.AxLeft;
                line.X2 = 600;
                line.Y1 = rowTop;
                line.Y2 = line.Y1;
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
