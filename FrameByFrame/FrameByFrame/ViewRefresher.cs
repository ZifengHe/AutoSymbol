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
            int axToAx = MyProj.AxLeft - MyProj.AxRight;

            Dictionary<string, double> longestByGroup = MyProj.CalcLongestByGroup(CurrentHeaderIndex);

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
                line.Y1 = rowTop;
                line.Y2 = line.Y1;

                double rawData = 0;
                double.TryParse(row.Data[MyProj.Header[CurrentHeaderIndex]], out rawData);

                line.X2 = MyProj.CanvasWidth - MyProj.AxLeft +
                    (int)(axToAx * rawData / longestByGroup[row.SeriesCode]);
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
