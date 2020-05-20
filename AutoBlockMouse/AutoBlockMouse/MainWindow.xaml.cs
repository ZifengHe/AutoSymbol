using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace AutoBlockMouse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
        }

       

        private void StartClicked(object sender, RoutedEventArgs e)
        {
            counter = 0;
            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
            timer.Start();
            timer.Tick += ThinkOneMinute;

            if (cb.SelectedIndex == 0)
            {
                cycle = 30;
            }

            if (cb.SelectedIndex == 1)
            {
                cycle = 6;

            }
        }

        private void StopClicked(object sender, RoutedEventArgs e)
        {
            bg.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            timer.Stop();

        }


        static DispatcherTimer timer;
        static int counter = 0;
        static int cycle = 6;
        private void ThinkOneMinute(object sender, EventArgs e)
        {
            this.Topmost = true;
            this.Show();
            
            if (counter % cycle == 0)
                bg.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            else
                bg.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));

            counter++;
        }

        private void Window_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }
    }
}
