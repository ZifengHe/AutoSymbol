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

namespace FrameByFrame
{
    using MB = System.Windows.MessageBox;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private static Random Rand = new Random();
        private string RootFolder;
        private Dictionary<string, CountryCodeMapping> CountryDict = new Dictionary<string, CountryCodeMapping>();
        private Dictionary<RichTextBox, string> rtcMapping = new Dictionary<RichTextBox, string>();
        ProjData MyProj = new ProjData();
        string CurrentProj;
        bool loadEveryRowInCombobox = true;
        ProjData RawData;
        Dictionary<string, Dictionary<string, OneRow>> filtered = new Dictionary<string, Dictionary<string, OneRow>>();

        public MainWindow()
        {
            InitializeComponent();
            FindRootFolder();
            LoadCountryCode();
            CurrentProj = Path.Combine(RootFolder, @"FrameByFrame\Proj\Current.xml");
            ProjData.TempFile = Path.Combine(RootFolder, @"FrameByFrame\Proj\temp.csv");
            FlagRoot = @"file://" + RootFolder + @"\FrameByFrame\Flags\";
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

            LoadCsvToMyProj(dlg.FileName);
            System.Windows.MessageBox.Show("CSV Loaded");
        }

        private void LoadCsvToMyProj(string fileName)
        {
            MyProj = new ProjData();
            MyProj.CSVContent = File.ReadAllText(fileName);
            MyProj.ProcessCSVFile();

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
            foreach(var row in MyProj.Rows)
            {
                string shortCode = CountryDict[row.CountryCode].ShortCode;
                
                if(ColorByCountry.All.ContainsKey(shortCode))
                {
                    row.TextColor = ColorByCountry.All[shortCode];
                    row.LineColor = ColorByCountry.All[shortCode];
                }
            }
            RefreshView();

        }


        //  Dictionary<string, UIElement> Created = new Dictionary<string, UIElement>();
        private void AddRtbClicked(object sender, RoutedEventArgs e)
        {
            RTBHost host = new RTBHost();
            host.ShowDialog();

            RichTextConfig rtc = new RichTextConfig();
            rtc.Title = host.txtTitle.Text;
            MyProj.RichTexts.Add(rtc);

            cbEdit.Items.Add(rtc.GetKey());
            host.gridHost.Children.Remove(host.rtbItem);
            MainCanvas.Children.Add(host.rtbItem);
            rtcMapping[host.rtbItem] = rtc.Title;

            // SyncData();
            //RefreshCanvas();
        }



        //private void SyncClicked(object sender, RoutedEventArgs e)
        //{
        //    CavasToData();
        //    DataToCanvas();
        //}


        private void RowChanged(object sender, SelectionChangedEventArgs e)
        {
        }


        private void DeleteItemClicked(object sender, RoutedEventArgs e)
        {
            if (cbEdit.SelectedValue != null)
            {
                RichTextBox toDelete = null;
                foreach (FrameworkElement fe in MainCanvas.Children)
                {
                    if (fe is RichTextBox)
                    {
                        if (rtcMapping.ContainsKey((RichTextBox)fe))
                        {
                            if (rtcMapping[(RichTextBox)fe] == (string)cbEdit.SelectedValue)
                            {
                                toDelete = (RichTextBox)fe;
                                cbEdit.Items.Remove(rtcMapping[toDelete]);
                            }
                        }
                    }
                }
                if (toDelete != null)
                {
                    MainCanvas.Children.Remove(toDelete);
                    rtcMapping.Remove(toDelete);
                }
            }

        }

        private void EditItemClicked(object sender, RoutedEventArgs e)
        {
            if (cbEdit.SelectedValue != null)
            {
                RichTextBox toEdit = null;
                foreach (FrameworkElement fe in MainCanvas.Children)
                {
                    if (fe is RichTextBox)
                    {
                        if (rtcMapping[(RichTextBox)fe] == (string)cbEdit.SelectedValue)
                        {
                            toEdit = (RichTextBox)fe;
                            RichTextConfig rtc = MyProj.GetRTC((string)cbEdit.SelectedValue);

                            RTBHost host = new RTBHost();
                            host.txtTitle.Text = rtc.Title;
                            MainCanvas.Children.Remove(toEdit);
                            host.rtbItem = toEdit;
                            host.ShowDialog();
                            host.gridHost.Children.Remove(host.rtbItem);
                            MainCanvas.Children.Add(host.rtbItem);
                            rtcMapping[host.rtbItem] = rtc.Title;
                        }
                    }
                }
            }
        }

        private void EditItemChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void RefreshClicked(object sender, RoutedEventArgs e)
        {
            //DataToCanvas();
            //RefreshView();
        }



        private void FirstFrameClicked(object sender, RoutedEventArgs e)
        {

        }



        private void ConfigureFirstFrame()
        {
            /// Methodology
            /// Step 1. Show 1st Frame
            /// Step 2. Click text to configure,  link Config->CountryCode
            /// Step 3. Redrasw 1s Frame
            /// Step 4. CountryCode will be invisible in all frame mode

        }
        private void StartClicked(object sender, RoutedEventArgs e)
        {
            foreach (string file in Directory.GetFiles(@"c:\temp\zzzzz\"))
                File.Delete(file);

            for (int i = 5; i <= MyProj.Header.Length - 1; i++)
            {
                CurrentHeaderIndex = i;
                RefreshView();
                Transform transform = MainCanvas.LayoutTransform;
                MainCanvas.LayoutTransform = null;
                System.Windows.Size size = new System.Windows.Size(MainCanvas.Width, MainCanvas.Height);
                MainCanvas.Measure(size);
                MainCanvas.Arrange(new Rect(size));
                Helper.SaveToPng(MainCanvas, @"c:\temp\zzzzz\" + i.ToString() + ".png");

            }
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


        public void CavasToData()
        {
            foreach (FrameworkElement fe in MainCanvas.Children)
            {
                if (fe is RichTextBox)
                {
                    RichTextBox item = (RichTextBox)fe;
                    //RichTextConfig one = RichTextConfig.RTCDict[item.Text];
                    RichTextConfig one = MyProj.GetRTC(rtcMapping[item]);

                    //one.Width = item.Width;
                    //one.Height = item.Height;
                    // one.RenderTransform = item.RenderTransform;
                    //one.RenderTransformOrigin = item.RenderTransformOrigin;
                    one.xamlStr = XamlWriter.Save(item);
                }
            }
        }


        private void SaveProjectClicked(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = SaveXmlFile();
            CurrentProj = dlg.FileName;
            SaveProjectByFileName(CurrentProj);
        }

        private void SaveProjectByFileName(string fileName)
        {
            CavasToData();
            ObjectManager.ToXmlFile<ProjData>(fileName, MyProj);
        }

        private void LoadProjClicked(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = OpenFile("*.xml", "XML Files (*.xml)|*.xml");
            LoadProjFile(dlg.FileName);
        }

        private void LoadProjFile(string fileName)
        {
            MyProj = ObjectManager.FromXml<ProjData>(fileName);
            MyProj.ProcessCSVFile();

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
            int width = (int)MainCanvas.Width;
            int height = (int)MainCanvas.Height;

            VideoFileWriter writer = new VideoFileWriter();
            writer.Open(@"c:\temp\Ztest.avi", width, height, 15, VideoCodec.H264);

            int totalFrame = 15 * 20;
            MaxInterpolation = totalFrame / MyProj.Header.Length;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 5; i <= MyProj.Header.Length - 1; i++)
            {
                Trace.TraceInformation("{0} of {1}", i, MyProj.Header.Length);

                for (int j = 0; j < MaxInterpolation; j++)
                {
                    CurrentHeaderIndex = i;
                    CurrentInterpolationIndex = j;
                    RefreshView();
                    Transform transform = MainCanvas.LayoutTransform;
                    MainCanvas.LayoutTransform = null;
                    System.Windows.Size size = new System.Windows.Size(MainCanvas.ActualWidth, MainCanvas.ActualHeight);
                    MainCanvas.Measure(size);
                    MainCanvas.Arrange(new Rect(size));
                    Bitmap b = new Bitmap(Helper.CreateBmpStream(MainCanvas));
                    writer.WriteVideoFrame(b);
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
            RawData.ProcessCSVFile();
            
            cbCountry.Items.Clear();
            foreach(var one in  RawData.Rows.GroupBy(x=>x.CountryName))
                cbCountry.Items.Add(one.Key);

           
            System.Windows.MessageBox.Show("wdi file loaded");
        }

        private void RunFilterClicked(object sender, RoutedEventArgs e)
        {
            if (cbCountry.SelectedItems.Count != 1)
            {
                MB.Show("Filter must work on single focused country");
                return;
            }

            /// Start year 1996 end year 2016
            int index1996 = -1;
            int index2016 = -1;
            string focusCountry = cbCountry.SelectedValue;

            for(int i=0; i< RawData.Header.Length; i++)
            {
                if (RawData.Header[i].Contains("1996"))
                    index1996 = i;
                if (RawData.Header[i].Contains("2016"))
                    index2016 = i;
            }

            
            filtered.Clear();

            foreach(var one in RawData.Rows)
            {
                if (one.Data.ContainsKey(RawData.Header[index1996]) == false)
                    continue;
                if(one.Data.ContainsKey(RawData.Header[index2016]) == false)
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

                if(one.Value.Where(x=>x.Value.Magic!=0).Max(x=>x.Value.Magic) == one.Value[focusCountry].Magic
                    || one.Value.Where(x => x.Value.Magic != 0).Min(x => x.Value.Magic) == one.Value[focusCountry].Magic)
                {
                    cbIndicator.Items.Add(one.Key);
                    Trace.TraceInformation(one.Key);
                    foreach(var detail in one.Value)
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

            foreach(string line in rawLines)
            {
                bool keep = false;
                foreach(var indicator in cbIndicator.SelectedItems)
                {
                    string key = indicator.ToString();
                    if(line.Contains(key))
                    {
                        foreach(var item in filtered[key])
                        {
                            if(line.Contains(item.Key))
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
            foreach(var one in countryCount)
            {
                if(one.Value == cbIndicator.SelectedItems.Count)
                {
                    cbCountry.Items.Add(one.Key);
                }
            }
            cbCountry.SelectAll();
        }
    }
}
