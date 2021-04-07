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
    /// Interaction logic for ProcessImage.xaml
    /// </summary>
    public partial class ProcessImage : Page
    {
        public System.Drawing.Image img;
        public Dictionary<System.Drawing.Color, List<CustomPosition>> pos { get; set; }

        private bool anchorPointSet { get; set; }

        public bool CanMark { get; set; }
        public string ImageLocation { get;  set; }
        public bool IsSaved { get; set; }
        public System.Windows.Point anchorPoint { get; set; }
        public Ellipse elip { get;  set; } = new Ellipse();
        public System.Windows.Shapes.Shape DrawingShape { get; set; } = new System.Windows.Shapes.Rectangle();
        public System.Windows.Point pt { get;  set; }
        public double width { get; set; }
        public double height { get; set; }
        public Shape SelectedShape { get; set; } = Shape.none;
        public string SelectedImage { get;  set; }
        public enum Shape
        {
            none, rectangle, circle
        }
        public ProcessImage()
        {
            InitializeComponent();
        }
       
        private BitmapImage GetImage(string filename = null)
        {
            Bitmap bmp = null;
            if(img !=null)
                bmp = new Bitmap(img);
            if (filename != null)
            {
                bmp = new Bitmap(filename);
            }
            if (bmp == null && filename == null)
                return null;
            var bi = new BitmapImage();

            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = ms;
                bi.EndInit();
            }
            bmp.Dispose();
            return bi;
        }
        private void SelectImage_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (img != null)
                UpdateNewImage();
            var dia = new OpenFileDialog();
            //dia.Filter = "Image Files(*.BMP; *.JPG; *.GIF ; *.PNG)| *.BMP; *.JPG; *.GIF; *.PNG | All files(*.*) | *.* " ;
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
            SelectedImage = dia.FileName;
            SelectedImageTextBlock.Text = "Selected Image : " + SelectedImage;
        }
        private async void AnalyseImage(object sender, RoutedEventArgs e)
        {
            ShowProgressBar();
            img = null;
            ProcessedImage.Source = null;
            if (string.IsNullOrWhiteSpace(SelectedImage))
            {
                MessageBox.Show("Select a Image file (*.jpeg, *.jpg, *.png )", "Alert");
                HideProgressBar();
                return;
            }
            var res = MessageBox.Show("Do you want to mark the shapes in Black?", "Alert", MessageBoxButton.YesNo, MessageBoxImage.Question);

            var additional = new AdditionalColor()
            {
                HasValue = ClrPcker_Background.SelectedColor.HasValue,
            };
            if (ClrPcker_Background.SelectedColor.HasValue)
            {
                var c = ClrPcker_Background.SelectedColor.GetValueOrDefault();
                additional.colorCode = System.Drawing.Color.FromArgb(c.R, c.G, c.B);
                additional.ColorName = ClrPcker_Background.SelectedColorText;
                img = await Program.ProcessImage(SelectedImage,res == MessageBoxResult.OK, additional:  additional);
            }
            else
            {
                var imgWidth = ProcessedImage.ActualWidth;

                img  = await Program.ProcessImage(SelectedImage,res == MessageBoxResult.OK);
               // (img,pos)  = await Program.ProcessImage2(SelectedImage,res == MessageBoxResult.OK);
                if (img != null)
                {
                    ProcessedImage.Source = GetImage();
                }
                
                //foreach (var color in pos.Keys)
                //{
                //    foreach (var pos in pos[color])
                //    {
                //        var x = img.HorizontalResolution;
                //        var y = img.VerticalResolution;
                //        var iH = img.Height; //1104
                //        var iW = img.Width - pos.rect.Width; //1824
                //        var cH = canvas.ActualHeight; //1233
                //        var cW = canvas.ActualWidth- pos.rect.Width; //500


                //        var wMul = cW / iW;
                //        var hMul = cH / iH;
                //        var r = new System.Windows.Shapes.Rectangle()
                //        {
                //            Stroke = System.Windows.Media.Brushes.Black,
                //            StrokeThickness = 1
                //        };

                //        //var canvasWidth = canvas.ActualWidth;
                //        //var diff = (canvasWidth - imgWidth) / 2;
                //        r.Width = wMul*  pos.rect.Width;
                //        r.Height = hMul* pos.rect.Height ;

                //        var t = new TextBlock();
                //        t.Text = color.Name;

                //        Canvas.SetTop(r, hMul* pos.rect.Y );
                //        Canvas.SetLeft(r, wMul* pos.x - pos.rect.Width  );
                //        Canvas.SetTop(t, hMul* pos.rect.Y-30 );
                //        Canvas.SetLeft(t, wMul* pos.x - pos.rect.Width);


                //        canvas.Children.Add(r);
                //        canvas.Children.Add(t);
                //        //g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Black), pos.rect);
                //        //g.DrawString(color.Name, font, Brushes.Black, pos.x + 10, pos.y - 50);
                //    }
                //}
            }
            //clearCanvas();

            
            //SelectedImage = null;
            //SelectedImageTextBlock.Text = "No File Selected";
            HideProgressBar();
        }
        public void setImage(string path)
        {
            ProcessedImage.Source = GetImage(path);
        }
        

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var x = e.GetPosition((InkCanvas)sender);

            var res = GetName.Show();
            var y = new System.Drawing.Point((int)x.X + 410, (int)x.Y + 180);
            DrawText(App.color, y);
        }
        private void DrawText(string text, System.Drawing.Point p)
        {
            var font = new Font("rockwell", 12);

            using (var g = Graphics.FromImage(img))
            {
                g.DrawString(text, font, System.Drawing.Brushes.Black, p);
            }
            //img.SaveAdd(img, new System.Drawing.Imaging.EncoderParameters());
            //y.Save(ImageLocation);

            ProcessedImage.Source = GetImage();
            //img = System.Drawing.Image.FromFile(ImageLocation);
            UpdateNewImage();

        }


        private void DrawShape_Click(object sender, RoutedEventArgs e)
        {
            SaveToPng2();

            //DrawShape.Visibility = Visibility.Collapsed;
            //CancelDraw.Visibility = Visibility.Visible;
            //AddName.Visibility = Visibility.Collapsed;
            //canvas.EditingMode = InkCanvasEditingMode.Ink;
        }
        public void SaveToPng2()
        {

            //using (var fs = new FileStream(ImageLocation,FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.ReadWrite))
            //{
            //    var encoder = new PngBitmapEncoder();
            //    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)ProcessedImage.Source));
            //    encoder.Save(fs);

            //    img = System.Drawing.Image.FromStream(fs);
            //}
            using (var ms = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)ProcessedImage.Source));
                encoder.Save(ms);
                img = System.Drawing.Image.FromStream(ms);
            }
        }
        
        private void SaveImageLocal(object sender, RoutedEventArgs e)
        {
            if (!IsSaved)
            {
                var dia = new SaveFileDialog();
                var res = dia.ShowDialog();
                dia.Title = "Select a location";
                dia.DefaultExt = "png";
                dia.Filter = "Png (*.png) | *.png";
                ImageLocation = dia.FileName.Split('.')[0] + ".png";
                if (res == true)
                {
                    //SaveToPng2();
                    //SaveToPng(canvas, ImageLocation);
                }
                IsSaved = true;
                return;
            }
            else
            {
                //SaveToPng2();
                //SaveToPng(canvas, ImageLocation);
                MessageBox.Show("Image Saved", "Message");
            }
            UpdateNewImage();


        }
        private void UpdateNewImage()
        {
            canvas.Children.Clear();
            canvas.Children.RemoveRange(0, canvas.Children.Count - 1);
            var img = new System.Windows.Controls.Image();
            img.Name = "ProcessedImage";
            img.Height = canvas.Height;
            img.Width = canvas.Width;
            img.Source = GetImage();
            canvas.Children.Add(img);


        }
        private void CancelDraw_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ImageLocation))
                ProcessedImage.Source = GetImage();
            //DrawShape.Visibility = Visibility.Visible;
            //CancelDraw.Visibility = Visibility.Collapsed;
            //AddName.Visibility = Visibility.Visible;
            //canvas.EditingMode = InkCanvasEditingMode.None;
            canvas.MouseDown -= canvas_MouseDown;
        }

        private void AddName_Click(object sender, RoutedEventArgs e)
        {
            //canvas.EditingMode = InkCanvasEditingMode.None;
            if (!IsSaved)
            {
                SaveToPng2();

                //SaveImageLocal(null, null);
            }

            canvas.MouseDown += canvas_MouseDown;
            //AddName.Visibility = Visibility.Collapsed;
            //DrawShape.Visibility = Visibility.Collapsed;
            //CancelDraw.Visibility = Visibility.Visible;

        }

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            

        }
        private string ConvertPoint(System.Windows.Point p)
        {
            return $"{(int)(p.X)} {(int)(p.Y)}";
        }
        private void canvas1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!CanMark)
                return;

            canvas.CaptureMouse();

            anchorPoint = e.MouseDevice.GetPosition(canvas);
            anchorPointSet = true;
            if (SelectedShape == Shape.rectangle)
            {
                DrawingShape = new System.Windows.Shapes.Rectangle()
                {
                    Stroke = System.Windows.Media.Brushes.Black,
                    StrokeThickness = 2
                };
            }
            else
            {
                DrawingShape = new Ellipse
                {
                    Stroke = System.Windows.Media.Brushes.Black,
                    StrokeThickness = 2
                };
            }
            //elip = new Ellipse
            //{
            //    Stroke = System.Windows.Media.Brushes.Black,
            //    StrokeThickness = 2
            //};
            //canvas.Children.Add(elip);
            canvas.Children.Add(DrawingShape);
        }

        private void canvas1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!canvas.IsMouseCaptured)
                return;

            if (DrawingShape != null)
            {
                var location = e.MouseDevice.GetPosition(canvas);

                double minX = Math.Min(location.X, anchorPoint.X);
                double minY = Math.Min(location.Y, anchorPoint.Y);
                double maxX = Math.Max(location.X, anchorPoint.X);
                double maxY = Math.Max(location.Y, anchorPoint.Y);

                Canvas.SetTop(DrawingShape, minY);
                Canvas.SetLeft(DrawingShape, minX);

                pt = new System.Windows.Point(minX, minY);

                height = maxY - minY;
                width = maxX - minX;

                DrawingShape.Height = Math.Abs(height);
                DrawingShape.Width = Math.Abs(width);
            }
        }

        private void canvas1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (canvas.IsMouseCaptured)
            {
                canvas.ReleaseMouseCapture();
                var textBlock = new TextBlock();
                textBlock.Text = ColorTextBox.Text;
                textBlock.FontSize = 16;
                canvas.Children.Add(textBlock);
                Canvas.SetTop(textBlock, pt.Y - 25);
                Canvas.SetLeft(textBlock, pt.X + 5);
                DrawingShape = null;
            }

            //ColorTextBox.Text = "";
        }

        private void ShowProgressBar()
        {
            LoadingGrid.Visibility = Visibility.Visible;
            ContentGrid.Visibility = Visibility.Collapsed;
        }
        private void HideProgressBar()
        {
            LoadingGrid.Visibility = Visibility.Collapsed;
            ContentGrid.Visibility = Visibility.Visible;
        }

        private void MarkShape_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ColorTextBox.Text))
            {
                MessageBox.Show("Enter color name before marking!.", "alert");
                return;
            }
            if (SelectedShape == Shape.none)
            {
                MessageBox.Show("select a shape before marking!.", "alert");
                return;
            }

            CanMark = true;
            //CancelMark.Visibility = Visibility.Visible;
            //MarkShape.Visibility = Visibility.Collapsed;
        }

        private void CancelMark_Click(object sender, RoutedEventArgs e)
        {
            CanMark = false;
            //CancelMark.Visibility = Visibility.Collapsed;
            MarkShape.Visibility = Visibility.Visible;
        }

        private void shape_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn.Name != "circle")
            {
                btn.BorderBrush = System.Windows.Media.Brushes.LightBlue;
                circleBorder.BorderBrush = System.Windows.Media.Brushes.Black;
                SelectedShape = Shape.rectangle;

            }
            else
            {
                rectShape.BorderBrush = System.Windows.Media.Brushes.Black;
                circleBorder.BorderBrush = System.Windows.Media.Brushes.LightBlue;
                SelectedShape = Shape.circle;
            }
        }

        private void TitleChange(object sender, RoutedEventArgs e)
        {

        }

       
        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Drawing.Color> e)
        {
            colorTextBlock.Text = "#" + ClrPcker_Background.SelectedColor.GetValueOrDefault().R.ToString() + ClrPcker_Background.SelectedColor.GetValueOrDefault().G.ToString() + ClrPcker_Background.SelectedColor.GetValueOrDefault().B.ToString();
        }

        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {

            colorTextBlock.Text = ClrPcker_Background.SelectedColor.GetValueOrDefault().ToString();

        }

    }
}
