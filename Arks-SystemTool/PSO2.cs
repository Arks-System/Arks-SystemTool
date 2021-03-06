﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Arks_SystemTool
{
    public class PSO2
    {
        public static String SegaSource { get { return ("http://download.pso2.jp/"); } }
        public static String ArksSystemSource { get { return ("https://patch.arks-system.eu/"); } }
        public String gamepath { get; set; }
        public String version { get; set; }
        public String gameversion { get; set; }
        public String[] binaries = {"pso2.exe",
            "pso2launcher.exe",
            "pso2download.exe",
            "pso2updater.exe" };

        public PSO2()//: this(Arks_SystemTool.Properties.Settings.Default.pso2_path)
        {
            if (String.IsNullOrEmpty(Arks_SystemTool.Properties.Settings.Default.pso2_path))
            {
                this.gamepath = this.DetectGamePath();
                this.gamepath = this.gamepath.Replace(@"\\", @"\") + "\\";
                Arks_SystemTool.Properties.Settings.Default.pso2_path = this.gamepath;
                Arks_SystemTool.Properties.Settings.Default.Save();
                Arks_SystemTool.Properties.Settings.Default.Reload();
            }
            else
            {
                this.gamepath = Arks_SystemTool.Properties.Settings.Default.pso2_path;
            }
        }
        /*public PSO2(String path)
        {
            this.gamepath = path;

            if (path != Arks_SystemTool.Properties.Settings.Default.pso2_path)
            {
                Arks_SystemTool.Properties.Settings.Default.pso2_path = path;
                Arks_SystemTool.Properties.Settings.Default.Save();
                Arks_SystemTool.Properties.Settings.Default.Reload();
            }
        }*/

        public void Launch(bool force = false)
        {
            String pso2 = this.gamepath + @"\pso2.exe";

            if (force || (File.Exists(pso2) && !this.IsRunning()))
            {
                Environment.SetEnvironmentVariable("-pso2", "+0x01e3f1e9");
                Process.Start(pso2, "+0x33aca2b9");
            }
        }

        public String GetRemoteVersion(String remoteURL = "http://download.pso2.jp")
        {
            Management man = new Management();
            String url = String.Format("{0}gameversion.ver.pat", man.GetPatchBaseURL());
            String version = Requests.Get(url);
            return (version);
        }

        public String GetLocalVersion()
        {
            String path = this.gamepath + @"\gameversion.ver";
            if (File.Exists(path))
            {
                using (var version = new StreamReader(path))
                {
                    return (version.ReadToEnd().Replace("\r\n", ""));
                }
            }
            return (String.Empty);
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
                while (String.IsNullOrEmpty(path))
                {
                    using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                    {
                        MessageBox.Show(Arks_SystemTool.Properties.Resources.str_select_create_pso2_bin,
                            Arks_SystemTool.Properties.Resources.title_pso2_bin_not_found,
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        dialog.ShowNewFolderButton = true;
                        dialog.Description = Arks_SystemTool.Properties.Resources.str_browse_pso2_bin;
                        System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                        if (result == System.Windows.Forms.DialogResult.OK)
                        {
                            path = dialog.SelectedPath;
                        }
                        else
                        {
                            if (MessageBox.Show(Arks_SystemTool.Properties.Resources.str_pso2_bin_not_found,
                                Arks_SystemTool.Properties.Resources.title_pso2_bin_not_found,
                                MessageBoxButton.OKCancel,
                                MessageBoxImage.Error) == MessageBoxResult.Cancel)
                                Environment.Exit(-1);
                        }
                    }
                }

                if (!path.EndsWith(@"\pso2_bin"))
                {
                    if (!Directory.Exists(path + @"\pso2_bin\"))
                        Directory.CreateDirectory(path + @"\pso2_bin\");
                    path = path + @"\pso2_bin";
                }
                pso2 = path + @"\pso2.exe";
                
                if (!String.IsNullOrEmpty(path) && !File.Exists(pso2))
                {
                    MessageBoxResult mb_result = MessageBox.Show(Arks_SystemTool.Properties.Resources.str_prompt_create_pso2_bin,
                        Arks_SystemTool.Properties.Resources.title_pso2_bin_not_found,
                        MessageBoxButton.YesNo);

                    if (mb_result == MessageBoxResult.Yes)
                    {
                        Management man = new Management();

                        this.gamepath = Directory.GetParent(pso2).FullName;
                        if (!Directory.Exists(this.gamepath))
                            Directory.CreateDirectory(this.gamepath + @"\data\win32\");
                        Requests.Download(man.GetPatchBaseURL() + "/pso2.exe.pat", this.gamepath + @"\pso2.exe");
                    }
                    else if (mb_result == MessageBoxResult.No)
                    {
                        MessageBox.Show(Arks_SystemTool.Properties.Resources.str_exit_no_pso2,
                            Arks_SystemTool.Properties.Resources.title_pso2_bin_not_found,
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        Environment.Exit(0);
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

        public void ForceGameVersion(String version)
        {
            Arks_SystemTool.Properties.Settings.Default.current_patch_version = version;
            Arks_SystemTool.Properties.Settings.Default.Save();
            Arks_SystemTool.Properties.Settings.Default.Reload();
            using (var version_file = new StreamWriter(this.gamepath + @"\gameversion.ver"))
            {
                version_file.Write(version + "\r\n");
            }
        }

        public void ForceTranslationVersion(String version)
        {
            Arks_SystemTool.Properties.Settings.Default.current_patch_version = version;
            Arks_SystemTool.Properties.Settings.Default.Save();
            Arks_SystemTool.Properties.Settings.Default.Reload();
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

        public void SetPermissions(FileSystemRights rights = FileSystemRights.FullControl)
        {
            foreach (String e in this.binaries)
            {
                String path = Arks_SystemTool.Properties.Settings.Default.pso2_path + @"\" + e;
                FileInfo finfo = new FileInfo(path);
                FileSecurity fsecurity = finfo.GetAccessControl();
                fsecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), rights, AccessControlType.Allow));
                fsecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null), rights, AccessControlType.Allow));
                fsecurity.AddAccessRule(new FileSystemAccessRule(Environment.UserName, rights, AccessControlType.Allow));
                finfo.SetAccessControl(fsecurity);
            }
        }
    }
}
