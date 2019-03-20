﻿using System;
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
    public static class UIData
    {
        public static Dictionary<string, OpChain> ItemMap;
        public static List<string> AllItems;
    }
    public partial class MainWindow : Window
    {

        public MainWindow()
        {            
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void Rebind()
        {
            cbList.Items.Clear();
            foreach (var one in UIData.AllItems)
                cbList.Items.Add(one);
        }

        private void RunClicked(object sender, RoutedEventArgs e)
        {
            // new Builder().BuildPattern0();
            Benchmark.RunAll();
            Rebind();
        }

        private void CbList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string sig = (string) cbList.SelectedValue;
            RenderOneTransform(OneTransform.All[sig]);
        }

        private void RenderOneTransform(OneTransform one)
        {
            GraphViewer graphViewer = new GraphViewer();
            graphViewer.BindToPanel(ContentPanel);
            Graph graph = new Graph();

            RecursiveRender(one.Result, graph);
            graph.Attr.LayerDirection = LayerDirection.LR;
            graphViewer.Graph = graph; // throws exception
        }

        private void RecursiveRender(OpChain branch, Graph g)
        {
            g.AddNode(branch.Sig);

            for(int i=0; i < branch.Operands.Length; i++)
            {
                if(branch.Operands[i].FromChain!= null)
                {
                    RecursiveRender(branch.Operands[i].FromChain, g);
                    g.AddEdge(branch.Sig, branch.Operands[i].FromChain.Sig);
                }
                else
                {
                    g.AddEdge(branch.Sig, branch.Operands[i].ShortName);
                }
            }

        }
    }
}
