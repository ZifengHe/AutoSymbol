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
        public static Reminder r = new Reminder();

        string[] trackinURL = {
                "seekingalpha",
                "seeking alpha",
                "xueqiu",
                "雪球",
                "wallstreet",
                "见闻",
                "linkedin",
                "facebook",
                "quora",
                "youtube",
                "wenxuecity",
                "bloomberg",
                "cnbc",
                "google news",
                "cnn",
                "zhihu",
                "wealthscape",
                "知乎",
                 "格隆汇",
                "文学城",
                "mitbbs",
                "fidelity",
                "twitter",
                "网易",
                "新浪",
                "搜狐",
                "wall street",
                "sven"
            };
        DateTime lastReadTime = DateTime.Now.AddMinutes(-20);

        AllDailyLog allDailyLog = new AllDailyLog();
        List<Line> All = new List<Line>();
        OneCheckItem currentItem;
        string fileName = @"C:\Users\zifengh\OneDrive\DailyReview\ContentOne.txt";
        string backupFile = @"C:\Users\zifengh\OneDrive\DailyReview\ContentOneBackup.txt";
        //string logFile = @"C:\Users\zifengh\OneDrive\DailyReview\log.txt";
        string scoreFile = @"C:\Users\zifengh\OneDrive\DailyReview\ScoreCard.txt";
        int current = 0;
        int lastScore = 0;
        FileSystemWatcher fsw;
        public MainWindow()
        {
            //if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Count() == 1)
            //{
            //    string fileName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //    Process.Start(fileName);
            //}

            Mutex mutex = new Mutex(false, "Global\\12345678");
            if (!mutex.WaitOne(0, false))
            {
                Environment.Exit(100);
            }

            if (File.Exists(@"C:\Users\zifeng\OneDrive\DailyReview\ContentOne.txt"))
            {
                fileName = @"C:\Users\zifeng\OneDrive\DailyReview\ContentOne.txt";
                backupFile = @"C:\Users\zifeng\OneDrive\DailyReview\ContentOneBackup.txt";
                //logFile = @"C:\Users\zifeng\OneDrive\DailyReview\log.txt";
                scoreFile = @"C:\Users\zifeng\OneDrive\DailyReview\ScoreCard.txt";
            }
            InitializeComponent();
            this.WindowState = System.Windows.WindowState.Maximized;
            ReloadFile();
            EnableWatchAndNotify();

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += OnTimerAction;
            dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
            dispatcherTimer.Start();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            File.WriteAllText(@"c:\crash.txt", e.ToString());
        }

        void Harass()
        {
            IntPtr lHwnd = FindWindow("Shell_TrayWnd", null);
            // SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL, IntPtr.Zero);
            this.Activate();
            // if (MainWindow.r == null)
            // if(MainWindow.r.Visibility== Visibility.Hidden
            //    || MainWindow.r.WindowState == WindowState.)
            // MainWindow.r.Visibility = Visibility.Visible;
            MainWindow.r.Close();
            MainWindow.r = new Reminder();
            MainWindow.r.Show();
            // MainWindow.r.WindowState = WindowState.Maximized;
            // this.WindowState = WindowState.Maximized;
        }

        private void OnTimerAction(object sender, EventArgs e)
        {

            CloseForOverRead();

            currentItem = allDailyLog.CheckPendingItemForToday();
            if (currentItem != null)
            {
                CurrentItemText.Text = currentItem.Description;
                if (KeyAnswer.Text == currentItem.KeyAnswer)
                {
                    currentItem.Status = CompleteStatus.KeyInputComplete;
                    return;
                }

                if(this.Visibility != Visibility.Visible)
                    Harass();
                return;
            }
        }

        private static long TimeCount = 0;
        private void CloseForOverRead()
        {
            TimeCount++;
            TimeSpan ts = DateTime.Now - lastReadTime;
            int minutes = ts.Days * 1440 + ts.Hours * 60 + ts.Minutes;
            tbReadTime.Text = string.Format("{0} minutes since last read.", minutes);

            List<Process> toKill = new List<Process>();
            Process[] procsChrome = Process.GetProcessesByName("chrome");
            //Process[] processEdge = Process.GetProcessesByName("microsoftedge");
            //foreach (var p in processEdge)
            //    p.Kill();

            List<Process> pList = new List<Process>();
            //pList.AddRange(processEdge);
            pList.AddRange(procsChrome);
            if (pList.Count() <= 0)
                return;

            //Process[] processVS = Process.GetProcessesByName("devenv");
            //if (procsChrome.Length > 0)
            //    foreach (Process proc in processVS)
            //        proc.Kill();
            int hour = DateTime.Now.Hour;
            if (hour < 17 && TimeCount % 8 != 0)
                return;
            if (hour>17 && hour < 20 && TimeCount % 15 != 0)
                return;
            if (hour==20 && TimeCount % 3 != 0)
                return;



            //if (DateTime.Now.Hour == 19
            //    || DateTime.Now.Hour == 18
            //     || DateTime.Now.Hour == 17
            //    || ((DateTime.Now.Hour == 6
            //    || DateTime.Now.Hour == 7)&& DateTime.Now.DayOfWeek== DayOfWeek.Friday))
            //    return;

            foreach (Process proc in pList)
            {
                // the chrome process must have a window 
                if (proc.MainWindowHandle == IntPtr.Zero)
                {
                    continue;
                }

                string x = proc.MainWindowTitle.ToLower();

                foreach (var one in trackinURL)
                {
                    if (x.ToLowerInvariant().Contains(one))
                    {
                        toKill.Add(proc);
                    }
                }
            }

            //if (((minutes > 20 && minutes < 150)
            //    ||DateTime.Now.Hour>20 || DateTime.Now.Hour<7))
            //{
            foreach (var p in toKill)
            {
                try
                {
                    p.Kill();
                }
                catch
                { }
            }
            //}
            //else if (minutes >= 40)
            //{
            //    if (toKill.Count() > 0)
            //        lastReadTime = DateTime.Now;
            //}
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
            Thread.Sleep(15000);
            this.Dispatcher.Invoke(() => { ReloadFile(); });
        }

        private void SaveAllChanges(bool scoreCardOnly)
        {
            //DisableWatchAndNotify();
            //if (scoreCardOnly == false)
            //{
            //    All = All.OrderByDescending(x => x.Index).ToList();
            //    string[] items = new string[All.Count + 1];
            //    items[0] = "Index|Date|Category|Question|Answer|TestFrequency";
            //    for (int i = 0; i < All.Count; i++)
            //    {
            //        Line l = All[i];
            //        items[i + 1] = string.Format("{0}|{1}|{2}|{3}|{4}|{5}", l.Index.ToString(),
            //            l.Date, l.Category, l.Question, l.Answer, l.TestFrequency);
            //    }
            //    File.WriteAllLines(fileName, items);

            //    if (File.Exists(backupFile) == false || File.ReadAllLines(backupFile).Length < File.ReadAllLines(fileName).Length)
            //        File.Copy(fileName, backupFile, true);
            //}

            //ObjectManager.ToXmlFile<AllDailyLog>(scoreFile, allDailyLog);
            //EnableWatchAndNotify();
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

            cbAll.Items.Clear();
            foreach (var one in lines.ToList().Take(200))
                cbAll.Items.Add(one);

            /// Reload score card
            if (File.Exists(scoreFile))
                allDailyLog = ObjectManager.FromXml<AllDailyLog>(scoreFile);

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
            ReloadFile();
            one.Index = All.Count + 1;
            one.TestFrequency = 'A';
            All.Insert(0, one);
            SaveAllChanges(false);
            MessageBox.Show("New entry added");
            txtAnswer.Text = string.Empty;
            txtQuestion.Text = string.Empty;
        }

        private void ClearClicked(object sender, RoutedEventArgs e)
        {
            txtAnswer.Text = string.Empty;
            txtQuestion.Text = string.Empty;
        }

        private void wndClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveAllChanges(false);
            string fileName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            Process.Start(fileName);
        }

        private void UpdateClicked(object sender, RoutedEventArgs e)
        {
            All[current].Question = txtQuestion.Text;
            All[current].Answer = txtAnswer.Text;
            SaveAllChanges(false);
            MessageBox.Show("Current entry updated");
        }



        private void ShowAnswerClicked(object sender, RoutedEventArgs e)
        {
            txtQuestion.Text = string.Format("{0}", All[current].Question);
            txtAnswer.Text = All[current].Answer;
            cbCategory.SelectedValue = All[current].Category;
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
            if (currentItem != null)
            {
                if (currentItem.Status != CompleteStatus.KeyInputComplete)
                {
                    if (cDone.IsChecked == true) currentItem.Status = CompleteStatus.Done;
                    if (cValidExcuse.IsChecked == true) currentItem.Status = CompleteStatus.ValidExcuse;
                    if (cDoItNow.IsChecked == true) currentItem.Status = CompleteStatus.DoItNow;
                    if (cFailed.IsChecked == true) currentItem.Status = CompleteStatus.Failed;
                }

                SaveAllChanges(true);

                OnTimerAction(null, null);
                KeyAnswer.Text = string.Empty;
            }
            else
            {
                WindowState = WindowState.Minimized;
            }
        }

        private void MinimizeClicked(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void SymbolLabelMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (txtSymbols.Visibility == Visibility.Collapsed)
                txtSymbols.Visibility = Visibility.Visible;
            else
                txtSymbols.Visibility = Visibility.Collapsed;

        }
    }
}
