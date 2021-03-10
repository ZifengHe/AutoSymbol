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

namespace RiemannZeta
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
           
            InitializeComponent();

            //Zeta.PrintPolynomial();

            Zeta.OptimizeOneTemplate(Zeta.TemplateFour,
                start: 0,
                total: 30);

            Zeta.OptimizeSingleFunction(Zeta.f4, 
                start: 0,
                total:3000,
                optNum:7,
                round:100000);

                Zeta.FindCoefficient(Zeta.f1,1000);

            List<double> d = Zeta.LoadZeros();
            List<int> x = Prime.GeneratePrimesNaive(100000);

        }
    }
}
