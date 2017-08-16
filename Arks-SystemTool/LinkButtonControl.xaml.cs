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
using System.Diagnostics;

namespace Arks_SystemTool
{
    /// <summary>
    /// Interaction logic for LinkButtonControl.xaml
    /// </summary>
    public partial class LinkButtonControl : UserControl
    {
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(String), typeof(LinkButtonControl));
        public static readonly DependencyProperty LinkProperty = DependencyProperty.Register("Link", typeof(String), typeof(LinkButtonControl));

        public String Label
        {
            get
            {
                return this.GetValue(LabelProperty) as String;
            }
            set
            {
                this._button.Content = value;
                this.SetValue(LabelProperty, value);
            }
        }
        public String Link {
            get
            {
                return this.GetValue(LinkProperty) as String;
            }
            set
            {
                this.SetValue(LabelProperty, value);
            }
        }
        public LinkButtonControl()
        {
            InitializeComponent();
        }

        private void _Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(this.Link))
            {
                Process.Start(this.Link);
            }
        }

        private void _Loaded(object sender, RoutedEventArgs e)
        {

            this._button.Content = Label;
        }
    }
}
