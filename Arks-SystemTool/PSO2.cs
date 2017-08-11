using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;

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
            if (String.IsNullOrEmpty(Arks_SystemTool.Properties.Settings.Default.pso2_path))
            {
                this.gamepath = String.Format(@"{0}\pso2_bin\", this.DetectGamePath());
                this.gamepath = this.gamepath.Replace(@"\\", @"\");
                Arks_SystemTool.Properties.Settings.Default.pso2_path = this.gamepath;
                Arks_SystemTool.Properties.Settings.Default.Save();
                Arks_SystemTool.Properties.Settings.Default.Reload();
            }
            else
            {
                this.gamepath = Arks_SystemTool.Properties.Settings.Default.pso2_path;
            }
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

        public String DetectGamePath()
        {
            String pso2 = String.Empty;
            String path = String.Empty;

            while (String.IsNullOrEmpty(path))
            {
                if (Environment.Is64BitOperatingSystem)
                    path = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\http://pso2.jp/appid/release_is1", "InstallLocation", String.Empty) as String;
                else
                    path = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\http://pso2.jp/appid/release_is1", "InstallLocation", String.Empty) as String;
                if (String.IsNullOrEmpty(path))
                {
                    using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                    {
                        dialog.ShowNewFolderButton = true;
                        System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                        if (result == System.Windows.Forms.DialogResult.OK)
                        {
                            path = result.ToString();
                        }
                    }
                }

                pso2 = String.Format(@"{0}\pso2_bin\pso2.exe", path);
                if (!String.IsNullOrEmpty(path) && !File.Exists(pso2))
                {
                    MessageBoxResult mb_result = MessageBox.Show("Create?", "No game detected", MessageBoxButton.YesNo);

                    if (mb_result == MessageBoxResult.Yes)
                    {
                        if (!Directory.Exists(Directory.GetParent(pso2).FullName))
                            Directory.CreateDirectory(Directory.GetParent(pso2).FullName);
                        this.gamepath = Directory.GetParent(pso2).FullName;
                    }
                    else if (mb_result == MessageBoxResult.No)
                    {
                        return (String.Empty);
                    }
                }
                else if (!String.IsNullOrEmpty(path) && File.Exists(pso2))
                {
                    this.gamepath = Directory.GetParent(pso2).FullName;
                }
            }
            return (path);
        }

        public bool IsRunning()
        {
            Process[] pso2 = Process.GetProcessesByName("pso2");

            return (pso2.Length > 0);
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
