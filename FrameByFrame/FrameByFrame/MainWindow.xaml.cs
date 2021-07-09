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

namespace FrameByFrame
{
    using MB = System.Windows.MessageBox;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer dt;
        private static Random Rand = new Random();
        private string RootFolder;
        private Dictionary<string, CountryCodeMapping> CountryDict = new Dictionary<string, CountryCodeMapping>();
        //private Dictionary<RichTextBox, string> rtcMapping = new Dictionary<RichTextBox, string>();
        ProjData MyProj = new ProjData();
        string CurrentProjFileName;
        bool loadEveryRowInCombobox = true;
        ProjData RawData;
        Dictionary<string, Dictionary<string, OneRow>> filtered = new Dictionary<string, Dictionary<string, OneRow>>();
        Dictionary<string, System.Windows.Controls.TextBox> settingDict = new Dictionary<string, System.Windows.Controls.TextBox>();
        int CurrentSlowTick = 0;
        public MainWindow()
        {
            InitializeComponent();
            FindRootFolder();
            LoadCountryCode();
            CurrentProjFileName = Path.Combine(RootFolder, @"FrameByFrame\Proj\Current.xml");
            ProjData.TempFile = Path.Combine(RootFolder, @"FrameByFrame\Proj\temp.csv");
            FlagRoot = @"file://" + RootFolder + @"\FrameByFrame\Flags\";

        }

        void DisplaySetting()
        {
            SettingBox.Children.Clear();
            FieldInfo[] fis = typeof(ProjSetting).GetFields();
            foreach (var fi in fis)
            {
                StackPanel sp = new StackPanel();
                sp.Orientation = System.Windows.Controls.Orientation.Horizontal;
                SettingBox.Children.Add(sp);
                TextBlock tb = new TextBlock();
                tb.Text = fi.Name;
                System.Windows.Controls.TextBox tbox = new System.Windows.Controls.TextBox();
                sp.Children.Add(tb);
                sp.Children.Add(tbox);
                string content = (string)fi.GetValue(MyProj.Setting).ToString();
                tbox.Text = content;
                settingDict[fi.Name] = tbox;
            }

            //Type type = typeof(Job);
            //Job job_out = new Job();
            //FieldInfo[] fields = type.GetFields();

            //// iterate through all fields of Job class
            //for (int i = 0; i < fields.Length; i++)
            //{
            //    List<string> filterslist = new List<string>();
            //    string filters = (string)fields[i].GetValue(job_filters);

            //    // if job_filters contaisn magic word "$" for the field, then do something with a field, otherwise just copy it to job_out object
            //    if (filters.Contains("$"))
            //    {
            //        filters = filters.Substring(filters.IndexOf("$") + 1, filters.Length - filters.IndexOf("$") - 1);
            //        // MessageBox.Show(filters);
            //        // do sothing useful...
            //    }
            //    else
            //    {
            //        // this is my current field value 
            //        var str_value = fields[i].GetValue(job_in);
            //        // this is my current filed name
            //        var field_name = fields[i].Name;

            //        //  I got stuck here :(
            //        // I need to save (copy) data "str_value" from job_in.field_name to job_out.field_name
            //        // HELP!!!

            //    }
        }

        void FindRootFolder()
        {
            if (Directory.Exists(@"C:\Users\zifengh\Source\Repos\ZifengHe\AutoSymbol"))
                RootFolder = @"C:\Users\zifengh\Source\Repos\ZifengHe\AutoSymbol";
            if (Directory.Exists(@"C:\Users\Zifeng\source\repos\ZifengHe\AutoSymbol"))
                RootFolder = @"C:\Users\Zifeng\source\repos\ZifengHe\AutoSymbol";
        }

        private void ProcessOneFrame(int index)
        {
            char[] sep = new char[] { ' ' };
            //string timeStr = Header[index].Split(sep)[0];

        }

        private void CSVClicked(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = OpenFile("*.csv", "CSV Files (*.csv)|*.csv");

            if (dlg != null)
            {
                LoadCsvToMyProj(dlg.FileName);
                System.Windows.MessageBox.Show("CSV Loaded");
            }
            System.Windows.MessageBox.Show("FileName Empty");
        }

        private void LoadCsvToMyProj(string fileName)
        {
            MyProj = new ProjData();
            MyProj.CSVContent = File.ReadAllText(fileName);
            MyProj.ProcessHorizontalFormatCSVFile();

            if (loadEveryRowInCombobox)
            {
                cbConfig.Items.Clear();
                foreach (var one in MyProj.Rows)
                {
                    cbConfig.Items.Add(one.Id());
                }
            }
            loadEveryRowInCombobox = true;
        }

        private void RowLineColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            OneRow row = MyProj.GetRow((string)cbConfig.SelectedValue);
            row.LineColor = RowLineColor.SelectedColor.Value;
            System.Windows.Clipboard.SetText(string.Format("All[\"{0}\"] = Color.FromRgb({1},{2},{3});", CountryDict[row.CountryCode].ShortCode,
                row.LineColor.R,
                row.LineColor.G,
                row.LineColor.B)); RefreshView();
        }

        private void RowTextColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            OneRow row = MyProj.GetRow((string)cbConfig.SelectedValue);
            row.TextColor = RowTextColor.SelectedColor.Value;
            System.Windows.Clipboard.SetText(string.Format("All[\"{0}\"] = Color.FromRgb({1},{2},{3});", CountryDict[row.CountryCode].ShortCode,
                row.TextColor.R,
                row.TextColor.G,
                row.TextColor.B));
            RefreshView();
        }


        private void AutoColorClicked(object sender, RoutedEventArgs e)
        {
            foreach (var row in MyProj.Rows)
            {
                string shortCode = CountryDict[row.CountryCode].ShortCode;

                if (ColorByEntity.All.ContainsKey(shortCode))
                {
                    row.TextColor = ColorByEntity.All[shortCode];
                    row.LineColor = ColorByEntity.All[shortCode];
                }
            }
            RefreshView();
        }

        private void RowChanged(object sender, SelectionChangedEventArgs e)
        {
        }


        private void AddRtbClicked(object sender, RoutedEventArgs e)
        {
            RichTextConfig rtc = new RichTextConfig();
            EditRichTextBox(rtc);
            MyProj.RichTexts.Add(rtc);
            RefreshView();
        }

        private void EditRichTextBox(RichTextConfig rtc)
        {
            RTBHost host = new RTBHost();
            host.txtTitle.Text = rtc.Title;
            host.MainItem.Children.Clear();
            host.MainItem.Children.Add((RichTextBox)XamlReader.Load(Helper.GenerateStreamFromString(rtc.xamlStr)));
            host.ShowDialog();
            rtc.Title = host.txtTitle.Text;
            rtc.xamlStr = XamlWriter.Save(VisualTreeHelper.GetChild(host.MainItem, 0));
        }

        private void EditItemClicked(object sender, RoutedEventArgs e)
        {
            EditRichTextBox(MyProj.GetRTC((string)cbEdit.SelectedValue));
            RefreshView();
        }

        private void DeleteItemClicked(object sender, RoutedEventArgs e)
        {
            MyProj.RichTexts.Remove(MyProj.GetRTC((string)cbEdit.SelectedValue));
            RefreshView();
        }

        private void BackGroundClicked(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = OpenFile(
                "*.png",
                "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif");

            ImageBrush myBrush = new ImageBrush();
            myBrush.ImageSource = new BitmapImage(new Uri(@"file://" + dlg.FileName));
            MainCanvas.Background = myBrush;
        }

        private void LoadCountryCode()
        {
            string fileName = RootFolder + @"\FrameByFrame\Master\CountryCode.csv";
            using (TextFieldParser parser = new TextFieldParser(fileName))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                bool firstLine = true;

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    if (firstLine)
                    {
                        firstLine = false;
                        continue;
                    }
                    CountryCodeMapping m = new CountryCodeMapping();
                    m.Name = fields[0];
                    m.ShortCode = fields[1];
                    m.LongCode = fields[2];
                    CountryDict[m.LongCode] = m;
                }
            }
        }

        private Microsoft.Win32.OpenFileDialog OpenFile(string ext, string filter)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ext;
            dlg.Filter = filter;
            Nullable<bool> result = dlg.ShowDialog();
            if (result != true)
                return null;
            return dlg;
        }

        private Microsoft.Win32.SaveFileDialog SaveXmlFile()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Name"; // Default file name
            dlg.DefaultExt = ".xml"; // Default file extension
            dlg.Filter = "Xml documents (.xml)|*.xml"; // Filter files by extension

            // Show save file dialog box
            dlg.ShowDialog();
            return dlg;
        }

        private void RowNumberChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)cbRowNumber.SelectedItem;
            MyProj.GetRow((string)cbConfig.SelectedValue).RowNumber = int.Parse((string)(item.Content));
            RefreshView();
        }


        //public void CavasToData()
        //{
        //    foreach (FrameworkElement fe in MainCanvas.Children)
        //    {
        //        if (fe is RichTextBox)
        //        {
        //            RichTextBox item = (RichTextBox)fe;
        //            //RichTextConfig one = RichTextConfig.RTCDict[item.Text];
        //            RichTextConfig one = MyProj.GetRTC(rtcMapping[item]);

        //            //one.Width = item.Width;
        //            //one.Height = item.Height;
        //            // one.RenderTransform = item.RenderTransform;
        //            //one.RenderTransformOrigin = item.RenderTransformOrigin;
        //            one.xamlStr = XamlWriter.Save(item);
        //        }
        //    }
        //}


        private void SaveProjectClicked(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = SaveXmlFile();
            CurrentProjFileName = dlg.FileName;
            SaveProjectByFileName(CurrentProjFileName);
        }

        private void SaveProjectByFileName(string fileName)
        {
            //CavasToData();
            ObjectManager.ToXmlFile<ProjData>(fileName, MyProj);
        }

        private void LoadProjClicked(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = OpenFile("*.xml", "XML Files (*.xml)|*.xml");
            CurrentProjFileName = dlg.FileName;
            LoadProjFile(dlg.FileName);
        }

        private void LoadProjFile(string fileName)
        {
            MyProj = ObjectManager.FromXml<ProjData>(fileName);
            MyProj.ProcessHorizontalFormatCSVFile();

            DisplaySetting();
            cbConfig.Items.Clear();
            foreach (var one in MyProj.Rows)
            {
                cbConfig.Items.Add(one.Id());
            }
            RefreshView();
        }

        private void FontClicked(object sender, RoutedEventArgs e)
        {
            FontDialog fd = new FontDialog();
            var result = fd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                MyProj.FontFamily = fd.Font.Name;
                MyProj.FontWeight = fd.Font.Bold ? FontWeights.Bold : FontWeights.Regular;
                MyProj.FontStyle = fd.Font.Italic ? FontStyles.Italic : FontStyles.Normal;
            }
        }

        private void RecordVideoClicked(object sender, RoutedEventArgs e)
        {
            GC.Collect();
            int width = (int)MainCanvas.Width;
            int height = (int)MainCanvas.Height;

            VideoFileWriter writer = null;
            MemoryStream ms = new MemoryStream();
            var encoder = new BmpBitmapEncoder();
            PresentationSource source = PresentationSource.FromVisual(MainCanvas);

            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)MainCanvas.ActualWidth, (int)MainCanvas.ActualHeight,
                96 * source.CompositionTarget.TransformToDevice.M11,
                96 * source.CompositionTarget.TransformToDevice.M22,
                PixelFormats.Pbgra32);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            int counter = 0;
            for (int i = CalcStartHeaderIndex(); i <= MyProj.Header.Length - 1; i++)
            {
                Trace.TraceInformation("{0} of {1}", i, MyProj.Header.Length);

                for (int j = 0; j < MaxInterpolation; j++)
                {
                    if (((i - 5) * MaxInterpolation + j) % 10000 == 0) // Max frame per file in case too big
                    {
                        counter++;
                        if (writer != null)
                        {
                            writer.Close();
                        }
                        writer = new VideoFileWriter();
                        string fileName = @"c:\tempvideo\Result-" + counter.ToString() + ".avi";
                        writer.Open(fileName, width, height, 15, VideoCodec.H264);
                    }

                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    CurrentHeaderIndex = i;
                    CurrentInterpolationIndex = j;
                    RefreshView();
                    Transform transform = MainCanvas.LayoutTransform;
                    MainCanvas.LayoutTransform = null;
                    System.Windows.Size size = new System.Windows.Size(MainCanvas.ActualWidth, MainCanvas.ActualHeight);
                    MainCanvas.Measure(size);
                    MainCanvas.Arrange(new Rect(size));

                    ms.Seek(0, System.IO.SeekOrigin.Begin);
                    ms.SetLength(0);

                    bitmap.Render(MainCanvas);
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
            }

            writer.Close();
            System.Windows.MessageBox.Show("Complete in " + sw.Elapsed.TotalSeconds + " Seconds");
        }

        private void WDIClicked(object sender, RoutedEventArgs e)
        {
            RawData = new ProjData();
            string wdiFile = Path.Combine(RootFolder, @"FrameByFrame\csv\download\wdi.csv");
            RawData.CSVContent = File.ReadAllText(wdiFile);
            RawData.ProcessHorizontalFormatCSVFile();

            cbCountry.Items.Clear();
            cbCountryGroup.Items.Clear();
            foreach (var one in RawData.Rows.GroupBy(x => x.CountryName))
            {
                cbCountry.Items.Add(one.Key);
                cbCountryGroup.Items.Add(one.Key);
            }

            System.Windows.MessageBox.Show("wdi file loaded");
        }

        private void RunFilterClicked(object sender, RoutedEventArgs e)
        {
            if (cbCountry.SelectedItems.Count != 1 || cbCountryGroup.SelectedItems.Count == 0)
            {
                MB.Show("Filter must work on single focused country, and several other in the group");
                return;
            }

            /// Start year 1996 end year 2016
            int index1996 = -1;
            int index2016 = -1;
            string focusCountry = cbCountry.SelectedValue;

            for (int i = 0; i < RawData.Header.Length; i++)
            {
                if (RawData.Header[i].Contains("1996"))
                    index1996 = i;
                if (RawData.Header[i].Contains("2016"))
                    index2016 = i;
            }


            filtered.Clear();


            foreach (var one in RawData.Rows)
            {
                bool ignore = true;
                if (one.CountryName == cbCountry.SelectedValue)
                    ignore = false;
                foreach (var c in cbCountryGroup.SelectedItems)
                    if (c.ToString() == one.CountryName)
                        ignore = false;

                if (ignore)
                    continue;

                if (one.Data.ContainsKey(RawData.Header[index1996]) == false)
                    continue;
                if (one.Data.ContainsKey(RawData.Header[index2016]) == false)
                    continue;

                double d1996;
                double d2016;

                if (double.TryParse(one.Data[RawData.Header[index1996]], out d1996) == false)
                    continue;
                if (double.TryParse(one.Data[RawData.Header[index2016]], out d2016) == false)
                    continue;
                if (d1996 == 0)
                    continue;

                one.Magic = (d2016 - d1996) / d1996;

                if (filtered.ContainsKey(one.SeriesName) == false)
                    filtered.Add(one.SeriesName, new Dictionary<string, OneRow>());

                if (filtered[one.SeriesName].ContainsKey(one.CountryName) == false)
                    filtered[one.SeriesName][one.CountryName] = one;
            }

            cbIndicator.Items.Clear();
            List<string> toRemove = new List<string>();
            foreach (var one in filtered)
            {
                if (one.Value.ContainsKey(focusCountry) == false)
                    continue;
                if (one.Value[focusCountry].Magic == 0)
                    continue;

                if (one.Value.Where(x => x.Value.Magic != 0).Max(x => x.Value.Magic) == one.Value[focusCountry].Magic
                    || one.Value.Where(x => x.Value.Magic != 0).Min(x => x.Value.Magic) == one.Value[focusCountry].Magic)
                {
                    cbIndicator.Items.Add(one.Key);
                    Trace.TraceInformation(one.Key);
                    foreach (var detail in one.Value)
                    {
                        Trace.TraceInformation("{0} ratio={1} From {2} in 1996 to {3} in 2016 ",
                            detail.Key,
                            detail.Value.Magic,
                            double.Parse(detail.Value.Data[RawData.Header[index1996]]),
                            double.Parse(detail.Value.Data[RawData.Header[index2016]]));
                    }
                }
                else
                {
                    toRemove.Add(one.Key);
                }
            }
            foreach (var key in toRemove)
                filtered.Remove(key);
        }

        private void GenerateProj(object sender, RoutedEventArgs e)
        {
            /// 1. read raw csv and delete unintended line, use series name
            /// 2. Auto save project 
            /// 
            string wdiFile = Path.Combine(RootFolder, @"FrameByFrame\csv\download\wdi.csv");
            List<string> result = new List<string>();
            string[] rawLines = File.ReadAllLines(wdiFile);
            result.Add(rawLines[0]);

            foreach (string line in rawLines)
            {
                bool keep = false;
                foreach (var indicator in cbIndicator.SelectedItems)
                {
                    string key = indicator.ToString();
                    if (line.Contains(key))
                    {
                        foreach (var item in filtered[key])
                        {
                            if (line.Contains(item.Key))
                            {
                                keep = true;
                            }
                        }
                    }
                }
                if (keep)
                    result.Add(line);
            }

            string csvName = string.Empty;
            foreach (var item in cbIndicator.SelectedItems)
                csvName += item.ToString() + " ";
            string csvFile = Path.Combine(RootFolder, @"FrameByFrame\csv\" + csvName + ".csv");
            File.WriteAllLines(csvFile, result.ToArray());

            LoadCsvToMyProj(csvFile);
            AutoColorClicked(null, null);

            string projFile = Path.Combine(RootFolder, @"FrameByFrame\proj\" + csvName + ".xml");
            SaveProjectByFileName(projFile);
            LoadProjFile(projFile);
        }

        private void IndicatorChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {
            cbCountry.Items.Clear();
            if (cbIndicator.SelectedItems.Count == 0)
                return;

            Dictionary<string, int> countryCount = new Dictionary<string, int>();
            foreach (var one in cbIndicator.SelectedItems)
            {
                foreach (var item in filtered[one.ToString()].Where(x => x.Value.Magic != 0))
                {
                    if (countryCount.ContainsKey(item.Key) == false)
                        countryCount[item.Key] = 0;
                    countryCount[item.Key]++;
                }
            }
            foreach (var one in countryCount)
            {
                if (one.Value == cbIndicator.SelectedItems.Count)
                {
                    cbCountry.Items.Add(one.Key);
                }
            }
            cbCountry.SelectAll();
        }

        private void CanvasRightMouseClicked(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point p = Mouse.GetPosition(MainCanvas);
            MB.Show(string.Format("({0},{1})", p.X, p.Y));
        }

        private void SettingClicked(object sender, RoutedEventArgs e)
        {
            FieldInfo[] fis = typeof(ProjSetting).GetFields();
            foreach (var one in fis)
            {
                one.SetValue(MyProj.Setting, int.Parse(settingDict[one.Name].Text));
            }
            SaveProjectClicked(null, null);
            SaveProjectByFileName(CurrentProjFileName);
            RefreshView();
        }
        private void FastPlayClicked(object sender, RoutedEventArgs e)
        {
            CurrentHeaderIndex = CalcStartHeaderIndex();
            dt = new DispatcherTimer();
            dt.Interval = new TimeSpan(0, 0, 0, 0, 250);
            dt.Tick += FastPlayTick;
            dt.Start();
        }
        private void SlowPlayClicked(object sender, RoutedEventArgs e)
        {
            CurrentSlowTick = MaxInterpolation * CalcStartHeaderIndex();
            dt = new DispatcherTimer();
            dt.Interval = new TimeSpan(0, 0, 0, 0, 1000/MaxInterpolation);
            dt.Tick += SlowPlayTick;
            dt.Start();
        }

        private int CalcStartHeaderIndex()
        {
            for (int i = 0; i < MyProj.Header.Length; i++)
            {
                if (MyProj.Header[i].StartsWith(MyProj.Setting.StartYear))
                    return i;
            }
            return 5;
        }

        private void StopPlayClicked(object sender, RoutedEventArgs e)
        {
            if (dt != null)
            {
                dt.Stop();
            }
        }
        private void FastPlayTick(object sender, EventArgs e)
        {
            if (CurrentHeaderIndex < MyProj.Header.Length - 1)
            {
                RefreshView();
                CurrentHeaderIndex++;
            }
            else
            {
                dt.Stop();
            }
        }

        private void SlowPlayTick(object sender, EventArgs e)
        {
            CurrentSlowTick++;
            CurrentHeaderIndex = CurrentSlowTick / MaxInterpolation;
            CurrentInterpolationIndex = CurrentSlowTick % MaxInterpolation;
            if (CurrentHeaderIndex < MyProj.Header.Length - 1)
            {
                RefreshView();
                CurrentHeaderIndex++;
            }
            else
            {
                dt.Stop();
            }
        }
    }
}
