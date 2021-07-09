using MathGen;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MathGenUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private static Dictionary<string, int> EdgeCount = new Dictionary<string, int>();

        private void DisplayOpTreeTemplates(object sender, RoutedEventArgs e)
        {
            OpTree.CreateTemplates(4);
            GraphViewer graphViewer = new GraphViewer();
            Graph graph = PrepareLeftPanelGraph(graphViewer);
            
            for (int i = 0; i< OpTree.AllTemplates.Count; i++)
            {
                string suffix = string.Format("[{0}]", i);
                OpTree.AllTemplates[i].AssignVisualId();
                RecursiveRenderByIdCounter(OpTree.AllTemplates[i].Root, graph, suffix );
            }
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

        private void RecursiveRenderByIdCounter(OpNode node, Graph g, string suffix)
        {
            if (node == null)
                return;

            g.AddNode(node.VisualId.ToString()+suffix);

            if(node.Left != null)
            {
                RecursiveRenderByIdCounter(node.Left, g, suffix);
                AddEdge(g, node.VisualId.ToString() +suffix, node.Left.VisualId.ToString()+ suffix);
            }
          if(node.Right != null)
            {
                RecursiveRenderByIdCounter(node.Right, g, suffix);
                AddEdge(g, node.VisualId.ToString()+suffix, node.Right.VisualId.ToString()+suffix);
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

        private void LeftPanelMouseDown(object sender, MsaglMouseEventArgs e)
        {

        }

        private void LeftPanelUnderMouseObjectChanged(object sender, ObjectUnderMouseCursorChangedEventArgs e)
        {
            MessageTextBlock.Text = "";
            GraphViewer gv = (GraphViewer)sender;
            if (gv.ObjectUnderMouseCursor != null)
            {
                string selected = gv.ObjectUnderMouseCursor.ToString();
            }
        }
    }
}
