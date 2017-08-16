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
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        public String log { get { return (this._textbox_output.Text); } }
        
        public LogWindow()
        {
            InitializeComponent();
        }

        public void AppendLine(String str, int count = -1)
        {
            this._AppendLine(str, count);
        }

        private void _AppendLine(String str, int count)
        {
            // this._textbox_output.Text += str;
            // this._textbox_output.Text += "\r\n";
            if (count > 0)
                this.Title = Arks_SystemTool.Properties.Resources.title_log + " (" + count + ")";
            this._textbox_output.AppendText(str + "\r\n");
            this._textbox_output.ScrollToEnd();
        }
    }
}
