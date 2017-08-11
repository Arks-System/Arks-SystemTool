using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using Arks_SystemTool;
using System.IO;
using System.Diagnostics;

namespace Arks_SystemToolCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Arks-System Tool CLI";
#if DEBUG
            Console.WriteLine("Version: {0} ({1})", PSO2Tools.GetVersion(), PSO2Tools.GetGameVersion());
            Console.WriteLine("Management:\n{0}", PSO2Tools.GetManagement(PSO2Tools.ArksSystemSource));
#endif

            Management man = new Management(PSO2Tools.ArksSystemSource);
            List<Patchlist> patchlist = man.GetPatchlist();

            Console.WriteLine("Patchlist total count: {0}", patchlist.Count.ToString());
#if DEBUG
            TimeSpan ts = DateTime.Now - Process.GetCurrentProcess().StartTime;
            Console.WriteLine("\nTotal runtime: {0}s", ts.TotalMilliseconds / 1000);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
#endif
        }
    }
}
