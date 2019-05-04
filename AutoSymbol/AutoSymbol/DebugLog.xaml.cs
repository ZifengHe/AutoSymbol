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
using System.Windows.Shapes;

namespace AutoSymbol
{
    /// <summary>
    /// Interaction logic for DebugLog.xaml
    /// </summary>
    public partial class DebugLog : Window
    {
        public int CurrentId = 0;
        public string CurrentStack = null;
        public string CurrentMsg = null;
        public int MaxId = 1000000;
        public Dictionary<int, string> StackById = new Dictionary<int, string>();
        public Dictionary<string, TreeMessageNode> NodeByMsg = new Dictionary<string, TreeMessageNode>();
        public DebugLog()
        {
            InitializeComponent();
            foreach (var one in d.MsgDict)
            {
                foreach (var two in one.Value)
                {
                    StackById[two.Key] = one.Key;
                    NodeByMsg[two.Value.ToMsg()] = two.Value;
                }
            }
            RefreshClicked(null, null);
        }

        private void PreviousIdClicked(object sender, RoutedEventArgs e)
        {
            for (int i = CurrentId - 1; i > -1; i--)
            {
                if (StackById.ContainsKey(i))
                {
                    CurrentId = i;
                    RefreshClicked(null, null);
                    return;
                }
            }
        }

        private void NextIdClicked(object sender, RoutedEventArgs e)
        {
            for (int i = CurrentId + 1; i < MaxId; i++)
            {
                if (StackById.ContainsKey(i))
                {
                    CurrentId = i;
                    RefreshClicked(null, null);
                    return;
                }
            }
        }

        private void RefreshClicked(object sender, RoutedEventArgs e)
        {
            if (CurrentStack == null)
            {
                foreach (var one in d.MsgDict)
                {
                    lvKeys.Items.Add(one.Key);
                }
                return;
            }

            int up = CurrentId;
            int down = CurrentId;
            lvValues.Items.Clear();
            if(StackById[CurrentId] != CurrentStack)
            {
                foreach(var one in d.MsgDict[CurrentStack].Take(11))
                {
                    lvValues.Items.Add(one.Value.ToMsg());
                }
            }
            else
            {

            }
        }

        private void KeySelected(object sender, SelectionChangedEventArgs e)
        {
            CurrentStack = (string)lvKeys.SelectedValue;
            RefreshClicked(null, null);
        }

        private void MessageSelected(object sender, SelectionChangedEventArgs e)
        {
            CurrentMsg = (string)lvValues.SelectedValue;
            CurrentId = NodeByMsg[CurrentMsg].Id;
            RefreshClicked(null, null);
        }
    }
}
