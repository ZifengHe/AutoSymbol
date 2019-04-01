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

namespace AutoSymbol
{
    using OpDict = Dictionary<string, OpChain>;
    public static class UIData
    {
        public static OpDict ItemMap;
        public static List<string> AllItems;
    }
    public partial class MainWindow : Window
    {

        public MainWindow()
        {            
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private static Dictionary<string, int> EdgeCount = new Dictionary<string, int>();

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void Rebind()
        {
            cbList.Items.Clear();
            foreach (var one in UIData.AllItems)
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
            Benchmark.RunOne();
            Rebind();
        }

        private void CbList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string sig = (string) cbList.SelectedValue;

            EdgeCount.Clear();
            GraphViewer graphViewer = new GraphViewer();
            graphViewer.BindToPanel(ContentPanel);
            graphViewer.ObjectUnderMouseCursorChanged += GraphViewer_ObjectUnderMouseCursorChanged;
            Graph graph = new Graph();

            switch ((string)(cbOptions.SelectedValue))
            {
                case "Transform":
                    RenderOneTransform(OneTransform.AllResult[sig], graph);
                    break;
                case "Proof":
                    RenderOneProof();
                    break;
                default:
                    break;
            }

            graphViewer.Graph = graph; // throws exception
        }

        private void RenderOneProof()
        {
            /// 
        }
        private void RenderOneTransform(OneTransform one, Graph graph)
        {                
            RecursiveRender(one.Result, graph);
            RecursiveRender(one.TemplateSrc, graph);
            RecursiveRender(one.TemplateTarget, graph);
            RecursiveRender(one.Original, graph);
            RecursiveRender(one.BranchInResult, graph);
            RecursiveRender(one.BranchInOrigin, graph);

            graph.AddEdge(one.Original.Sig, one.Result.Sig).Attr.Color = Microsoft.Msagl.Drawing.Color.Green;
            graph.AddEdge(one.Original.Sig, one.TemplateSrc.Sig).Attr.Color = Microsoft.Msagl.Drawing.Color.Red ;
            graph.AddEdge(one.Original.Sig, one.TemplateTarget.Sig).Attr.Color = Microsoft.Msagl.Drawing.Color.Yellow;
            graph.AddEdge(one.Original.Sig, one.BranchInResult.Sig).Attr.Color = Microsoft.Msagl.Drawing.Color.Blue;
            graph.AddEdge(one.Original.Sig, one.BranchInOrigin.Sig).Attr.Color = Microsoft.Msagl.Drawing.Color.Purple;

            graph.Attr.LayerDirection = LayerDirection.LR;            
        }

        private void GraphViewer_ObjectUnderMouseCursorChanged(object sender, ObjectUnderMouseCursorChangedEventArgs e)
        {
            MessageTextBlock.Text = "";
            GraphViewer gv = (GraphViewer)sender;
            if (gv.ObjectUnderMouseCursor != null)
            {
                string selected = gv.ObjectUnderMouseCursor.ToString();
                if (OneTransform.Keymaps.ContainsKey(selected))
                {
                    Dictionary<string, Member> dict = OneTransform.Keymaps[selected];
                    StringBuilder sb = new StringBuilder();
                    foreach (var one in dict)
                        sb.AppendFormat(" |{0}={1}", one.Key, one.Value.Sig.ToString());
                    MessageTextBlock.Text = sb.ToString();
                }                
            }
        }

        private void RecursiveRender(OpChain branch, Graph g)
        {
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

       

       
    }
}
