using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MyDailyReview
{

    public class Line
    {
        public int Index;
        public string Date;
        public string Category;
        public string Question;
        public string Answer;
        public char TestFrequency;
    }
    public partial class MainWindow : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]
        static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

        const int WM_COMMAND = 0x111;
        const int MIN_ALL = 419;
        const int MIN_ALL_UNDO = 416;

        List<Line> All = new List<Line>();
        string fileName = @"C:\Users\zifengh\OneDrive\DailyReview\ContentOne.txt";
        string backupFile = @"C:\Users\zifengh\OneDrive\DailyReview\ContentOneBackup.txt";
        string logFile = @"C:\Users\zifengh\OneDrive\DailyReview\log.txt";
        string scoreFile = @"C:\Users\zifengh\OneDrive\DailyReview\ScoreCard.txt";
        int current = 0;
        int lastScore = 0;
        FileSystemWatcher fsw;
        public MainWindow()
        {
            Mutex mutex = new Mutex(false, "Global\\12345678");
            if (!mutex.WaitOne(0, false))
            {
                Environment.Exit(100);
            }

            if (File.Exists(@"C:\Users\zifeng\OneDrive\DailyReview\ContentOne.txt"))
            {
                fileName = @"C:\Users\zifeng\OneDrive\DailyReview\ContentOne.txt";
                backupFile = @"C:\Users\zifeng\OneDrive\DailyReview\ContentOneBackup.txt";
                logFile = @"C:\Users\zifeng\OneDrive\DailyReview\log.txt";
                scoreFile = @"C:\Users\zifeng\OneDrive\DailyReview\ScoreCard.txt";
            }
            InitializeComponent();
            this.WindowState = System.Windows.WindowState.Maximized;
            ReloadFile();
            EnableWatchAndNotify();

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
            dispatcherTimer.Start();
        }

         void Harass()
        {
            IntPtr lHwnd = FindWindow("Shell_TrayWnd", null);
            SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL, IntPtr.Zero);
            this.Activate();
            this.Topmost = true;
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            CloseTabs();
        }

        private void CloseTabs()
        {
            Process[] procsChrome = Process.GetProcessesByName("chrome");
            if (procsChrome.Length <= 0)
            {
                Console.WriteLine("Chrome is not running");
                return;
            }

            foreach (Process proc in procsChrome)
            {
                Trace.TraceInformation(proc.MainWindowTitle);
                // the chrome process must have a window 
                if (proc.MainWindowHandle == IntPtr.Zero)
                {
                    continue;
                }

                string x = proc.MainWindowTitle.ToLower();
                if (x.Contains("seekingalpha")
                    || x.Contains("seeking alpha"))
                    proc.Kill();


            }

        }
        private void StartClicked(object sender, RoutedEventArgs e)
        { }
        private void DoneClicked(object sender, RoutedEventArgs e)
        { }
        private void DisableWatchAndNotify()
        {
            if (fsw != null)
            {
                fsw.Changed -= OnFileChangedElsewhere;
                fsw.Dispose();
            }
        }

        private void EnableWatchAndNotify()
        {
            fsw = new FileSystemWatcher();
            fsw.Path = System.IO.Path.GetDirectoryName(fileName);
            fsw.NotifyFilter = NotifyFilters.LastWrite;
            fsw.Changed += OnFileChangedElsewhere;
            fsw.EnableRaisingEvents = true;
        }

        private void OnFileChangedElsewhere(object source, FileSystemEventArgs e)
        {
            MessageBox.Show("File changed while this instance sits idle. Reloading ...");
            ReloadFile();
        }

        private void SaveAllChanges()
        {
            DisableWatchAndNotify();
            All = All.OrderByDescending(x => x.Index).ToList();
            string[] items = new string[All.Count + 1];
            items[0] = "Index|Date|Category|Question|Answer|TestFrequency";
            for (int i = 0; i < All.Count; i++)
            {
                Line l = All[i];
                items[i + 1] = string.Format("{0}|{1}|{2}|{3}|{4}|{5}", l.Index.ToString(),
                    l.Date, l.Category, l.Question, l.Answer, l.TestFrequency);
            }
            File.WriteAllLines(fileName, items);

            if (File.Exists(backupFile) == false || File.ReadAllLines(backupFile).Length < File.ReadAllLines(fileName).Length)
                File.Copy(fileName, backupFile, true);

            List<string> list = new List<string>();
            if (File.Exists(logFile))
                list = File.ReadAllLines(logFile).ToList();
            list.Insert(0, string.Format("Score ： {0} on {1}", lastScore, DateTime.Today.Date));
            File.WriteAllLines(logFile, list.ToArray());
            EnableWatchAndNotify();
        }

        private void ReloadFile()
        {
            All.Clear();
            string[] lines = File.ReadAllLines(fileName);
            int score = 0;
            for (int i = 1; i < lines.Length; i++)
            {
                score += 10;
                string[] item = lines[i].Split('|');
                Line newOne = new Line();
                newOne.Index = int.Parse(item[0].TrimStart().TrimEnd());
                newOne.Date = item[1].TrimStart().TrimEnd();
                newOne.Category = item[2].TrimStart().TrimEnd();
                newOne.Question = item[3].TrimStart().TrimEnd();
                newOne.Answer = item[4].TrimStart().TrimEnd();
                newOne.TestFrequency = item[5].TrimStart().TrimEnd().First();
                score += 3 * (int)(newOne.TestFrequency - 'A');
                All.Add(newOne);
            }

            All = All.OrderBy(x => x.TestFrequency).ToList();
            cbCategory.ItemsSource = All.GroupBy(x => x.Category).Select(x => x.Key).ToList();

            current = 0;
            lblName.Content = string.Format("{0} : {1}", All[current].Category, All[current].Question);
            tbScore.Text = score.ToString();
            lastScore = score;

            cbLog.Items.Clear();
            cbAll.Items.Clear();
            cbScoreCard.Items.Clear();
            foreach (var one in lines.ToList().Take(200))
                cbAll.Items.Add(one);
            foreach (var one in File.ReadAllLines(logFile).Take(200))
                cbLog.Items.Add(one);
            foreach (var one in File.ReadAllLines(scoreFile).Take(200))
                cbScoreCard.Items.Add(one);

        }

        private void Pass_Checked(object sender, RoutedEventArgs e)
        {
            All[current].TestFrequency = (char)((int)(All[current].TestFrequency) + 1);
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() => { System.Threading.Thread.Sleep(1000); cbFail.IsChecked = false; cbPass.IsChecked = false; }));

            MoveToNext();
        }
        private void FailClicked(object sender, RoutedEventArgs e)
        {
            if (All[current].TestFrequency > 'A')
            {
                All[current].TestFrequency = (char)((int)(All[current].TestFrequency) - 1);
            }
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() => { System.Threading.Thread.Sleep(1000); cbFail.IsChecked = false; cbPass.IsChecked = false; }));


            MoveToNext();
        }
        private void MoveToNext()
        {

            current++;
            if (current < All.Count())
                lblName.Content = string.Format("{0} : {1}", All[current].Category, All[current].Question);
            else
                MessageBox.Show("That is the end of current round.");
        }

        private void AddNewClicked(object sender, RoutedEventArgs e)
        {
            Line one = new Line();
            one.Date = DateTime.Now.Date.ToShortDateString();
            one.Category = (string)cbCategory.SelectedValue;
            one.Question = txtQuestion.Text;
            one.Answer = txtAnswer.Text;
            one.Index = All.Count + 1;
            one.TestFrequency = 'A';
            All.Insert(0, one);
            SaveAllChanges();
            MessageBox.Show("New entry added");
        }

        private void ClearClicked(object sender, RoutedEventArgs e)
        {
            txtAnswer.Text = string.Empty;
            txtQuestion.Text = string.Empty;
        }

        private void wndClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveAllChanges();
        }

        private void UpdateClicked(object sender, RoutedEventArgs e)
        {
            All[current - 1].Question = txtQuestion.Text;
            All[current - 1].Answer = txtAnswer.Text;
            SaveAllChanges();
            MessageBox.Show("Current entry updated");
        }



        private void ShowAnswerClicked(object sender, RoutedEventArgs e)
        {
            txtQuestion.Text = string.Format("{0} : {1}", All[current].Category, All[current].Question);
            txtAnswer.Text = All[current].Answer;
        }

       

        private void NextClicked(object sender, RoutedEventArgs e)
        {
            MoveToNext();
        }

        private void WindowsLoaded(object sender, RoutedEventArgs e)
        {
            //var hwnd = new WindowInteropHelper(this).Handle;
            //SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private void SaveOneEntry(object sender, RoutedEventArgs e)
        {

        }
    }
}
