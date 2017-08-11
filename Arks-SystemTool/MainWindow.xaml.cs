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

namespace Arks_SystemTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PSO2 _pso2;
        Timer _timer;
        public MainWindow()
        {
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("fr-CA");
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("fr-FR");
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("fr-BE");
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("fr");
            InitializeComponent();
            this._pso2 = new PSO2();
            this._timer = null;
        }

        private void _Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Check game version
            // Update according to the settings
            Thread t = new Thread(delegate ()
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    this._label_version.Content = this._pso2.GetRemoteVersion();
                }));
            });
            t.Start();
        }

        private void _EnableLaunch(object o)
        {
            if (this._pso2.IsRunning())
                return;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this._timer.Dispose();
                this._timer = null;
                this.button_launch.IsEnabled = true;
            }));
        }

        private void _button_launch_Click(object sender, RoutedEventArgs e)
        {
            this._timer = new Timer(this._EnableLaunch, this._pso2, 0, 1000);
            
            this.button_launch.IsEnabled = false;
#if !DEBUG
            if (!this._pso2.IsRunning())
                this._pso2.Launch();
            else
                MessageBox.Show("PSO2 already running.", "PSO2 already running", MessageBoxButton.OK);
#endif
        }

        private void _button_filecheck_Click(object sender, RoutedEventArgs e)
        {
            FilecheckWindow window = new FilecheckWindow();
            
            window.Owner = this;
            window.ShowDialog();
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
