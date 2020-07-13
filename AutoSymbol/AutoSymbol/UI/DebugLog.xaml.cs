using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;
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

using AutoSymbol.Core;

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
                    NodeByMsg[two.Value.ToDisplayMessage()] = two.Value;
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
                    CurrentStack = StackById[CurrentId];
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
                    CurrentStack = StackById[CurrentId];
                    RefreshClicked(null, null);
                    return;
                }
            }
        }

        private void RefreshClicked(object sender, RoutedEventArgs e)
        {
            tbStack.Text = string.Empty;
            tbMessage.Text = string.Empty;
            if (CurrentStack == null)
            {
                foreach (var one in d.MsgDict)
                {
                    lvKeys.Items.Add(one.Key);
                }
                return;
            }
            tbStack.Text = CurrentStack;
            RefreshMessagePanel();
            
        }
        private void RefreshMessagePanel()
        {
            lvValues.Items.Clear();
            /// Bug CurrentId==0
            if (StackById[CurrentId] != CurrentStack)
            {
                foreach (var one in d.MsgDict[CurrentStack].Take(11))
                {
                    lvValues.Items.Add(one.Value.ToDisplayMessage());
                }
            }
            else
            {
                List<TreeMessageNode> list = new List<TreeMessageNode>();
                int reverseCount = 6;

                foreach (var one in d.MsgDict[CurrentStack])
                {
                    list.Add(one.Value);
                    if (list.Count > 10)
                        list.RemoveAt(0);

                    if (one.Value.Id >= CurrentId)
                    {
                        reverseCount--;
                        if (one.Value.Id == CurrentId)
                            tbMessage.Text = one.Value.ToDisplayMessage();
                    }

                    if (reverseCount <= 0)
                        break;
                }
                foreach (var item in list)
                    lvValues.Items.Add(item.ToDisplayMessage());
            }
        }

        private void KeySelected(object sender, SelectionChangedEventArgs e)
        {
            if (lvKeys.SelectedValue != null)
            {
                CurrentStack = (string)lvKeys.SelectedValue;
                RefreshMessagePanel();
                //RefreshClicked(null, null);
            }
        }

        private void MessageSelected(object sender, SelectionChangedEventArgs e)
        {
            if (lvValues.SelectedValue != null)
            {
                CurrentMsg = (string)lvValues.SelectedValue;
                CurrentId = NodeByMsg[CurrentMsg].Id;
                //RefreshClicked(null, null);
                RenderObjects(NodeByMsg[CurrentMsg]);
            }
        }

        private void RenderObjects(TreeMessageNode node)
        {
            if(node.ObjOne != null)
            {
                RenderObjectOnPanel(node.ObjOne, DLeftPanel);
            }
            if(node.ObjTwo != null)
            {
                RenderObjectOnPanel(node.ObjTwo, DRightPanel);
            }
        }

        private void RenderObjectOnPanel(object obj, DockPanel panel)
        {
            GraphViewer graphViewer = new GraphViewer();
            panel.Children.Clear();
            graphViewer.BindToPanel(panel);
            Graph graph = new Graph();
           
            if(obj is OpNode)
            {
                OpNode opc = (OpNode) obj;
                graph.AddNode(opc.Sig);
            }
            if(obj is ReplaceRule)
            {
                ReplaceRule er = (ReplaceRule)obj;
                graph.AddNode(er.Left.Sig);
                graph.AddNode(er.Right.Sig);

            }
            graphViewer.Graph = graph;

        }
    }
}
