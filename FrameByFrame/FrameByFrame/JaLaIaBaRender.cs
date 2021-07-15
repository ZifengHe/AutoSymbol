using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class JaLaIaBa : Window
    {
        public void RenderAll(int frameNum)
        {
            //var visual = new DrawingVisual();
            //var property = typeof(Canvas).GetProperty("VisualTextRenderingMode",
            //            BindingFlags.NonPublic | BindingFlags.Instance);

            //property.SetValue(visual, TextRenderingMode.Aliased);

            JLIBCanvas.Children.Clear();
            DrawMultiText();
            DrawMultiRow(frameNum);
            DrawYearMonthDay(frameNum);
            StatusTextBlock.Text = frameNum.ToString();
        }


        private void DrawMultiRow(int frameNum)
        {
            OneFrameData oneFrame = DataSeries.AllFrameData[frameNum];
            foreach (var pair in oneFrame.LengthByKey)
                DrawOneRowByKey(pair.Key, oneFrame.LengthByKey[pair.Key], oneFrame.HeightOrder[pair.Key], oneFrame);
        }

        private void DrawYearMonthDay(int frameNum)
        {
            OneFrameData one = DataSeries.AllFrameData[frameNum];
            TextBlock tb = new TextBlock();
            tb.Text = String.Format("{0}.{1}", one.TimeStamp.Year, one.TimeStamp.Month);
            tb.Height = double.Parse(Setting.All["TimeHeight"]);
            tb.FontSize = int.Parse(Setting.All["TimeFontSize"]);
            Canvas.SetLeft(tb, double.Parse(Setting.All["TimeLeftX"]));
            Canvas.SetTop(tb, double.Parse(Setting.All["TimeTopY"]));
            JLIBCanvas.Children.Add(tb);
        }

        private void DrawOneRowByKey(string rowKey, double length, double heightOrder, OneFrameData oneFrame)
        {
            double fromTop = heightOrder * (Setting.RowHeight + double.Parse(Setting.All["RowMargin"])) + double.Parse(Setting.All["RowTopY"]);
            TextBlock tb = new TextBlock();
            tb.Background = Mapper.Brush[Setting.All[rowKey.ToUpper() + "Color"]];
            tb.Width = Setting.RowMaxLength * length / oneFrame.TopLength;
            tb.Height = Setting.RowHeight;
            tb.Text = DataSeries.AllDisplayName[rowKey];
            tb.TextAlignment = TextAlignment.Right;
            tb.Padding = new Thickness(0, double.Parse(Setting.All["RowTopTextPadding"]), 5, 0);
            tb.FontSize = tb.Height * 0.4;
            tb.Foreground = Mapper.Brush["White"];
            Canvas.SetLeft(tb, Setting.RowLeftX);
            Canvas.SetTop(tb, fromTop);
            JLIBCanvas.Children.Add(tb);
            DrawOneRowImageByKey(rowKey, fromTop);
            DrawOneRowNumber(length, Setting.RowLeftX + tb.Width + 5, fromTop);
        }

        private void DrawOneRowNumber(double length, double fromLeft, double fromTop)
        {
            TextBlock tb = new TextBlock();
            tb.Text = String.Format("{0:0.00}", length);
            tb.Height = Setting.RowHeight;
            tb.FontSize = tb.Height * 0.4;
            tb.Padding = new Thickness(0, double.Parse(Setting.All["RowTopTextPadding"]), 0, 0);
            Canvas.SetLeft(tb, fromLeft);
            Canvas.SetTop(tb, fromTop);
            JLIBCanvas.Children.Add(tb);
        }

        private void DrawOneRowImageByKey(string rowKey, double fromTop)
        {
            double height = Setting.RowHeight;
            double width = double.Parse(Setting.All["RowImageWidth"]);
            BitmapImage original = DataSeries.AllImages[rowKey];

            var after = new TransformedBitmap(original,
                new ScaleTransform(width / original.PixelWidth, height / original.PixelHeight));

            Image img = new Image
            {
                Height = height,
                Width = width,
                Name = rowKey,
                Source = after
            };

            Canvas.SetLeft(img, double.Parse(Setting.All["RowImageFromLeft"]));
            Canvas.SetTop(img, fromTop);
            JLIBCanvas.Children.Add(img);
        }
        private void DrawMultiText()
        {
            for (int i = 1; i < 10; i++)
            {
                string head = "Text" + i.ToString() + "-";
                if (Setting.All.ContainsKey(head + "Text"))
                {
                    TextBlock one = new TextBlock();
                    one.TextWrapping = TextWrapping.Wrap;
                    one.Text = Setting.All[head + "Text"];
                    one.Width = double.Parse(Setting.All[head + "Width"]);
                    one.Foreground = Mapper.Brush[Setting.All[head + "Foreground"]];
                    one.FontSize = double.Parse(Setting.All[head + "FontSize"]);
                    one.FontFamily = new FontFamily(Setting.All[head + "FontFamily"]);
                    one.FontWeight = FontWeight.FromOpenTypeWeight(int.Parse(Setting.All[head + "FontWeight"]));
                    one.SetValue(TextBlock.FontStyleProperty, FontStyles.Italic);
                    one.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);

                    Canvas.SetLeft(one, double.Parse(Setting.All[head + "Canvas.Left"]));
                    Canvas.SetTop(one, double.Parse(Setting.All[head + "Canvas.Top"]));
                    JLIBCanvas.Children.Add(one);
                }
            }
        }
    }

}