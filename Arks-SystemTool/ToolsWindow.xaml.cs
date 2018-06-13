using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
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
    /// Interaction logic for ToolsWindow.xaml
    /// </summary>
    public partial class ToolsWindow : Window
    {
        private PSO2 _pso2;
        public ToolsWindow(PSO2 pso2)
        {
            InitializeComponent();
            this._pso2 = pso2;
        }

        private void button_ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void button_fix_perm_Click(object sender, RoutedEventArgs e)
        {
            //this._pso2.SetPermissions(FileSystemRights.Modify);
            this._pso2.SetPermissions(FileSystemRights.Modify);
            MessageBox.Show(Arks_SystemTool.Properties.Resources.str_permissions_fixed,
                Arks_SystemTool.Properties.Resources.title_permissions_fixed,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void button_clean_gg_Click(object sender, RoutedEventArgs e)
        {
            String path = Arks_SystemTool.Properties.Settings.Default.pso2_path + @"\GameGuard\";

            if (Directory.Exists(path))
            {
                foreach (var f in Directory.GetFiles(path))
                {
                    if (!f.EndsWith(".sys") && File.Exists(f))
                        File.Delete(f);
                }
                //Directory.Delete(path, true);
                //Directory.CreateDirectory(path);
            }
            MessageBox.Show(Arks_SystemTool.Properties.Resources.str_clean_gg,
                Arks_SystemTool.Properties.Resources.title_clean_gg,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
