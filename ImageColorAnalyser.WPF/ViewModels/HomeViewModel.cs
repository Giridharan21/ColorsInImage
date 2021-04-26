using ColorsInImage.WPF;
using ColorsLib.Models;
using ImageColorAnalyser.WPF.Models;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Image = System.Windows.Controls.Image;
using Toolkit = Xceed.Wpf.Toolkit;

namespace ImageColorAnalyser.WPF.ViewModels
{
    public enum ElemType{
        Rectangle,
        Elipse,
        Text
    }
    public class HomeViewModel : BindableBase
    {
        private HomeModel homeModel;
        private bool DragInProgress = false;
        private System.Windows.Point LastPoint;
        private HitType MouseHitType = HitType.None;

        public HomeModel HomeModel { get => homeModel; set => SetProperty(ref homeModel, value); }
        public bool CanMark { get; set; }
        public System.Windows.Point anchorPoint { get; set; }
        public System.Windows.Shapes.Shape DrawingShape { get; set; } = new System.Windows.Shapes.Rectangle();
        public System.Windows.Point pt { get; set; }

        private bool anchorPointSet { get; set; }
        public CurrentOp CurrentOp { get; set; }
        public UIElement CurrentElem { get; set; }
        public ElemType CurrentElemType { get; set; }
        public Dictionary<System.Drawing.Color, List<CustomPosition>> pos { get; set; }
        public ICommand UploadCmd { get; set; }
        public ICommand SaveCmd { get; set; }
        public ICommand EditTitleCmd { get; set; }
        public ICommand ClearCmd { get; set; }
        public ICommand UndoCmd { get; set; }
        public ICommand ColorChangedCmd { get; set; }
        public ICommand AnalyseImageCmd { get; set; }
        public ICommand MouseUpCmd { get; set; }
        public ICommand MouseDownCmd { get; set; }
        public ICommand MouseMoveCmd { get; set; }
        public ICommand OpChangedCmd { get; set; }
        public ICommand ShapeChangedCmd { get; set; }
        public ICommand ChangeTitleCmd { get; set; }
        public ICommand ShapeColorChangedCmd { get; set; }
        public ICommand CorrelateCmd { get; set; }
        public HomeViewModel()
        {
            HomeModel = new HomeModel();
            HomeModel.Title = "Image Color Analyser";
            HomeModel.Loader = Visibility.Collapsed;
            HomeModel.IsDataGridVisible = Visibility.Collapsed;
            HomeModel.BtnContent = "Show Correlate";
            HomeModel.ColorNames = new List<ColorNames>() { new ColorNames() };
            CurrentElem = new System.Windows.Shapes.Rectangle();
            //homeModel.FileName = @"C:\Users\844617\Downloads\red.png";
            //homeModel.FileName = @"C:\Users\844617\Downloads\Testing-Image.png";
            UploadCmd = new DelegateCommand(UploadImage);
            SaveCmd = new DelegateCommand<UIElement>(SaveImage);
            UndoCmd = new DelegateCommand<UIElement>(Undo);
            ClearCmd = new DelegateCommand<UIElement>(Clear);
            ColorChangedCmd = new DelegateCommand<UIElement>(ColorChanged);
            AnalyseImageCmd = new DelegateCommand<UIElement>(AnalyseImage);
            MouseUpCmd = new DelegateCommand<UIElement>(MouseUp);
            MouseDownCmd = new DelegateCommand<UIElement>(MouseDown);
            MouseMoveCmd = new DelegateCommand<UIElement>(MouseMove);
            OpChangedCmd = new DelegateCommand<object>(OpChanged);
            ShapeChangedCmd = new DelegateCommand<object>(ShapeChanged);
            ChangeTitleCmd = new DelegateCommand(ChangeTitle);
            ShapeColorChangedCmd = new DelegateCommand<UIElement>(ShapeColorChanged);
            CorrelateCmd = new DelegateCommand(Correlate);

        }

        private void Correlate()
        {
            try
            {
                
                if(HomeModel.BtnContent== "Show Correlate")
                {
                    HomeModel.BtnContent = "Hide Correlate";
                    HomeModel.IsDataGridVisible = Visibility.Visible;
                }
                else if(HomeModel.BtnContent == "Hide Correlate")
                {
                    HomeModel.BtnContent = "Show Correlate";
                    HomeModel.IsDataGridVisible = Visibility.Collapsed;

                }
            }
            catch (Exception)
            {

                
            }
        }

        private void ChangeTitle()
        {
            var str = GetNameFromWindow();
            HomeModel.Title = str;
        }

        private string GetNameFromWindow()
        {
            var s = new GetName();
            s.ColorTextBox.Focus();
            s.ShowDialog();
            return s.ColorTextBox.Text;
        }

        private void ShapeChanged(object obj)
        {
            var str = (((ComboBoxItem)obj).Content as TextBlock).Text;
            if (str is null) return;
            HomeModel.SelectedShape = str == "Circle" ? Models.Shape.circle : Models.Shape.rectangle;
        }

        private void OpChanged(object obj)
        {
            var str = (((ComboBoxItem)obj).Content as TextBlock).Text;
            if (str is null) return;
            switch (str)
            {
                case "None":
                    CurrentOp = CurrentOp.None;
                    break;
                case "Draw":
                    CurrentOp = CurrentOp.Draw;
                    break;
                case "Resize":
                    CurrentOp = CurrentOp.Resize;
                    break;
                case "Change Name":
                    CurrentOp = CurrentOp.ChangeName;
                    break;
                case "Remove":
                    CurrentOp = CurrentOp.RemoveChild;
                    break;
                default:
                    CurrentOp = CurrentOp.None;
                    break;
            }
            //CurrentOp = str == "None" ? CurrentOp.None : str == "Draw" ? CurrentOp.Draw : CurrentOp.Resize;
        }

        private void UploadImage()
        {
            var dia = new OpenFileDialog();
            dia.Filter = "Image Files|*.jpg;*.png;*.bmp;*.jpeg; ";
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
            //ImagePage.SelectedImage = dia.FileName;
            HomeModel.FileName = dia.FileName;
            //ImagePage.SelectedImageTextBlock.Text = "Selected Image : " + ImagePage.SelectedImage;
            //ImagePage.setImage(dia.FileName);
            HomeModel.CurrentImage = GetBitMapImage(fileName:dia.FileName);

            
        }
        private BitmapImage GetBitMapImage(System.Drawing.Image img =null,string fileName = null)
        {
            if (img == null && fileName == null)
                return null;
            var bmp = fileName == null ? new Bitmap(img) : new Bitmap(fileName);
            var bi = new BitmapImage();

            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
            }
            bmp.Dispose();
            return bi;
        }
        private void SaveImage(UIElement canvas)
        {
            if (string.IsNullOrWhiteSpace(HomeModel.FileName))
            {
                MessageBox.Show("Image is not processed yet!.", "alert");
                return;
            }
            var dia = new SaveFileDialog();
            dia.Title = "Select a location";
            dia.Filter = "JPG Files(*.JPG)|*.jpg; | PNG Files|*.png;| BMP Files|*.bmp;| JPEG Files|*.jpeg";
            var res = dia.ShowDialog();
            var tempName = dia.FileName.ToLower().Split('.');
            if (tempName.Count() == 2)
            {
                if (tempName[1] == "png" || tempName[1] == "jpg" || tempName[1] == "jpeg" || tempName[1] == "bmp")
                    HomeModel.SavedImagePath = dia.FileName;
                else
                    HomeModel.SavedImagePath = dia.FileName.Split('.')[0] + ".png";
            }
            else
                HomeModel.SavedImagePath = dia.FileName.Split('.')[0] + ".png";
            if (res == true)
            {
                SaveToPng((FrameworkElement)canvas, HomeModel.SavedImagePath);
            }
        }
        public void SaveToPng(FrameworkElement visual, string fileName)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            var visualSize = new System.Windows.Size(visual.ActualWidth, visual.ActualHeight);
            visual.Measure(visualSize);
            visual.Arrange(new Rect(visualSize));
            bitmap.Render(visual);
            BitmapFrame frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            using (var stream = File.Create(fileName))
            {
                encoder.Save(stream);
            }

            
        }
        private void Undo(UIElement element)
        {
            var canvas = (Canvas)element;
            if (canvas != null)
            {
                var len = canvas.Children.Count;
                if (len > 1)
                {
                    canvas.Children.RemoveAt(len - 1);
                }
            }

        }
        private void Clear(UIElement element)
        {
            var canvas = (Canvas)element;
            if (canvas != null)
            {
                var len = canvas.Children.Count;
                while(len > 1)
                {
                    canvas.Children.RemoveAt(--len);
                }
            }
        }

        private void ColorChanged(UIElement element)
        {
            HomeModel.ColorCode= ((Toolkit.ColorPicker)element)?.SelectedColor.GetValueOrDefault().ToString();
        }
        private void ShapeColorChanged(UIElement element)
        {
            HomeModel.ShapeColorCode = ((Toolkit.ColorPicker)element)?.SelectedColor.GetValueOrDefault().ToString();
        }

        private async void AnalyseImage(UIElement element)
        {
            try
            {
                var canvas = (Canvas)element;
                if (string.IsNullOrWhiteSpace(HomeModel.FileName))
                {
                    MessageBox.Show("Select a Image file (*.jpeg, *.jpg, *.png )", "Alert");
                    return;
                }
                //var res = MessageBox.Show("Do you want to mark the shapes in Black?", "Alert", MessageBoxButton.YesNo, MessageBoxImage.Question);
                System.Drawing.Image img = null;
                var additional = new AdditionalColor()
                {
                    HasValue = HomeModel.ColorCode != null,
                };
                if(HomeModel.BtnContent == "Hide Correlate")
                {
                    Correlate();
                }
                HomeModel.Loader = Visibility.Visible;

                if (additional.HasValue)
                {
                    var c = HomeModel.SelectedColor.GetValueOrDefault();
                    additional.colorCode = System.Drawing.Color.FromArgb(c.R, c.G, c.B);
                    additional.ColorName = HomeModel.SelectedColor.GetValueOrDefault().ToString();
                    //img = await Program.ProcessImage(HomeModel.FileName, res == MessageBoxResult.Yes);
                    (img, pos) = await Program.ProcessImage2(HomeModel.FileName,false, additional: additional);

                }
                else
                {
                    //img = await Program.ProcessImage(HomeModel.FileName, res == MessageBoxResult.Yes);
                    (img, pos) = await Program.ProcessImage2(HomeModel.FileName, false);

                }
                //var imgPath = @"C:\Program Files (x86)\Image Analyser";
                //if (!Directory.Exists(imgPath))
                //{
                //    Directory.CreateDirectory(imgPath);
                //}
                //var imgName = System.IO.Path.Combine(imgPath, "temp " + GetDateTime() + ".png");
                //img.Save(imgName);
                //HomeModel.FileName = imgName;



                var brush = HomeModel.ShapeColor.HasValue ? new SolidColorBrush(HomeModel.ShapeColor.Value) : System.Windows.Media.Brushes.Black;
                var x = img.HorizontalResolution;
                var y = img.VerticalResolution;
                var iH = img.Height; //1104
                var iW = img.Width; //1824
                int gcd = GetGCD(iW, iH);
                var ratio = (double)iW / iH;
                var aspectRatioW = iW / gcd;
                var aspectRatioH = iH / gcd;

                int aH = 0, aW = 0;

                var w = canvas.ActualWidth;
                var h = canvas.ActualHeight;

                if ((iH > h && iW > w) || (iH < h && iW < w))
                {
                    if (h > w)
                    {
                        aH = (int)(w / ratio);
                        aW = (int)w;
                    }
                    else
                    {
                        aW = (int)(ratio * h);
                        aH = (int)h;
                    }
                }
                else if (iH < h && iW > w)
                {
                    aW = (int)w;
                    aH = (int)(w / ratio);
                }
                else if (iH > h && iW < w)
                {
                    aH = (int)h;
                    aW = (int)(ratio * h);
                }

                var wMul = (double)aW / iW;
                var hMul = (double)aH / iH;

                foreach (var color in pos.Keys)
                {
                    foreach (var pos in pos[color])
                    {

                        //var wMul = 1;
                        //var hMul = 1;




                        var r = new System.Windows.Shapes.Rectangle()
                        {
                            Stroke = brush,
                            StrokeThickness = 2
                        };

                        //var canvasWidth = canvas.ActualWidth;
                        //var diff = (canvasWidth - imgWidth) / 2;
                        //pos.ResizeRect(wP, hP);
                        var minimizedWidth = Math.Abs((pos.Rect.Width * wMul));
                        //minimizedWidth -= minimizedWidth * (wMul * 0.4);
                        r.Width = minimizedWidth;
                        var minimizedHeight = Math.Abs((pos.Rect.Height * hMul));
                        //minimizedHeight -= minimizedHeight * hMul * 0.20;
                        r.Height = minimizedHeight;
                        var t = new TextBlock();
                        t.Foreground = brush;
                        var name = GetColorName(color.Name);
                        t.Text = name ?? color.Name;
                        pos.Y = pos.Y * hMul;// - (pos.Y * hP);
                        pos.X = (pos.X * wMul);// - (pos.X * wP);
                                               //pos.X -= (pos.X * 0.5 * wMul);// - (pos.X * wP);
                        Canvas.SetTop(r, pos.Y);
                        Canvas.SetLeft(r, pos.X);
                        Canvas.SetTop(t, pos.Y - 20);
                        Canvas.SetLeft(t, pos.X);


                        canvas.Children.Add(r);
                        canvas.Children.Add(t);
                        //g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Black), pos.rect);
                        //g.DrawString(color.Name, font, Brushes.Black, pos.x + 10, pos.y - 50);
                    }
                }
                //clearCanvas();
                HomeModel.Loader = Visibility.Collapsed;

                HomeModel.CurrentImage = GetBitMapImage(img);
                //SelectedImage = null;
                //SelectedImageTextBlock.Text = "No File Selected";
            }
            catch (Exception exp)
            {

                MessageBox.Show(exp.Message + "\n"+exp.ToString());
            }
            
        }
        private string GetColorName(string name)
        {
            var y = HomeModel.ColorNames[0];

            try
            {
                return y.GetType().GetProperty(name).GetValue(y).ToString();
            }
            catch (Exception)
            {

                return null;
            }
            

            
        }

        private int GetGCD(int iW, int iH)
        {
            var gcd = 1;
            var min = iW < iH ? iW : iH;
            for (int i = 2; i < min; i++)
            {
                if (iW % i == 0 && iH % i == 0)
                    gcd = i;
            }
            return gcd;
        }

        private HitType SetHitType(UIElement rect, System.Windows.Point point)
        {
            double left = Canvas.GetLeft(rect);
            double top = Canvas.GetTop(rect);
            double right = 0;//= left + rect.Width;
            double bottom = 0;// = top + rect.Height;
            if (CurrentElemType == ElemType.Rectangle)
            {
                right = left+ (CurrentElem as System.Windows.Shapes.Rectangle).Width;
                bottom = top+ (CurrentElem as System.Windows.Shapes.Rectangle).Height;

            }
            else if (CurrentElemType == ElemType.Elipse)
            {
                right = left + (CurrentElem as System.Windows.Shapes.Ellipse).Width;
                bottom = top + (CurrentElem as System.Windows.Shapes.Ellipse).Height;
            }
            else if (CurrentElemType == ElemType.Text)
            {
                right = left + (CurrentElem as TextBlock).Width;
                bottom = top + (CurrentElem as TextBlock).Height;
                return HitType.Body;

            }
            if (double.IsNaN(left))
            {
                return HitType.None;
            }
            if (point.X < left) return HitType.None;
            if (point.X > right) return HitType.None;
            if (point.Y < top) return HitType.None;
            if (point.Y > bottom) return HitType.None;

            const double GAP = 10;
            if (point.X - left < GAP)
            {
                // Left edge.
                if (point.Y - top < GAP) return HitType.UL;
                if (bottom - point.Y < GAP) return HitType.LL;
                return HitType.L;
            }
            if (right - point.X < GAP)
            {
                // Right edge.
                if (point.Y - top < GAP) return HitType.UR;
                if (bottom - point.Y < GAP) return HitType.LR;
                return HitType.R;
            }
            if (point.Y - top < GAP) return HitType.T;
            if (bottom - point.Y < GAP) return HitType.B;
            return HitType.Body;
        }
        private void SetMouseCursor()
        {
            // See what cursor we should display.
            Cursor desired_cursor = Cursors.Arrow;
            switch (MouseHitType)
            {
                case HitType.None:
                    desired_cursor = Cursors.Arrow;
                    break;
                case HitType.Body:
                    desired_cursor = Cursors.ScrollAll;
                    break;
                case HitType.UL:
                case HitType.LR:
                    desired_cursor = Cursors.SizeNWSE;
                    break;
                case HitType.LL:
                case HitType.UR:
                    desired_cursor = Cursors.SizeNESW;
                    break;
                case HitType.T:
                case HitType.B:
                    desired_cursor = Cursors.SizeNS;
                    break;
                case HitType.L:
                case HitType.R:
                    desired_cursor = Cursors.SizeWE;
                    break;
            }

            // Display the desired cursor.
            if (HomeModel.Cursor != desired_cursor) HomeModel.Cursor = desired_cursor;
        }

        // Start dragging.
        private void MouseDown(UIElement element)
        {
            if (CurrentOp == CurrentOp.Resize)
            {
                ResizeMouseDown(element);
            }
            else if(CurrentOp == CurrentOp.RemoveChild)
            {
                RemoveChildFromCanvas(element);
            }
            else if (CurrentOp == CurrentOp.Draw)
            {
                if (string.IsNullOrEmpty(homeModel.ColorName))
                    HomeModel.Message =  ("Enter Color name");
                else if (HomeModel.SelectedShape == Models.Shape.none)
                    HomeModel.Message = ("Select a shape");
                else
                {
                    DrawMouseDown(element);
                    HomeModel.Message = "";
                }
            }
            else if(CurrentOp == CurrentOp.ChangeName)
            {
                var canvas = (Canvas)element;
                TextBlock child = null;
                foreach (var item in canvas.Children)
                {
                    var t = item as TextBlock;
                    if (t !=null && t.IsMouseOver)
                    {
                        child = t;
                    }
                }
                if(child !=null)
                    child.Text = GetNameFromWindow();
            }
        }

        // If a drag is in progress, continue the drag.
        // Otherwise display the correct cursor.
        private void MouseMove(UIElement element)
        {

            if (CurrentOp == CurrentOp.Resize)
            {
                ResizeMouseMove(element);
            }
            else if (CurrentOp == CurrentOp.Draw)
            {
                DrawMouseMove(element);
            }
        }

        private void MouseUp(UIElement element)
        {
            if (CurrentOp == CurrentOp.Resize)
            {
                ResizeMouseUp();
            }
            else if (CurrentOp == CurrentOp.Draw)
            {
                DrawMouseUp(element);
            }
        }

        private void ResizeMouseUp()
        {
            DragInProgress = false;
        }

        private void RemoveChildFromCanvas(UIElement element)
        {
            var canvas1 = (Canvas)element;
            if (canvas1 == null)
                return;
            UIElement selectedChild = null;
            foreach (var x in canvas1.Children)
            {
                var r = x as System.Windows.Shapes.Rectangle;
                var c = x as Ellipse;
                var t = x as TextBlock;
                if( r!= null && r.IsMouseOver)
                {
                    selectedChild = r;
                    break;
                }
                else if (c != null && c.IsMouseOver)
                {
                    selectedChild = c;
                    break;
                }
                else if (t != null && t.IsMouseOver)
                {
                    selectedChild = t;
                    break;
                }

            }
            canvas1.Children.Remove(selectedChild);
        }
        private void ResizeMouseDown(UIElement element)
        {
            var canvas1 = (Canvas)element;
            if (canvas1 == null)
                return;
            foreach (var x in canvas1.Children)
            {
                var r = x as System.Windows.Shapes.Rectangle;
                var e =  x as System.Windows.Shapes.Ellipse;
                var t =  x as TextBlock;

                if (r != null && r.IsMouseOver)
                {
                    CurrentElem = r;

                    CurrentElemType = ElemType.Rectangle;
                }
                else if (e != null && e.IsMouseOver)
                {
                    CurrentElem = e;
                    CurrentElemType = ElemType.Elipse;
                }
                else if (t != null && t.IsMouseOver)
                {
                    CurrentElem = t;
                    CurrentElemType = ElemType.Text;
                }

            }
            if (CurrentElem == null)
                return;
            MouseHitType = SetHitType(CurrentElem, Mouse.GetPosition(canvas1));
            SetMouseCursor();
            if (MouseHitType == HitType.None) return;

            LastPoint = Mouse.GetPosition(canvas1);
            DragInProgress = true;
        }
        private void ResizeMouseMove(UIElement element)
        {
            var canvas1 = (Canvas)element;
            if (canvas1 == null)
                return;
            if (CurrentElem == null)
                return;
            if (!DragInProgress)
            {
                MouseHitType = SetHitType(CurrentElem, Mouse.GetPosition(canvas1));
                SetMouseCursor();
            }
            else
            {
                // See how much the mouse has moved.
                var point = Mouse.GetPosition(canvas1);
                double offset_x = point.X - LastPoint.X;
                double offset_y = point.Y - LastPoint.Y;

                // Get the rectangle's current position.
                double new_x = Canvas.GetLeft(CurrentElem);
                double new_y = Canvas.GetTop(CurrentElem);
                double new_width = 0; 
                double new_height = 0;
                if (CurrentElemType == ElemType.Rectangle)
                {
                    new_width = (CurrentElem as System.Windows.Shapes.Rectangle).Width;
                    new_height = (CurrentElem as System.Windows.Shapes.Rectangle).Height;

                }
                else if(CurrentElemType == ElemType.Elipse)
                {
                    new_width = (CurrentElem as System.Windows.Shapes.Ellipse).Width;
                    new_height = (CurrentElem as System.Windows.Shapes.Ellipse).Height;
                }
                else if (CurrentElemType == ElemType.Text)
                {
                    new_width = (CurrentElem as TextBlock).ActualWidth;
                    new_height = (CurrentElem as TextBlock).ActualHeight;
                }
                // Update the rectangle.
                switch (MouseHitType)
                {
                    case HitType.Body:
                        new_x += offset_x;
                        new_y += offset_y;
                        break;
                    case HitType.UL:
                        new_x += offset_x;
                        new_y += offset_y;
                        new_width -= offset_x;
                        new_height -= offset_y;
                        break;
                    case HitType.UR:
                        new_y += offset_y;
                        new_width += offset_x;
                        new_height -= offset_y;
                        break;
                    case HitType.LR:
                        new_width += offset_x;
                        new_height += offset_y;
                        break;
                    case HitType.LL:
                        new_x += offset_x;
                        new_width -= offset_x;
                        new_height += offset_y;
                        break;
                    case HitType.L:
                        new_x += offset_x;
                        new_width -= offset_x;
                        break;
                    case HitType.R:
                        new_width += offset_x;
                        break;
                    case HitType.B:
                        new_height += offset_y;
                        break;
                    case HitType.T:
                        new_y += offset_y;
                        new_height -= offset_y;
                        break;
                }

                // Don't use negative width or height.
                if ((new_width > 0) && (new_height > 0))
                {
                    // Update the rectangle.
                    Canvas.SetLeft(CurrentElem, new_x);
                    Canvas.SetTop(CurrentElem, new_y);
                    if (CurrentElemType == ElemType.Rectangle)
                    {
                        (CurrentElem as System.Windows.Shapes.Rectangle).Width = new_width;
                        (CurrentElem as System.Windows.Shapes.Rectangle).Height = new_height;
                    }
                    else if (CurrentElemType == ElemType.Elipse)
                    {
                         (CurrentElem as System.Windows.Shapes.Ellipse).Width = new_width ;
                         (CurrentElem as System.Windows.Shapes.Ellipse).Height = new_height;
                    }
                    else if (CurrentElemType == ElemType.Text)
                    {
                        (CurrentElem as TextBlock).Width = new_width;
                        (CurrentElem as TextBlock).Height = new_height;
                    }
                    //CurrentElem.Width = new_width;
                    //CurrentElem.Height = new_height;

                    // Save the mouse's new location.
                    LastPoint = point;
                }
            }
        }
        private void DrawMouseDown(UIElement element)
        {
            
            var canvas = (Canvas)element;
            

            canvas.CaptureMouse();

            anchorPoint = Mouse.GetPosition(canvas);
            anchorPointSet = true;
            if (HomeModel.SelectedShape == Models.Shape.rectangle)
            {
                DrawingShape = new System.Windows.Shapes.Rectangle()
                {
                    Stroke = HomeModel.ShapeColor.HasValue ? new SolidColorBrush(HomeModel.ShapeColor.Value ) : System.Windows.Media.Brushes.Black,
                    StrokeThickness = 2
                };
            }
            else
            {
                DrawingShape = new Ellipse
                {
                    Stroke = HomeModel.ShapeColor.HasValue ? new SolidColorBrush(HomeModel.ShapeColor.Value) : System.Windows.Media.Brushes.Black,
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

        private void DrawMouseMove(UIElement element)
        {
            var canvas = (Canvas)element;
            if (!canvas.IsMouseCaptured)
                return;

            if (DrawingShape != null)
            {
                var location = Mouse.GetPosition(canvas);

                double minX = Math.Min(location.X, anchorPoint.X);
                double minY = Math.Min(location.Y, anchorPoint.Y);
                double maxX = Math.Max(location.X, anchorPoint.X);
                double maxY = Math.Max(location.Y, anchorPoint.Y);

                Canvas.SetTop(DrawingShape, minY);
                Canvas.SetLeft(DrawingShape, minX);

                pt = new System.Windows.Point(minX, minY);

                var height = maxY - minY;
                var width = maxX - minX;

                DrawingShape.Height = Math.Abs(height);
                DrawingShape.Width = Math.Abs(width);
            }
        }

        private void DrawMouseUp(UIElement element)
        {
            var canvas = (Canvas)element;
            if (canvas is null)
                return;
            if (canvas.IsMouseCaptured)
            {
                canvas.ReleaseMouseCapture();
                var textBlock = new TextBlock();
                textBlock.Text = HomeModel.ColorName;
                textBlock.FontSize = 16;
                textBlock.Foreground = HomeModel.ShapeColor.HasValue ? new SolidColorBrush(HomeModel.ShapeColor.Value) : System.Windows.Media.Brushes.Black;
                canvas.Children.Add(textBlock);
                Canvas.SetTop(textBlock, pt.Y - 25);
                Canvas.SetLeft(textBlock, pt.X + 5);
                DrawingShape = null;
            }

            //ColorTextBox.Text = "";
        }

        private string GetDateTime()
        {
            return DateTime.Now.ToString("MMddyyyyhhmmss");
        }

    }
}
