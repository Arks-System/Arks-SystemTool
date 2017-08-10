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

namespace Arks_SystemTool
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void _button_ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;

            Arks_SystemTool.Properties.Settings.Default.pso2_path = this.textBox_path.Text;
            Arks_SystemTool.Properties.Settings.Default.remove_censorship = (bool)this.checkBox_censorship.IsChecked;
            Arks_SystemTool.Properties.Settings.Default.translate = (bool)this.checkBox_translate.IsChecked;
            Arks_SystemTool.Properties.Settings.Default.keep_enemy_jpnames = (bool)this.checkBox_keep_jp_ennemy.IsChecked;
            Arks_SystemTool.Properties.Settings.Default.keep_et_jpnames = (bool)this.checkBox_keep_jp_et.IsChecked;
            Arks_SystemTool.Properties.Settings.Default.clean_gg_atlaunch = (bool)this.checkBox_deletegg_atlaunch.IsChecked;
            Arks_SystemTool.Properties.Settings.Default.Save();
            Arks_SystemTool.Properties.Settings.Default.Reload();

            this.Close();
        }

        private void _button_reset_Click(object sender, RoutedEventArgs e)
        {
            Arks_SystemTool.Properties.Settings.Default.Reset();
            Arks_SystemTool.Properties.Settings.Default.Reload();
            this._Window_Loaded(sender, e);
        }

        private void _checked_translate(object sender, RoutedEventArgs e)
        {
            this.checkBox_keep_jp_et.IsEnabled = (bool)this.checkBox_translate.IsChecked;
            this.checkBox_keep_jp_ennemy.IsEnabled = (bool)this.checkBox_translate.IsChecked;
        }

        private void _Window_Loaded(object sender, RoutedEventArgs e)
        {
            Properties.Settings settings = Arks_SystemTool.Properties.Settings.Default;

            this.SizeToContent = SizeToContent.Height;

            this.checkBox_censorship.IsChecked = settings.remove_censorship;
            this.checkBox_translate.IsChecked = settings.translate;
            this.checkBox_keep_jp_ennemy.IsChecked = settings.keep_enemy_jpnames;
            this.checkBox_keep_jp_et.IsChecked = settings.keep_et_jpnames;
            this.checkBox_deletegg_atlaunch.IsChecked = settings.clean_gg_atlaunch;

            this.textBox_path.Text = settings.pso2_path;

            this.checkBox_keep_jp_et.IsEnabled = (bool)this.checkBox_translate.IsChecked;
            this.checkBox_keep_jp_ennemy.IsEnabled = (bool)this.checkBox_translate.IsChecked;
        }
    }
}
