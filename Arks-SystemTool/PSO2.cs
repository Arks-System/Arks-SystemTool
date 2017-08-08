using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arks_SystemTool
{
    public class PSO2
    {
        public String gamepath { get; set; }
        public String version { get; set; }
        public String[] binaries = {"pso2.exe",
            "pso2launcher.exe",
            "pso2download.exe",
            "pso2updater.exe" };

        public PSO2()
        {
        }
        public PSO2(String path)
        {
            this.gamepath = path;
        }
    }
}
