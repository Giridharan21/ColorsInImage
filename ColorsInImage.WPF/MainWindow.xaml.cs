using ColorsLib.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColorsInImage.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        public ProcessImage ImagePage { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ImagePage = new ProcessImage();
            ParentFrame.Navigate(ImagePage);
            
            //canvas.EditingMode = InkCanvasEditingMode.None;
            //ProcessedImage.Source = new BitmapImage(new Uri(@"C:\Users\844617\source\repos\ColorsInImage\ColorsInImage.WPF\uml.png"));
        }
        
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            ImagePage.canvas.Children.Clear();
            ImagePage = new ProcessImage();
            var img = new System.Windows.Controls.Image();
            img.Name = "ProcessedImage";
            img.Height = ImagePage.canvas.Height;
            img.Width = ImagePage.canvas.Width;
            ImagePage.canvas.Children.Add(img);
            ParentFrame.Navigate(ImagePage);
            var win = new MainWindow();
            win.Show();
            this.Close();

        }
        

        private void SelectImage_ClickAsync(object sender, RoutedEventArgs e)
        {
            
            var dia = new OpenFileDialog();
            dia.Filter = "Image Files|*.jpg;*.png;*.bmp;*.jpeg; " ;
            dia.Title = "Select an Image";
            var res = dia.ShowDialog();
            if (res == false && string.IsNullOrWhiteSpace(dia.FileName))
            {
                MessageBox.Show("Select an image to Process", "Alert");
                return;
            }
            var temp = dia.FileName.Split('.')[1].ToLower();
            if (!(temp == "png" || temp == "jpg" || temp == "jpeg"))
            {
                MessageBox.Show("Select only Image file (*.jpeg, *.jpg, *.png )", "Alert");
                return;
            }
            ImagePage.SelectedImage = dia.FileName;
            ImagePage.SelectedImageTextBlock.Text = "Selected Image : " + ImagePage.SelectedImage;
            ImagePage.setImage(dia.FileName);
            

        }

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            if(ImagePage.img == null)
            {
                MessageBox.Show("Image is not processed yet!.", "alert");
                return;
            }
            var dia = new SaveFileDialog();
            dia.Title = "Select a location";
            dia.Filter = "JPG Files(*.JPG)|*.jpg; | PNG Files|*.png;| BMP Files|*.bmp;| JPEG Files|*.jpeg";
            var res = dia.ShowDialog();
            var tempName = dia.FileName.ToLower().Split('.');
            if(tempName.Count() == 2)
            {
                if(tempName[1]=="png" || tempName[1] == "jpg" || tempName[1] == "jpeg" ||tempName[1] == "bmp")
                    ImagePage.ImageLocation = dia.FileName;
                else
                    ImagePage.ImageLocation = dia.FileName.Split('.')[0] + ".png";
            }
            else
                ImagePage.ImageLocation = dia.FileName.Split('.')[0] + ".png";
            
            if (res == true)
            {
                //using (var fs = new FileStream(ImageLocation, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                //{
                //    var encoder = new PngBitmapEncoder();
                //    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)ProcessedImage.Source));
                //    encoder.Save(fs);

                //    img = System.Drawing.Image.FromStream(fs);
                //}
                SaveToPng(ImagePage.canvas, ImagePage.ImageLocation);
            }
        }
        public void SaveToPng(FrameworkElement visual, string fileName)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            SaveUsingEncoder(visual, fileName, encoder);

        }
        private static void SaveUsingEncoder(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            var visualSize = new System.Windows.Size(visual.ActualWidth, visual.ActualHeight);
            visual.Measure(visualSize);
            visual.Arrange(new Rect(visualSize));
            bitmap.Render(visual);
            BitmapFrame frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);

            using (var stream = File.Create(fileName))
            {
                encoder.Save(stream);
            }


        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GetName.Show();
            if (!string.IsNullOrWhiteSpace(App.color))
            {
                this.Title = App.color;
            }
        }
        private void Undo_Click(object sender, RoutedEventArgs e)
        {

            var len = ImagePage.canvas.Children.Count;
            if (len > 1)
            {
               ImagePage.canvas.Children.RemoveAt(len - 1);
            }
        }
    }
}
