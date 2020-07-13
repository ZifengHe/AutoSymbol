using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbolTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //new NTest().ProveTwoPlusXPlusThree();
            new RTest().ProveAPlusBSquare();

            Console.WriteLine("Finished Successfully!");
            Console.ReadLine();
        }
    }

    public class Assert
    {
        public static void IsTrue(bool b)
        {
            if (b == false)
                Debugger.Break();
        }
    }
}
