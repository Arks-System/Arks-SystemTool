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
using System.Windows.Shapes;
using System.Threading;
using System.Reflection;
using System.IO;

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
                this._UpdateArksSystemTool();
                return (true);
            }
            return (false);
        }

        private void _SpawnMainWindow()
        {
            PSO2 pso2 = new PSO2();
            MainWindow window = new MainWindow(pso2);

            window.Owner = this;
            this.Hide();
            window.ShowDialog();
        }

        private bool _UpdateAvailable()
        {
            String fullpath = Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path);
            String path = Directory.GetParent(fullpath).FullName;
            return (false);
        }

        private void _UpdateArksSystemTool()
        {
        }

    }
}
