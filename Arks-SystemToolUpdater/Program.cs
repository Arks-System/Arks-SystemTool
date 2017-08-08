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
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
#endif
        }
    }
}
