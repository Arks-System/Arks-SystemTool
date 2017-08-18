using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;

namespace Arks_SystemToolUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Arks-System Tool Updater";
#if DEBUG
            DebugInfo();
#endif
            Arks_SystemTool arks = new Arks_SystemTool(AppDomain.CurrentDomain.BaseDirectory + "Arks-SystemTool.exe");

            //Console.WriteLine("Arks-SystemTool.exe ({0}) as {1}", arks.GetASTVersion(), arks.GetMD5());
            Console.WriteLine("User-Agent: {0}", Requests.UserAgent);

            Console.WriteLine("");

            while (arks.IsRunning())
            {
                Console.WriteLine("Waiting for Arks-SystemTool.exe to close...");
                Thread.Sleep(1500);
            }

            Console.WriteLine("Checking Updates...");
            if (arks.UpdateRequired())
            {
                Console.WriteLine("Updating...");
                arks.Update();
            }

            Console.WriteLine("Launching Arks-System Tool");
            Process.Start(arks.path);
            while (!arks.IsRunning())
            {
                Console.WriteLine("Waiting for Arks-System Tool to run...");
                Thread.Sleep(1500);
            }
#if DEBUG
            TimeSpan ts = DateTime.Now - Process.GetCurrentProcess().StartTime;
            Console.WriteLine("\nTotal runtime: {0}s", ts.TotalMilliseconds / 1000);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
#endif
        }

        static void DebugInfo()
        {
            Console.WriteLine("Architecture: " + (Environment.Is64BitOperatingSystem ? "x64" : "x86"));
            Console.WriteLine("Processor count: " + Environment.ProcessorCount);
            Console.WriteLine("Base Directory: " + AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine("Full path: " + Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
            Console.WriteLine("");
        }
    }
}
