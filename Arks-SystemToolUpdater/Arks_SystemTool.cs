using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Arks_SystemToolUpdater
{
    public class Arks_SystemTool
    {
        private String _path;
        public Arks_SystemTool(String path)
        {
            this._path = path;
        }

        public bool IsRunning()
        {
            Process[] processes = Process.GetProcessesByName("Arks-SystemTool");

            return (processes.Length > 0);
        }
    }
}
