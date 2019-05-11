using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FrameByFrame
{
    public partial class MainWindow : Window
    {
        public string ImgRoot = @"file://C:\Users\zifengh\source\repos\ZifengHe\AutoSymbol\FrameByFrame\Flags\";
        public int CurrentHeaderIndex = 5;
        public int MaxInterpolation = 2;
        public int CurrentInterpolationIndex = 0;
        public Dictionary<string, Image> ImgDict = new Dictionary<string, Image>();
        private void RefreshView()
        {
            MainCanvas.Children.Clear();

            int totalRows = MyProj.Rows.Count(x => x.RowNumber != -1);
            int rowHeight = (MyProj.AxBottom - MyProj.AxTop) / totalRows - MyProj.RowMargin;
            int axToAx = MyProj.AxLeft - MyProj.AxRight;

            Dictionary<string, double> longestByGroup = MyProj.CalcLongestByGroup(CurrentHeaderIndex);

            foreach (var row in MyProj.Rows.Where(x => x.RowNumber != -1))
            {
                /// Country name section
                int rowTop = MyProj.AxTop + rowHeight * row.RowNumber;
                TextBlock tb = new TextBlock();
                tb.FontSize = rowHeight / 2;
                if (MyProj.FontFamily != null)
                {
                    tb.FontFamily = new FontFamily(MyProj.FontFamily);
                    tb.FontWeight = MyProj.FontWeight;
                    tb.FontStyle = MyProj.FontStyle;
                }
                tb.Text = row.CountryName;
                tb.Foreground = new SolidColorBrush(row.TextColor);
                tb.Height = rowHeight;
                Canvas.SetRight(tb, MyProj.AxLeft + 5);
                Canvas.SetTop(tb, rowTop - tb.FontSize);
                MainCanvas.Children.Add(tb);

                /// Horizontal bar section
                Line line = new Line();
                line.Stroke = new SolidColorBrush(row.LineColor);
                line.StrokeThickness = rowHeight / 3 * 2;
                line.X1 = MyProj.CanvasWidth - MyProj.AxLeft;
                line.Y1 = rowTop;
                line.Y2 = line.Y1;

                double rawData = 0;
                double.TryParse(row.Data[MyProj.Header[CurrentHeaderIndex]], out rawData);

                double nextRawData = rawData;
                if(CurrentHeaderIndex < MyProj.Header.Length -1)
                    double.TryParse(row.Data[MyProj.Header[CurrentHeaderIndex+1]], out nextRawData);


                line.X2 = MyProj.CanvasWidth - MyProj.AxLeft +
                    (int)(axToAx * (rawData +(nextRawData-rawData)* CurrentInterpolationIndex/MaxInterpolation) / longestByGroup[row.SeriesCode]);
                MainCanvas.Children.Add(line);

                /// Load the flag section
                if (rawData > 0.1)
                {
                    string imgPath = ImgRoot + CountryDict[row.CountryCode].ShortCode + ".png";
                    string key = row.RowNumber.ToString() + imgPath;

                    if (ImgDict.ContainsKey(key) == false)
                    {
                        ImgDict[key] = new Image
                        {
                            Width = 60,
                            Height = rowHeight,
                            Name = CountryDict[row.CountryCode].ShortCode,
                            Source = new BitmapImage(new Uri(imgPath)),
                        };
                    }

                    MainCanvas.Children.Add(ImgDict[key]);
                    Canvas.SetTop(ImgDict[key], rowTop);
                    Canvas.SetRight(ImgDict[key], MyProj.AxLeft - (line.X2 - line.X1) / 2);
                }

                /// Draw dotted line section
                Line leftDot = new Line();
                leftDot.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                leftDot.StrokeDashArray = new DoubleCollection() { 2, 2 };
                leftDot.X1 = MyProj.CanvasWidth - MyProj.AxLeft;
                leftDot.X2 = line.X1;
                leftDot.Y1 = 100;
                leftDot.Y2 = 700;
                MainCanvas.Children.Add(leftDot);
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
