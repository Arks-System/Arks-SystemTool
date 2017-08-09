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

            Management man = new Management();

            //Console.WriteLine("Version: {0}", PSO2.GetVersion());
            /*
            using (WebClient web = new WebClient())
            {
                web.Headers.Add("User-Agent: AQUA_HTTP");
                Console.WriteLine("Version: {0}", web.DownloadString("http://download.pso2.jp/patch_prod/patches/version.ver"));
            }
            */
#if DEBUG
            TimeSpan ts = DateTime.Now - Process.GetCurrentProcess().StartTime;
            Console.WriteLine("Total runtime: {0}s", ts.TotalMilliseconds / 1000);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
#endif
        }
    }
}
