using AutoSymbol.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSymbolTestConsole
{
    public class TestBase
    {
        public void Log(List<OpNode> result)
        {
            for (int i = 0; i < result.Count; i++)
            {
                Trace.TraceInformation(i.ToString() + " " + result[i].Sig);
            }

        }
    }
}
