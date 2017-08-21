using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

namespace Arks_SystemTool
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            this._label_version.Content = "Arks-System Tool v" + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
        }

        private void _site_double_click(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://arks-system.eu");
        }

        private void _site_tool_double_click(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://arks-system.eu/tool");
        }

        private void _Close_button(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
