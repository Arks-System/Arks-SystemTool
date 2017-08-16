﻿using System;
using System.Collections.Generic;
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
            this._pso2.SetPermissions(FileSystemRights.ReadAndExecute);
        }
    }
}
