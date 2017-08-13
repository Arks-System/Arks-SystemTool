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

namespace Arks_SystemTool
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();
        }

        private void _Window_Loaded(object sender, RoutedEventArgs e)
        {
            PSO2 pso2 = new PSO2();
            MainWindow window = new MainWindow(pso2);

            window.Owner = this;
            this.Hide();
            //Thread.Sleep(3000);
            window.ShowDialog();
            this.Close();
        }
    }
}
