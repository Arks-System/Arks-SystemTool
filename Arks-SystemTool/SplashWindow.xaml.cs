﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Net;
using System.Diagnostics;

namespace Arks_SystemTool
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("fr-CA");
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("fr-FR");
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("fr-BE");
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("fr");
            InitializeComponent();
            Arks_SystemTool.Properties.Settings.Default.update_source = 0;
            Arks_SystemTool.Properties.Settings.Default.Save();
            Arks_SystemTool.Properties.Settings.Default.Reload();
        }

        private async void _Window_Rendered(object sender, EventArgs e)
        {
            bool updateavailable = await this._CheckUpdate();
            if (!updateavailable)
            {
                this._SpawnMainWindow();
            }
            this.Close();
        }

        private async Task<bool> _CheckUpdate()
        {
            await Task.Delay(700);
            if (this._UpdateAvailable())
            {
                MessageBox.Show(Arks_SystemTool.Properties.Resources.str_tool_update_available,
                    Arks_SystemTool.Properties.Resources.title_tool_update_available,
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
#if !DEBUG
                this._UpdateArksSystemTool();
                return (true);
#endif
            }
            return (false);
        }

        private void _SpawnMainWindow()
        {
            PSO2 pso2 = null;
            try
            { 
                pso2 = new PSO2();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
            MainWindow window = new MainWindow(pso2);

            this.Hide();
            window.ShowDialog();
        }

        private bool _UpdateAvailable()
        {
            String fullpath = Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path);
            String path = Directory.GetParent(fullpath).FullName;

            try
            {
                String[] r = Requests.Get("http://tool.arks-system.eu/repository/versions.txt").Split('\n');

                foreach (String e in r)
                {
                    String[] toollist = e.Split('=');
                    if (toollist[0] == "ProductVersion")
                    {
                        Version remote = new Version(toollist[1]);
                        //Version local = Assembly.GetExecutingAssembly().GetName().Version;
                        Version local = this._GetProductVersion();

                        return (remote > local);
                    }
                }
            }
            catch (WebException we)
            {
                MessageBox.Show(we.Message);
            }
            return (false);
        }

        private Version _GetProductVersion()
        {
            return (new Version(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion));
        }

        private void _UpdateArksSystemTool()
        {
            String fullpath = Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path);
            String folderpath = Directory.GetParent(fullpath).FullName;
            String updater = folderpath + @"\Arks-SystemToolUpdater.exe";

            try
            {
                Requests.Download("http://tool.arks-system.eu/repository/Arks-SystemToolUpdater.exe", updater);
                Process.Start(updater);
            }
            catch (WebException we)
            {
                Console.WriteLine(we.Message);
            }
        }
    }
}
