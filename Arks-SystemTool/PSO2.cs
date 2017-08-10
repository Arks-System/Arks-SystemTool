using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Arks_SystemTool
{
    public class PSO2
    {
        public String gamepath { get; set; }
        public String version { get; set; }
        public String gameversion { get; set; }
        public String[] binaries = {"pso2.exe",
            "pso2launcher.exe",
            "pso2download.exe",
            "pso2updater.exe" };

        public PSO2(): this(Arks_SystemTool.Properties.Settings.Default.pso2_path)
        {
        }
        public PSO2(String path)
        {
            this.gamepath = path;

            if (path != Arks_SystemTool.Properties.Settings.Default.pso2_path)
            {
                Arks_SystemTool.Properties.Settings.Default.pso2_path = path;
                Arks_SystemTool.Properties.Settings.Default.Save();
                Arks_SystemTool.Properties.Settings.Default.Reload();
            }
        }

        public void Launch(bool force = false)
        {
            String pso2 = this.gamepath + @"\pso2.exe";

            if (File.Exists(pso2) && !this.IsRunning())
            {
                Environment.SetEnvironmentVariable("-pso2", "+0x01e3f1e9");
                Process.Start(pso2, "+0x33aca2b9");
                Thread.Sleep(2000);
            }
        }

        public String GetRemoteVersion(String remoteURL = "http://download.pso2.jp")
        {
            Management man = new Management();
            String url = String.Format("{0}gameversion.ver.pat", man.GetPatchBaseURL());
            String version = Requests.Get(url);
            return (version);
        }

        public bool IsRunning()
        {
            return (Process.GetProcessesByName("pso2.exe").Length > 0);
        }

        public void KillGame()
        {
            foreach (Process proc in Process.GetProcessesByName("pso2.exe"))
            {
                proc.Kill();
            }
        }
    }
}
