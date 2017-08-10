using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
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

namespace Arks_SystemTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void _EnableLaunch(object o, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.button_launch.IsEnabled = true;
            }));
        }

        private void _button_launch_Click(object sender, RoutedEventArgs e)
        {
            Timer timer = new Timer(5000f);

            timer.Elapsed += new ElapsedEventHandler(this._EnableLaunch);
            timer.Start();
            this.button_launch.IsEnabled = false;
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

        private void _Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Check game version
            // Update according to the settings
        }
    }
}
