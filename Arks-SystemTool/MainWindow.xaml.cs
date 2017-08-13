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
        }

        private void _Window_Loaded(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(delegate ()
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    String remoteversion = this._pso2.GetRemoteVersion().Replace("\r\n", "");

                    this._label_version.Content = remoteversion;
                    if (remoteversion != this._pso2.GetLocalVersion().Replace("\r\n", ""))
                        this._status_label.Content = Arks_SystemTool.Properties.Resources.str_game_out_of_date;
                    else
                        this._status_label.Content = Arks_SystemTool.Properties.Resources.str_game_up_to_date;
                }));
            });
            t.Start();
        }

        private void _CheckTranslation(bool force = false)
        {
            String source = this._management.Sources[Arks_SystemTool.Properties.Settings.Default.update_source];
            Management man = new Management(this._management.Sources[1]);
            DownloadkWindow window = new DownloadkWindow(Arks_SystemTool.Properties.Resources.title_translation_update, man.GetPatchlist(man.Bases["TranslationURL"]));

            if (!force || Arks_SystemTool.Properties.Settings.Default.current_patch_version == this._pso2.GetLocalVersion())
                return;

            window.Owner = this;
            if ((bool)window.ShowDialog())
            {
                String version = Requests.Get(man.GetPatchBaseURL() + @"/gameversion.ver.pat");
                this._pso2.ForceTranslationVersion(version);
            }
            else
            {
                MessageBox.Show(Arks_SystemTool.Properties.Resources.str_download_cancelled,
                    Arks_SystemTool.Properties.Resources.title_translation_update);
            }
        }

        private void _EnableLaunch(object o)
        {
            if (this._pso2.IsRunning())
                return;
            try
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    this._timer.Dispose();
                    this._timer = null;
                    this.button_launch.IsEnabled = true;
                }));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void _button_launch_Click(object sender, RoutedEventArgs e)
        {
            String source = this._management.Sources[Arks_SystemTool.Properties.Settings.Default.update_source];
            Management man = new Management(source);
            String censorship_file = String.Format(@"{0}\data\win32\ffbff2ac5b7a7948961212cefd4d402c", Arks_SystemTool.Properties.Settings.Default.pso2_path);
            this._timer = new Timer(this._EnableLaunch, this._pso2, 5000, 1000 * 5);

            String remote_version = man.GetRemoteVersion();
            String local_version = this._pso2.GetLocalVersion();
            
            this.button_launch.IsEnabled = false;
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
                }
            }
            if (Arks_SystemTool.Properties.Settings.Default.remove_censorship && File.Exists(censorship_file))
                File.Delete(censorship_file);
            if (Arks_SystemTool.Properties.Settings.Default.translate
                && Arks_SystemTool.Properties.Settings.Default.current_patch_version != remote_version)
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
            String source = this._management.Sources[Arks_SystemTool.Properties.Settings.Default.update_source];
            Management man = new Management(source);
            List<Patchlist> patchlist = new List<Patchlist>();

            //patchlist += man.GetPatchlist();
            patchlist.AddRange(man.GetPatchlist());
            patchlist.AddRange(man.GetLauncherlist());

            DownloadkWindow window = new DownloadkWindow(Arks_SystemTool.Properties.Resources.title_filecheck, patchlist);
            //DownloadkWindow window = new DownloadkWindow(man.GetPatchlist());
            //DownloadkWindow window = new DownloadkWindow(man.GetPatchlist(man.Bases["TranslationURL"]));
            
            window.Owner = this;
            if ((bool)window.ShowDialog())
            {
                String version = Requests.Get(man.GetPatchBaseURL() + @"/gameversion.ver.pat");
                
                this._pso2.ForceGameVersion(version);
                Arks_SystemTool.Properties.Settings.Default.current_patch_version = String.Empty;
                Arks_SystemTool.Properties.Settings.Default.Save();
                Arks_SystemTool.Properties.Settings.Default.Reload();
            }
            else
            {
                MessageBox.Show(Arks_SystemTool.Properties.Resources.str_download_cancelled,
                    Arks_SystemTool.Properties.Resources.title_filecheck);
            }
            patchlist.Clear();
        }

        private void _button_translate_Click(object sender, RoutedEventArgs e)
        {
            this._CheckTranslation(true);
        }
        private void _button_tools_Click(object sender, RoutedEventArgs e)
        {
            ToolsWindow window = new ToolsWindow();

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
    }
}
