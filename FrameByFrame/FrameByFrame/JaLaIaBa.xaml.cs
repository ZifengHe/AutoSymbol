using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.Windows.Controls.Primitives;
using Xceed.Wpf.Toolkit;
using RichTextBox = Xceed.Wpf.Toolkit.RichTextBox;
using Path = System.IO.Path;
using System.Xml.Serialization;
using System.Windows.Markup;
using System.Windows.Forms;
//using AForge.Video.FFMPEG;
using Accord;
using Accord.Video;
using System.Drawing;
using System.Diagnostics;
using Accord.Video.FFMPEG;
using System.Runtime;
using System.Reflection;
using System.Windows.Threading;
using M = System.Windows.MessageBox;

namespace FrameByFrame
{
    /// <summary>
    /// Interaction logic for JaLaIaBa.xaml
    /// </summary>
    public partial class JaLaIaBa : Window
    {
        DispatcherTimer dt;
        int CurrentFrameNum;
        public JaLaIaBa()
        {
            InitializeComponent();
        }

        private void LoadSettingClicked(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(SettingFolder.Text) == false)
            {
                M.Show("Setting Folder empty!");
                return;
            }

            string fileName = Path.Combine(SettingFolder.Text, "setting.txt");
            SettingContent.Text = File.ReadAllText(fileName);

            Setting.All.Clear();
            Setting.All["Folder"] = SettingFolder.Text;
            foreach (var one in File.ReadAllLines(fileName))
            {
                string[] words = one.Split(':');
                if(words  != null && words.Length==2)
                    Setting.All[words[0]] = words[1];
            }
            Setting.CSVFile = Path.Combine(SettingFolder.Text, Setting.All["CSVFile"]);
            Setting.VideoFile = Path.Combine(SettingFolder.Text, "Result.avi");
            Setting.ImgFolder = Path.Combine(SettingFolder.Text, "Images");
            Setting.RowStrokeThickness = double.Parse(Setting.All["RowStrokeThickness"]);
            Setting.RowMaxLength = double.Parse(Setting.All["RowMaxLength"]);
            Setting.RowHeight = double.Parse(Setting.All["RowHeight"]);
            Setting.RowTopY = double.Parse(Setting.All["RowTopY"]);
            Setting.RowLeftX = double.Parse(Setting.All["RowLeftX"]);

            if (File.Exists(Setting.CSVFile) == false)
                M.Show("CSV file does not exist.");

            DataSeries.LoadData();
            this.RenderAll(0);
        }

        private void FirstFrameClicked(object sender, RoutedEventArgs e)
        {
            this.RenderAll(0);
        }

        private void LastFrameClicked(object sender, RoutedEventArgs e)
        {
            this.RenderAll(DataSeries.AllFrameData.Count - 1);
        }

        private void PlayClicked(object sender, RoutedEventArgs e)
        {
            CurrentFrameNum = 0;
            dt = new DispatcherTimer();
            dt.Interval = new TimeSpan(0, 0, 0, 0, 66);
            dt.Tick += PlayTick;
            dt.Start();
        }

        private void PlayTick(object sender, EventArgs e)
        {
            if(CurrentFrameNum < DataSeries.AllFrameData.Count)
            {
                this.RenderAll(CurrentFrameNum);
                CurrentFrameNum++;
            }
            else
            {
                dt.Stop();
            }
        }

        private void RecordClicked(object sender, RoutedEventArgs e)
        {
            GC.Collect();
            int width = (int)JLIBCanvas.Width;
            int height = (int)JLIBCanvas.Height;
           

            VideoFileWriter writer = new VideoFileWriter();
            writer.Open(Setting.VideoFile, width, height, 15, VideoCodec.H264,2000000);

            
            MemoryStream ms = new MemoryStream();
            var encoder = new BmpBitmapEncoder();
            PresentationSource source = PresentationSource.FromVisual(JLIBCanvas);

            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)JLIBCanvas.ActualWidth, (int)JLIBCanvas.ActualHeight,
                64* source.CompositionTarget.TransformToDevice.M11,
                64* source.CompositionTarget.TransformToDevice.M22,
                PixelFormats.Pbgra32);

            
            for (int i = 0; i <= DataSeries.AllFrameData.Count() - 1; i++)
            {
                Trace.TraceInformation("{0} of {1}", i, DataSeries.AllFrameData.Count());
                StatusTextBlock.Text = string.Format("{0} of {1}", i, DataSeries.AllFrameData.Count());
                GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                GC.Collect();
                this.RenderAll(i);
                JLIBCanvas.LayoutTransform = null;
                System.Windows.Size size = new System.Windows.Size(JLIBCanvas.ActualWidth, JLIBCanvas.ActualHeight);
                JLIBCanvas.Measure(size);               
                JLIBCanvas.Arrange(new Rect(size));

                ms.Seek(0, System.IO.SeekOrigin.Begin);
                ms.SetLength(0);

                bitmap.Render(JLIBCanvas);
                encoder = new BmpBitmapEncoder();
                BitmapFrame frame = BitmapFrame.Create(bitmap);
                encoder.Frames.Add(frame);
                encoder.Save(ms);

                // Helper.CreateBmpStream(MainCanvas, ms);
                using (Bitmap b = new Bitmap(ms))
                {
                    writer.WriteVideoFrame(b);
                }
            }

            writer.Close();
            
        }
    }
}
