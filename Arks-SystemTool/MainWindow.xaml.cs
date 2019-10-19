using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace Arks_SystemTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PSO2 _pso2;
        private Management _management;
        private Timer _timer;

        public MainWindow(PSO2 pso2)
        {
            this._management = new Management();
            this._timer = null;
            this._pso2 = pso2;
            InitializeComponent();

            this._label_version.Content = "AST v" + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            this.button_launch.IsEnabled = false;
            this._timer = new Timer(this._EnableLaunch, this._pso2, 500, 1000 * 5);
#if DEBUG
            this.Title += " [DEBUG]";
#endif
        }

        private void _Window_Loaded(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(delegate ()
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    String remoteversion = this._pso2.GetRemoteVersion().Replace("\r\n", "");
                    
                    if (remoteversion != this._pso2.GetLocalVersion().Replace("\r\n", ""))
                        this._status_label.Content = Arks_SystemTool.Properties.Resources.str_game_out_of_date;
                    else
                        this._status_label.Content = Arks_SystemTool.Properties.Resources.str_game_up_to_date;
                    this._status_label.Content += " (" + remoteversion + ")";
                }));
            });
            t.Start();
        }

        private void _CheckTranslation(bool force = false)
        {
            String source = this._management.Sources[Arks_SystemTool.Properties.Settings.Default.update_source];
            Management man = new Management(this._management.Sources[1]);
            DownloadkWindow window = new DownloadkWindow(Arks_SystemTool.Properties.Resources.title_translation_download, man.GetPatchlist(man.Bases["TranslationURL"]), man);
            String str_local = this._pso2.GetLocalVersion();

            if (!force && Arks_SystemTool.Properties.Settings.Default.current_patch_version == str_local)
                return;

            window.Owner = this;
            if ((bool)window.ShowDialog())
            {
                //String version = Requests.Get(man.GetPatchBaseURL() + @"/gameversion.ver.pat");
                this._pso2.ForceTranslationVersion(str_local);
            }
            else
            {
                MessageBox.Show(Arks_SystemTool.Properties.Resources.str_download_cancelled,
                    Arks_SystemTool.Properties.Resources.title_translation_update);
            }
        }

        private void _EnableLaunch(object o)
        {
            if (this._pso2 != null && this._pso2.IsRunning())
                return;
            try
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (this._timer != null)
                        this._timer.Dispose();
                    this.button_launch.IsEnabled = true;
                }));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private bool _CanTranslate()
        {
            String str_remote = Requests.Get("https://patch.arks-system.eu/patch_prod/translation/gameversion.ver.pat");
            String str_translation = String.IsNullOrEmpty(Arks_SystemTool.Properties.Settings.Default.current_patch_version) ? "0.0.0.0" : Arks_SystemTool.Properties.Settings.Default.current_patch_version;

						if (str_translation.Length > 32)
							str_translation = "0.0.0.0";
            Version ver_remote = new Version(String.IsNullOrEmpty(str_remote) ? "0.0.0.0" : str_remote);
            Version ver_translation = new Version(str_translation);
            Version ver_pso2 = new Version(this._pso2.GetRemoteVersion());

            return (Arks_SystemTool.Properties.Settings.Default.translate
                && ver_translation < ver_remote
                && ver_remote == ver_pso2);
        }

        private void _button_launch_Click(object sender, RoutedEventArgs e)
        {
            this.button_launch.IsEnabled = false;

            String source = this._management.Sources[Arks_SystemTool.Properties.Settings.Default.update_source];
            Management man = new Management(source);
            String censorship_file = String.Format(@"{0}\data\win32\ffbff2ac5b7a7948961212cefd4d402c", Arks_SystemTool.Properties.Settings.Default.pso2_path);
            this._timer = new Timer(this._EnableLaunch, this._pso2, 5000, 1000 * 5);

            String remote_version = man.GetRemoteVersion();
            String local_version = this._pso2.GetLocalVersion();
            
            this.button_launch.IsEnabled = false;
            if (this._pso2.IsRunning())
            {
                MessageBox.Show(Arks_SystemTool.Properties.Resources.str_pso2_already_running,
                    Arks_SystemTool.Properties.Resources.title_pso2_already_running);
                return;
            }
            if (local_version != remote_version)
            {
                if (MessageBox.Show(Arks_SystemTool.Properties.Resources.str_prompt_to_update,
                    Arks_SystemTool.Properties.Resources.title_update_required,
                    MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    if (MessageBox.Show(Arks_SystemTool.Properties.Resources.str_launch_anyway,
                        Arks_SystemTool.Properties.Resources.title_update_required,
                        MessageBoxButton.YesNo) == MessageBoxResult.No)
                        return;
                }
                else
                {
                    this._button_filecheck_Click(sender, e);
                    return;
                }
            }
            if (Arks_SystemTool.Properties.Settings.Default.remove_censorship
                && File.Exists(censorship_file))
                File.Delete(censorship_file);
            if (this._CanTranslate())
            {
                if (MessageBox.Show(Arks_SystemTool.Properties.Resources.str_translation_missmatch,
                    Arks_SystemTool.Properties.Resources.title_translation_update,
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    this._CheckTranslation();
                }
                else if (MessageBox.Show(Arks_SystemTool.Properties.Resources.str_force_translation_version,
                    Arks_SystemTool.Properties.Resources.title_translation_update,
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    this._pso2.ForceTranslationVersion(remote_version);
                }
                else
                {
                    return;
                }
            }
#if !DEBUG
            
            if (!this._pso2.IsRunning())
                this._pso2.Launch();
            else
                MessageBox.Show(Arks_SystemTool.Properties.Resources.str_pso2_already_running,
            Arks_SystemTool.Properties.Resources.title_pso2_already_running, MessageBoxButton.OK);
#endif
        }

        private void _button_filecheck_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder(4096000);
            String source = this._management.Sources[Arks_SystemTool.Properties.Settings.Default.update_source];
            Management man = new Management(source);
            List<Patchlist> patchlist = new List<Patchlist>();
            
            patchlist.AddRange(man.GetLauncherlist());
            patchlist.AddRange(man.GetPatchlist());

            DownloadkWindow window = new DownloadkWindow(Arks_SystemTool.Properties.Resources.title_filecheck, patchlist, man);
            //DownloadkWindow window = new DownloadkWindow(man.GetPatchlist(man.Bases["TranslationURL"]));
            
            window.Owner = this;
            if ((bool)window.ShowDialog())
            {
                String version = Requests.Get(man.GetPatchBaseURL() + @"gameversion.ver.pat");
                
                this._pso2.ForceGameVersion(version);
                Arks_SystemTool.Properties.Settings.Default.current_patch_version = String.Empty;
                Arks_SystemTool.Properties.Settings.Default.Save();
                Arks_SystemTool.Properties.Settings.Default.Reload();
                this._Window_Loaded(sender, e);
            }
            else
            {
                MessageBox.Show(Arks_SystemTool.Properties.Resources.str_download_cancelled,
                    Arks_SystemTool.Properties.Resources.title_filecheck);
            }
        }

        private void _button_translate_Click(object sender, RoutedEventArgs e)
        {
            this._CheckTranslation(true);
        }
        private void _button_tools_Click(object sender, RoutedEventArgs e)
        {
            ToolsWindow window = new ToolsWindow(this._pso2);

            window.Owner = this;
            window.ShowDialog();
        }
        private void _button_settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow window = new SettingsWindow();

            window.Owner = this;
            if ((bool)window.ShowDialog())
                Arks_SystemTool.Properties.Settings.Default.Reload();
        }
        private void _button_about_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow window = new AboutWindow();

            window.Owner = this;
            window.ShowDialog();
        }
    }
}
