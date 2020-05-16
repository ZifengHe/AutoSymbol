using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbol.Core
{
    
    public class Symbol
    {
        public static int RandomCounter = 0;
        public string ShortName;

        public Symbol()
        {
            // Temporary name
            this.ShortName = NextRandomName();
        }

        public Symbol(string name)
        {
            this.ShortName = name;
        }

        public static string NextRandomName()
        {
            string ret = "T" + RandomCounter.ToString();
            RandomCounter++;
            return ret;
        }
    }


   
    

   
   




}
