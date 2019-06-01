using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AutoSymbol
{
    public partial class MainWindow : Window
    {
        public List<OpChain> ManualBuilds = new List<OpChain>();
        Dictionary<ManualTransform, StrToOp> OneLevelResult;

        private void LoadManualTransforms()
        {
            foreach (var one in TrainingSet.All)
            {
                cbScenario.Items.Add(one.Key);
            }

            cbFilterER.Items.Add("All");
            foreach (var one in Set.GetAllManualTransform())
            {
                cbFilterER.Items.Add(one.Key);
            }

            OneTransform.Reset();

            cbER.Items.Clear();
            foreach (var one in Set.AllSets)
            {
                foreach (var er in one.Value.ERStore)
                {
                    cbER.Items.Add(er.Value.ToString());
                }
            }
        }

        private void ProceedOneLevel(OpChain toChange)
        {
            OneLevelResult = new Dictionary<ManualTransform, StrToOp>();
            OneTransform.Reset();
            OneTransform.AddTransformWithNoSource(toChange.Sig);
            foreach(var ms in Set.GetAllManualTransform().Where(x=>x.MyType== TransformType.ERReplace))
            {
                OneLevelResult[ms] = new StrToOp();
                ms.ER.BuildERChainAtAllBranchOnce(OneLevelResult[ms], toChange);
                List<string> keys = OneLevelResult[ms].Keys.ToList();
                foreach (var str in keys)
                {
                    ER.ShortenOneChain(OneLevelResult[ms][str], OneLevelResult[ms]);
                }
            }         
        }

        private void OptimizeClicked(object sender, RoutedEventArgs e)
        {

        }

        private void FilterERChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OneLevelResult != null)
            {
                GraphViewer rightView = new GraphViewer();
                Graph graph = PrepareRightPanelGraph(rightView);
                AddOneLevelManualTransformToGraph(graph);
                rightView.Graph = graph;

                RenderManualBuildList();
            }
        }

        private void TryManualBuildClicked(object sender, RoutedEventArgs e)
        {
            ProceedOneLevel(ManualBuilds[ManualBuilds.Count - 1]);
            FilterERChanged(null, null);
        }

        private void RenderManualBuildList()
        {
            GraphViewer leftView = new GraphViewer();
            Graph graph = PrepareLeftPanelGraph(leftView);
            for(int i=0; i < ManualBuilds.Count -1; i++)
            {
                graph.AddNode(ManualBuilds[i].Sig);
                if(i!= ManualBuilds.Count-1)
                {
                    graph.AddEdge(ManualBuilds[i].Sig, ManualBuilds[i + 1].Sig).Attr.Color = Microsoft.Msagl.Drawing.Color.Green;
                }
            }

            leftView.Graph = graph;
        }

        private void AddOneLevelManualTransformToGraph(Graph graph)
        {
            string prevSig = "Start";
            string selectedKey = cbFilterER.SelectedValue.ToString();
            foreach (var one in OneLevelResult)
            {                
                if(one.Key.Key == selectedKey || selectedKey =="All" )
                {
                    foreach (var sig in one.Value.Keys)
                    {
                        graph.AddEdge(prevSig, sig).Attr.Color = Microsoft.Msagl.Drawing.Color.Green;
                        //prevSig = sig;
                        //graph.AddNode(sig);
                    }
                }
            }
        }

        private void RightPanelMouseDown_ManualBuildMode(object sender)
        {
            GraphViewer gv = (GraphViewer)sender;
            if (gv.ObjectUnderMouseCursor != null)
            {
                string selected = gv.ObjectUnderMouseCursor.ToString();
                foreach (var one in OneLevelResult)
                    foreach(var pair in one.Value)
                    {
                        if (pair.Key == selected)
                            ManualBuilds.Add(pair.Value);
                    }
            }
            RenderManualBuildList();
        }

        private void LeftPanelMouseDown_ManualBuildMode(object sender)
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

                /// Steps 1. Invoke BuildERChainAtAllBranchOnce
                /// Steps 2. Review at right panel
                /// Steps 3. Select right panel node and insert to manual build
                /// Step 4. Generate preferred signature list
                /// Step 5. Start optimization
                /// 

            }
        }
        private void ScenarioChanged(object sender, SelectionChangedEventArgs e)
        {
            MyMode = RunningMode.ManualBuild;
            ManualScenario ms = TrainingSet.All[(string)cbScenario.SelectedValue];
            ManualBuilds.Clear();
            ManualBuilds.Add(ms.StartChain);
        }
    }
}
