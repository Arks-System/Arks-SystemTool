using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arks_SystemUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Arks-System Tool Updated";
#if DEBUG
            TimeSpan ts = DateTime.Now - Process.GetCurrentProcess().StartTime;
            Console.WriteLine("Total runtime: {0}s", ts.TotalMilliseconds / 1000);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
#endif
        }
    }
}
