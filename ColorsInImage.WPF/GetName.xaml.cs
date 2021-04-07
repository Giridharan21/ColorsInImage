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

namespace ColorsInImage.WPF
{
    /// <summary>
    /// Interaction logic for GetName.xaml
    /// </summary>
    public partial class GetName : Window
    {
        public GetName()
        {
            InitializeComponent();
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            var str  = ColorTextBox.Text;
            if (string.IsNullOrWhiteSpace(str))
            {
                alert.Text = "Enter title";
                return;
            }
            App.color = str;
            this.Close();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public static new bool? Show()
        {
            var s = new GetName();
            s.ColorTextBox.Focus();
            return s.ShowDialog();
        }

        private void ColorTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                ok_Click(sender, null);
            }
        }
    }
}
