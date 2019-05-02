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

namespace FrameByFrame
{
    
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
        public MainWindow()
        {
            InitializeComponent();
            FindRootFolder();
            LoadCountryCode();
            CurrentProj = Path.Combine(RootFolder, @"FrameByFrame\Proj\Current.xml");   
            ProjData.TempFile = Path.Combine(RootFolder, @"FrameByFrame\Proj\temp.csv");
        }  
        
        void FindRootFolder()
        {
            if (Directory.Exists(@"C:\Users\zifengh\Source\Repos\ZifengHe\AutoSymbol"))
                RootFolder = @"C:\Users\zifengh\Source\Repos\ZifengHe\AutoSymbol";
        }
       
        private void ProcessOneFrame(int index)
        {
            char[] sep = new char[] { ' ' };
            //string timeStr = Header[index].Split(sep)[0];

        }      
     
        private void CSVClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = OpenFile("*.csv", "CSV Files (*.csv)|*.csv");

            MyProj.CSVContent = File.ReadAllText(dlg.FileName);
            MyProj.ProcessCSVFile();      

            cbConfig.Items.Clear();
            foreach(var one in MyProj.Rows)
            {
                cbConfig.Items.Add(one.Id());
            }
        }

        private void RowLineColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            MyProj.GetRow((string)cbConfig.SelectedValue).LineColor = RowLineColor.SelectedColor.Value;
            RefreshView();
        }

        private void RowTextColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            MyProj.GetRow((string)cbConfig.SelectedValue).TextColor = RowTextColor.SelectedColor.Value;
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
            rtcMapping[host.rtbItem]=rtc.Title;

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
                        if (rtcMapping[(RichTextBox)fe] == (string)cbEdit.SelectedValue)
                        {
                            toDelete = (RichTextBox)fe;
                            cbEdit.Items.Remove(rtcMapping[toDelete]);
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
            if (MyProj.Header == null || MyProj.Header.Length < 4)
            {
                System.Windows.MessageBox.Show("Forgot to load CSV?");
                return;
            }

            for (int i = 4; i < MyProj.Header.Length; i++)
            {
                ProcessOneFrame(i);
            }


            string cs = "abcdefghigjdfsdfsfds";
            for (int i = 1; i < 8; i++)
            {
                string text = cs.Substring(0, i * 2);
                TextBlock tb = new TextBlock();
                tb.Text = text;
                tb.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
                Canvas.SetRight(tb, 600);
                Canvas.SetTop(tb, 100 + i * 20);
                MainCanvas.Children.Add(tb);

                Line line = new Line();
                line.Stroke = Brushes.AliceBlue;
                line.StrokeThickness = 10;
                line.X1 = 200;
                line.X2 = 250 + i * 30;
                line.Y1 = 100 + i * 20;
                line.Y2 = 100 + i * 20;

                MainCanvas.Children.Add(line);
            }

            Image cnImg = new Image
            {
                Width = 60,
                Height = 45,
                Name = "cn",
                Source = new BitmapImage(new Uri(@"file://C:\Users\zifengh\source\repos\ZifengHe\AutoSymbol\FrameByFrame\Flags\cn.png")),
            };

            MainCanvas.Children.Add(cnImg);
            Canvas.SetTop(cnImg, 300);
            Canvas.SetLeft(cnImg, 500);



            Transform transform = MainCanvas.LayoutTransform;
            MainCanvas.LayoutTransform = null;
            Size size = new Size(MainCanvas.Width, MainCanvas.Height);
            MainCanvas.Measure(size);
            MainCanvas.Arrange(new Rect(size));
            Helper.SaveToPng(MainCanvas, @"c:\temp\1.png");
        }

        private void BackGroundClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = OpenFile(
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

        private OpenFileDialog OpenFile(string ext, string filter)
        {
            OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ext;
            dlg.Filter = filter;
            Nullable<bool> result = dlg.ShowDialog();
            if (result != true)
                return null;
            return dlg;
        }

        private SaveFileDialog SaveXmlFile()
        {
            SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
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
            CavasToData();
            SaveFileDialog dlg = SaveXmlFile();
            CurrentProj = dlg.FileName;
            ObjectManager.ToXmlFile<ProjData>(CurrentProj, MyProj);
        }

        private void LoadProjClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = OpenFile("*.xml", "XML Files (*.xml)|*.xml");
            
            MyProj = ObjectManager.FromXml<ProjData>(dlg.FileName);
            RefreshView();           
        }
    }
}
