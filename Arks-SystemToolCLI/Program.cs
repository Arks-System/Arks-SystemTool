using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using Arks_SystemTool;
using System.IO;

namespace Arks_SystemToolCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Arks-System Tool CLI";

            Console.WriteLine("Version: {0}", PSO2.GetVersion());
#if DEBUG
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
#endif
        }
    }
}
