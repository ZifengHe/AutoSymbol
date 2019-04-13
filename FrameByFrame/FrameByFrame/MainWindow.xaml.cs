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

namespace FrameByFrame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Color SelectedColor;
        DataTable dt = new DataTable();
        public MainWindow()
        {
            InitializeComponent();
        }

        void SaveToBmp(FrameworkElement visual, string fileName)
        {
            var encoder = new BmpBitmapEncoder();
            SaveUsingEncoder(visual, fileName, encoder);
        }

        void SaveToPng(FrameworkElement visual, string fileName)
        {
            var encoder = new PngBitmapEncoder();
            SaveUsingEncoder(visual, fileName, encoder);
        }

        // and so on for other encoders (if you want)


        void SaveUsingEncoder(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            BitmapFrame frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);

            using (var stream = File.Create(fileName))
            {
                encoder.Save(stream);
            }
        }

        private void StartClicked(object sender, RoutedEventArgs e)
        {
            string cs = "abcdefghigjdfsdfsfds";
            for (int i = 1; i < 8; i++)
            {
                string text = cs.Substring(0, i * 2);
                TextBlock tb = new TextBlock();
                tb.Text = text;
                tb.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
                Canvas.SetRight(tb, 600);
                Canvas.SetTop(tb, 100 + i * 20);
                ContentPanel.Children.Add(tb);

                Line line = new Line();
                line.Stroke = Brushes.AliceBlue;
                line.StrokeThickness = 10;
                line.X1 = 200;
                line.X2 = 250 + i * 30;
                line.Y1 = 100 + i * 20;
                line.Y2 = 100 + i * 20;

                ContentPanel.Children.Add(line);
            }



            Transform transform = ContentPanel.LayoutTransform;
            ContentPanel.LayoutTransform = null;
            Size size = new Size(ContentPanel.Width, ContentPanel.Height);
            ContentPanel.Measure(size);
            ContentPanel.Arrange(new Rect(size));
            SaveToPng(ContentPanel, @"c:\temp\1.png");
        }

        private void BackGroundClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = OpenFile(
                "*.png",
                "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif");

            ImageBrush myBrush = new ImageBrush();
            myBrush.ImageSource = new BitmapImage(new Uri(@"file://" + dlg.FileName));
            ContentPanel.Background = myBrush;
        }


        private void CSVClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = OpenFile("*.csv", "CSV Files (*.csv)|*.csv");
            dt.Clear();

            using (TextFieldParser parser = new TextFieldParser(dlg.FileName))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                bool firstLine = true;

                while (!parser.EndOfData)
                {
                    //Processing row
                    string[] fields = parser.ReadFields();
                    //for (int i = 0; i < fields.Length; i++)
                    //    fields[i] = ":" + fields[i];

                    // get the column headers
                    if (firstLine)
                    {
                        foreach (var val in fields)
                        {
                            dt.Columns.Add(val);
                        }

                        firstLine = false;

                        continue;
                    }


                    // get the row data
                    dt.Rows.Add(fields);
                }
            }

            gridData.ItemsSource = dt.DefaultView;
            //gridData.DataContext = dt;
            ContentPanel.Visibility = Visibility.Collapsed;

            for (int i = 0; i < dt.Rows.Count; i++)
                for (int j = 0; j < dt.Rows[i].ItemArray.Count(); j++)
                    gridData.GetCell(i, j).Content = (string)dt.Rows[i].ItemArray[j];

        }

        private OpenFileDialog OpenFile(string ext, string filter)
        {
            OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ext;
            dlg.Filter = filter;
            Nullable<bool> result = dlg.ShowDialog();
            if (result != true)
                throw new ApplicationException("Fail to open file");
            return dlg;
        }

        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            var mouseWasDownOn = e.Source as FrameworkElement;
            if (mouseWasDownOn is Line)
            {
                Line line = (Line)mouseWasDownOn;
                line.Stroke = new SolidColorBrush(SelectedColor);
            }
        }

        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            SelectedColor = ClrPcker_Background.SelectedColor.Value;
        }
    }

    public static class Helper { 
        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        public static DataGridRow GetSelectedRow(this DataGrid grid)
        {
            return (DataGridRow)grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem);
        }
        public static DataGridRow GetRow(this DataGrid grid, int index)
        {
            DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // May be virtualized, bring into view and try again.
                grid.UpdateLayout();
                grid.ScrollIntoView(grid.Items[index]);
                row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        public static DataGridCell GetCell(this DataGrid grid, DataGridRow row, int column)
        {
            if (row != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);

                if (presenter == null)
                {
                    grid.ScrollIntoView(row, grid.Columns[column]);
                    presenter = GetVisualChild<DataGridCellsPresenter>(row);
                }

                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                return cell;
            }
            return null;
        }

        public static DataGridCell GetCell(this DataGrid grid, int row, int column)
        {
            DataGridRow rowContainer = grid.GetRow(row);
            return grid.GetCell(rowContainer, column);
        }
    }
}
