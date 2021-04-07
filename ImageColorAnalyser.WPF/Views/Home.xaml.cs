using ImageColorAnalyser.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace ImageColorAnalyser.WPF.Views
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        public Home()
        {
            InitializeComponent();
            this.DataContext = new HomeViewModel();
            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ClearTempFiles();


        }
        private void ClearTempFiles() 
        {
            try
            {
                var imgPath = @"C:\Program Files (x86)\Image Analyser";
                var files = Directory.GetFiles(imgPath);
                foreach (var x in files)
                {
                    File.Delete(x);
                }
            }
            catch (Exception)
            {

            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ClearTempFiles();
        }
    }
}
