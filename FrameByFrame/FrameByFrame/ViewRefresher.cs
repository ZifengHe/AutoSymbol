using System;
using System.Collections.Generic;
using System.Globalization;
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
            Dictionary<string, double> longestByGroup = MyProj.CalcLongestByGroup(CurrentHeaderIndex);

            MainCanvas.Children.Clear();
            MainCanvas.Height = MyProj.CanvasHeight;
            MainCanvas.Width = MyProj.CanvasWidth;

            int totalRows = MyProj.Rows.Count(x => x.RowNumber != -1);
            int rowHeight = (MyProj.AxBottom - MyProj.AxTop) / (totalRows + longestByGroup.Count) - MyProj.RowMargin;
            int axToAx = MyProj.AxLeft - MyProj.AxRight;
            int rowTop = MyProj.AxTop;

            foreach (var g in longestByGroup.Keys)
            {
                /// Draw group header
                DrawGroupHeader(rowTop, rowHeight, MyProj.Rows.Where(x=>x.SeriesCode==g).First().SeriesName);

                /// Draw parking lines
                double maxParking = DrawGroupParkingLine(rowHeight, longestByGroup, g);

                rowTop += rowHeight;

                foreach (var row in MyProj.Rows.Where(x => x.RowNumber != -1))
                {
                    if (row.SeriesCode == g)
                    {
                        /// Country name section
                       DrawRowCountryName(rowTop, rowHeight, row);

                        /// Horizontal bar section
                        Line line;
                        double rawData;
                        DrawRowLineAndNumber(rowHeight, axToAx, maxParking, row, rowTop, out line, out rawData);

                        /// Load the flag section
                        DrawRowFlag(rowHeight, row, rowTop, rawData);

                        rowTop += rowHeight;
                    }                   
                }
            }

            DrawBothAxis();

            cbEdit.Items.Clear();
            foreach (var rtc in MyProj.RichTexts)
            {
                cbEdit.Items.Add(rtc.Title);
                RichTextBox one = (RichTextBox)XamlReader.Load(Helper.GenerateStreamFromString(rtc.xamlStr));
                MainCanvas.Children.Add(one);
            }
        }

        private void DrawBothAxis()
        {
            /// Draw Axis line section
            Line leftDot = new Line();
            leftDot.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            leftDot.SnapsToDevicePixels = true;
            leftDot.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            leftDot.StrokeDashArray = new DoubleCollection() { 2, 2 };
            leftDot.X1 = MyProj.CanvasWidth - MyProj.AxLeft;
            leftDot.X2 = leftDot.X1;
            leftDot.Y1 = 100;
            leftDot.Y2 = 700;
            MainCanvas.Children.Add(leftDot);

            Line rightDot = new Line();
            rightDot.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            rightDot.SnapsToDevicePixels = true;
            rightDot.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            rightDot.StrokeDashArray = new DoubleCollection() { 2, 2 };

            rightDot.X1 = MyProj.CanvasWidth - MyProj.AxRight;
            rightDot.X2 = MyProj.CanvasWidth - MyProj.AxRight;
            rightDot.Y1 = 100;
            rightDot.Y2 = 700;
            MainCanvas.Children.Add(rightDot);
        }

        private void DrawRowFlag(int rowHeight, OneRow row, int rowTop, double rawData)
        {
            if (rawData > 0.1)
            {
                string imgPath = ImgRoot + CountryDict[row.CountryCode].ShortCode + ".png";
                string key = row.RowNumber.ToString() + imgPath;

                if (ImgDict.ContainsKey(key) == false)
                {
                    ImgDict[key] = new Image
                    {
                        Height = rowHeight / 3 * 2,
                        Width = 550 * Height / 367,
                        Name = CountryDict[row.CountryCode].ShortCode,
                        Source = new BitmapImage(new Uri(imgPath)),
                    };
                }

                Canvas.SetLeft(ImgDict[key], MyProj.CanvasWidth - MyProj.AxRight - 320);
                Canvas.SetTop(ImgDict[key], rowTop - ImgDict[key].Height / 2);
                MainCanvas.Children.Add(ImgDict[key]);
            }
        }

        private void DrawRowLineAndNumber(int rowHeight, int axToAx, double maxParking, OneRow row, int rowTop, out Line line, out double rawData)
        {
            line = new Line();
            line.Stroke = new SolidColorBrush(row.LineColor);
            line.SnapsToDevicePixels = true;
            line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            line.StrokeThickness = rowHeight / 3 * 2;
            line.X1 = MyProj.CanvasWidth - MyProj.AxLeft;
            line.Y1 = rowTop;
            line.Y2 = line.Y1;

            rawData = 0;
            double.TryParse(row.Data[MyProj.Header[CurrentHeaderIndex]], out rawData);

            double nextRawData = rawData;
            if (CurrentHeaderIndex < MyProj.Header.Length - 1)
                double.TryParse(row.Data[MyProj.Header[CurrentHeaderIndex + 1]], out nextRawData);

            line.X2 = MyProj.CanvasWidth - MyProj.AxLeft +
                (int)(axToAx * (rawData + (nextRawData - rawData) * CurrentInterpolationIndex / MaxInterpolation) / maxParking);
            MainCanvas.Children.Add(line);

            /// Numbers on the bar
            TextBlock numText = new TextBlock();
            numText.FontSize = rowHeight / 2;
            numText.Text = ConvertDouble(rawData);
            numText.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            numText.Height = rowHeight;
            Canvas.SetRight(numText, MyProj.CanvasWidth - line.X2 + 5);
            Canvas.SetTop(numText, rowTop - numText.FontSize);
            MainCanvas.Children.Add(numText);
        }

        private void DrawRowCountryName(int rowTop, int rowHeight, OneRow row)
        {
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
        }

        private void DrawGroupHeader(int rowTop, int rowHeight, string content)
        {
        }

        private double DrawGroupParkingLine(int rowHeight, Dictionary<string, double> longestByGroup, string g)
        {
            double maxParking = FindBestParkingNumber(longestByGroup[g]);
            for (int i = 0; i < 5; i++)
            {
                double current = maxParking / 5 * (i + 1);
                string curStr = ConvertDouble(current);

                Line parkingLine = new Line();
                parkingLine.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                parkingLine.SnapsToDevicePixels = true;
                parkingLine.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
                parkingLine.StrokeDashArray = new DoubleCollection() { 2, 2 };
                parkingLine.X1 = MyProj.CanvasWidth - MyProj.AxLeft + (MyProj.AxLeft - MyProj.AxRight) * (i + 1) / 5;
                parkingLine.X2 = parkingLine.X1;
                parkingLine.Y1 = 100;
                parkingLine.Y2 = 700;
                MainCanvas.Children.Add(parkingLine);

                TextBlock tbParking = new TextBlock();
                tbParking.FontSize = rowHeight / 2;
                tbParking.Text = curStr;
                tbParking.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                tbParking.Height = rowHeight;
                Canvas.SetRight(tbParking, parkingLine.X2);
                Canvas.SetTop(tbParking, 710);
                MainCanvas.Children.Add(tbParking);
            }

            return maxParking;
        }

        public string ConvertDouble(double num)
        {
            if (num > 999999999 || num < -999999999)
            {
                return num.ToString("0,,,.##B", CultureInfo.InvariantCulture);
            }
            else   if (num > 999999 || num < -999999)
            {
                return num.ToString("0,,.##M", CultureInfo.InvariantCulture);
            }
            else    if (num > 999 || num < -999)
            {
                return num.ToString("0,.##K", CultureInfo.InvariantCulture);
            }
            else
            {
                return num.ToString(CultureInfo.InvariantCulture);
            }
        }

        public double FindBestParkingNumber(double num)
        {
            double parking = 0.1;

            for(int i=0; i <40;i++)
            {
                if (num < parking * 1.2)
                    return parking * 1.2;
                if (num < parking * 1.5)
                    return parking * 1.5;
                if (num < parking * 1.8)
                    return parking * 1.8;
                if (num < parking * 2)
                    return parking * 2;
                if (num < parking * 2.5)
                    return parking * 2.5;
                if (num < parking * 3)
                    return parking * 3;
                if (num < parking * 3.5)
                    return parking * 3.5;
                if (num < parking * 4)
                    return parking * 4;
                if (num < parking * 5)
                    return parking * 5;
                if (num < parking * 6)
                    return parking * 6;
                if (num < parking * 7)
                    return parking * 7;
                if (num < parking * 8)
                    return parking * 8;
                if (num < parking * 9)
                    return parking * 9;
                if (num < parking * 10)
                    return parking * 10;
                parking = parking * 10;

            }

            return parking;
        }
    }
}
