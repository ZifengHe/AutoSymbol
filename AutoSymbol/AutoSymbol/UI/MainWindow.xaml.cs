using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;

using AutoSymbol.Core;

namespace AutoSymbol
{
    using StrToOp = Dictionary<string, OpChain>;
    public static class UIData
    {
       // public static StrToOp ItemMap;
        public static List<string> AllItems;
    }

    public enum RunningMode
    {
        Invalid,
        ManualBuild,
        Benchmark
    }

    public partial class MainWindow : Window
    {
        public List<string> TransList = new List<string>();        

        public RunningMode MyMode = RunningMode.Benchmark;

        public MainWindow()
        {            
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            for (int i = 0; i < 50; i++)
                cbGen.Items.Add(i);
        }

        private static Dictionary<string, int> EdgeCount = new Dictionary<string, int>();

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadManualTransforms();
        }

       
        private void Rebind()
        {
            cbList.Items.Clear();
            foreach (var one in UIData.AllItems.OrderBy(x=>x.Length))
                cbList.Items.Add(one);
        }

        private void RunAllClicked(object sender, RoutedEventArgs e)
        {
            // new Builder().BuildPattern0();
            Benchmark.RunAll();
            Rebind();
            MessageBox.Show("All Benchmark passed");
        }

        private void RunOneClicked(object sender, RoutedEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Benchmark.RunOne();
            Rebind();
            MessageBox.Show("One Benchmark passed " + sw.Elapsed.TotalSeconds.ToString() + " secondes");
        }

        private void CbList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbList.SelectedValue != null)
            {
                MyMode = RunningMode.Benchmark;
                string sig = (string)cbList.SelectedValue;
                TransList.Clear();
                TransList.Insert(0, sig);
                string current = sig;

                while (OneTransform.AllResult.ContainsKey(current))
                {
                    OneTransform one = OneTransform.AllResult[current];
                    if (one.Original != null)
                    {
                        string newSig = one.Original.Sig;
                        if (OneTransform.AllResult.ContainsKey(newSig))
                        {
                            TransList.Insert(0, newSig);
                        }
                        current = newSig;
                    }
                    else
                    {
                        break;
                    }
                }
                //ViewOneOpChain(sig);
                ViewFromStartToLast();
            }
        }

        private void ViewFromStartToLast()
        {
            GraphViewer graphViewer = new GraphViewer();
            Graph graph = PrepareLeftPanelGraph(graphViewer);
            for (int i = 0; i < TransList.Count; i++)
            {
                graph.AddNode(TransList[i]);

                if (i != TransList.Count - 1)
                {
                    graph.AddEdge(TransList[i], TransList[i+1]).Attr.Color = Microsoft.Msagl.Drawing.Color.Green;
                }
            }
            graphViewer.Graph = graph;
        }

        private void ViewOneOpChain(string sig)
        {
            GraphViewer graphViewer = new GraphViewer();
            Graph graph = PrepareLeftPanelGraph(graphViewer);
            RenderOneTransform(OneTransform.AllResult[sig], graph);
            graphViewer.Graph = graph;
        }

        private Graph PrepareLeftPanelGraph(GraphViewer graphViewer)
        {
            EdgeCount.Clear();
            LeftPanel.Children.Clear();            
            graphViewer.BindToPanel(LeftPanel);
            graphViewer.ObjectUnderMouseCursorChanged += LeftPanelUnderMouseObjectChanged;
            graphViewer.MouseDown += LeftPanelMouseDown;
            Graph graph = new Graph();         
            return graph;
        }

        private Graph PrepareRightPanelGraph(GraphViewer graphViewer)
        {
            EdgeCount.Clear();
            RightPanel.Children.Clear();
            graphViewer.BindToPanel(RightPanel);
            graphViewer.MouseDown += RightPanelMouseDown;
            Graph graph = new Graph();
            
            return graph;
        }

        private void RightPanelMouseDown(object sender, MsaglMouseEventArgs e)
        {
            switch (MyMode)
            {
                case RunningMode.ManualBuild:
                    RightPanelMouseDown_ManualBuildMode(sender);
                    break;               
                default:
                    break;
            }
        }

        private void LeftPanelMouseDown(object sender, MsaglMouseEventArgs e)
        {
            switch(MyMode)
            {
                case RunningMode.ManualBuild:
                    LeftPanelMouseDown_ManualBuildMode(sender);
                    break;
                case RunningMode.Benchmark:
                    LeftPanelMouseDown_BenchmarkMode(sender);
                    break;
                default:
                    break;
            }
        }

        private void LeftPanelMouseDown_BenchmarkMode(object sender)
        {
            GraphViewer gv = (GraphViewer)sender;
            if (gv.ObjectUnderMouseCursor != null)
            {
                string selected = gv.ObjectUnderMouseCursor.ToString();
                Clipboard.SetText(selected);
                GraphViewer graphViewer = new GraphViewer();
                Graph graph = PrepareRightPanelGraph(graphViewer);
                RenderOneTransform(OneTransform.AllResult[selected], graph);
                graphViewer.Graph = graph;
            }
        }        

        private void RenderOneTransform(OneTransform one, Graph graph)
        {                
            RecursiveRender(one.Result, graph);
            RecursiveRender(one.TemplateSrc, graph);
            RecursiveRender(one.TemplateTarget, graph);
            RecursiveRender(one.Original, graph);
            RecursiveRender(one.BranchInResult, graph);
            RecursiveRender(one.BranchInOrigin, graph);

            if (one.Gen != 0)
                graph.AddEdge(one.Original.Sig, one.Result.Sig).Attr.Color = Microsoft.Msagl.Drawing.Color.Green;
           
            if (one.TemplateSrc != null)
            {
                graph.AddEdge(one.Original.Sig, one.TemplateSrc.Sig).Attr.Color = Microsoft.Msagl.Drawing.Color.Red;
                graph.AddEdge(one.Original.Sig, one.TemplateTarget.Sig).Attr.Color = Microsoft.Msagl.Drawing.Color.Yellow;
                graph.AddEdge(one.Original.Sig, one.BranchInResult.Sig).Attr.Color = Microsoft.Msagl.Drawing.Color.Blue;
                graph.AddEdge(one.Original.Sig, one.BranchInOrigin.Sig).Attr.Color = Microsoft.Msagl.Drawing.Color.Purple;
            }
            graph.Attr.LayerDirection = LayerDirection.LR;            
        }

        private void LeftPanelUnderMouseObjectChanged(object sender, ObjectUnderMouseCursorChangedEventArgs e)
        {
            MessageTextBlock.Text = "";
            GraphViewer gv = (GraphViewer)sender;
            if (gv.ObjectUnderMouseCursor != null)
            {
                string selected = gv.ObjectUnderMouseCursor.ToString();
                
                StringBuilder sb = new StringBuilder();
                if(OneTransform.AllResult.ContainsKey(selected))
                {
                    sb.AppendFormat("Gen={0} SeqNum={1} Reason={2}", 
                        OneTransform.AllResult[selected].Gen, 
                        OneTransform.AllResult[selected].SequenceNumber,
                        OneTransform.AllResult[selected].Reason);
                }
                if (OneTransform.Keymaps.ContainsKey(selected))
                {
                    Dictionary<string, Member> dict = OneTransform.Keymaps[selected];
                    foreach (var one in dict)
                        sb.AppendFormat(" |{0}={1} ", one.Key, one.Value.Sig.ToString());
                }
                MessageTextBlock.Text = sb.ToString();
            }
        }

        private void RecursiveRender(OpChain branch, Graph g)
        {
            if (branch == null)
                return;

            g.AddNode(branch.Sig);

            for(int i=0; i < branch.Operands.Length; i++)
            {
                if(branch.Operands[i].FromChain!= null)
                {
                    RecursiveRender(branch.Operands[i].FromChain, g);
                    AddEdge(g, branch.Sig, branch.Operands[i].FromChain.Sig);
                }
                else
                {
                    AddEdge(g, branch.Sig, branch.Operands[i].ShortName);
                }
            }
        }

        private void AddEdge(Graph g, string node1, string node2)
        {
            string combined = node1 + "$" + node2;
            if (EdgeCount.ContainsKey(combined))
                return;
            else
            {
                g.AddEdge(node1, node2);
                EdgeCount[combined] = 1;
            }
        }       

        private void ShowListClicked(object sender, RoutedEventArgs e)
        {
            ViewFromStartToLast();
        }

        private void TrackingSigChanged(object sender, TextChangedEventArgs e)
        {
            d.TrackingSig = txtTrackingSig.Text;
        }

        private void DebugClicked(object sender, RoutedEventArgs e)
        {
            DebugLog dl = new DebugLog();
            dl.ShowDialog();
        }
             

        private void TrackingERChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var one in SetBase.AllSets)
            {
                foreach (var er in one.Value.ERStore)
                {
                    if (er.Value.ToString() == (string)cbER.SelectedValue)
                        d.TrackingER = er.Value;
                }
            }
        }

        private void cbGenSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GraphViewer graphViewer = new GraphViewer();
            Graph graph = PrepareLeftPanelGraph(graphViewer);

            int gen = (int)cbGen.SelectedValue;

            string prev = null;
            string current = null;
            foreach (var one in OneTransform.AllResult)
            {
                if (one.Value.Gen == gen)
                {
                    //  graph.AddEdge(one.Value.Original.Sig, one.Value.Result.Sig).Attr.Color = Microsoft.Msagl.Drawing.Color.Green;
                    // graph.AddEdge(one.Value.Result.Sig, one.Value.Result.lastWeight.ToString()).Attr.Color = Microsoft.Msagl.Drawing.Color.Red;
                    current = one.Value.Result.Sig + "---" + one.Value.Result.SigWithWeight;
                    if (prev == null)
                    {
                        graph.AddNode(current);
                       
                    }
                    else
                    {
                        graph.AddEdge(prev, current).Attr.Color = Microsoft.Msagl.Drawing.Color.Green;
                    }
                    prev = current;
                }
            }

            graphViewer.Graph = graph;

           // ViewFromStartToLast();
        }
               
    }
}
