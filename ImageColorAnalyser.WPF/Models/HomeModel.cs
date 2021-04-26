using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageColorAnalyser.WPF.Models
{
    public class HomeModel : BindableBase
    {
        private string colorCode;
        private string colorName;
        private string fileName;
        private string message;
        private Shape selectedShape;
        private BitmapImage currentImage;
        private string savedImagePath;
        private Color? selectedColor;
        private Cursor cursor;
        private string title;
        private Visibility loader;
        private Color? shapeColor;
        private string shapeColorCode;
        private List<ColorNames> colorNames;
        private Visibility isDataGridVisible;
        private string btnContent;

        public Visibility Loader { get => loader; set => SetProperty(ref loader, value); }
        public string Title { get => title; set => SetProperty(ref title, value); }
        public string ColorCode
        {
            get => colorCode;
            set => SetProperty(ref colorCode, value);
        }
        public string BtnContent { get => btnContent; set => SetProperty(ref btnContent, value); }
        public Visibility IsDataGridVisible { get => isDataGridVisible; set => SetProperty(ref isDataGridVisible, value); }
        public List<ColorNames> ColorNames { get => colorNames; set => SetProperty(ref colorNames, value); }
        public string ColorName
        {
            get => colorName;
            set => SetProperty(ref colorName, value);
        }

        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }

        public string FileName
        {
            get => fileName;
            set => SetProperty(ref fileName, value);
        }
        public Shape SelectedShape
        {
            get => selectedShape;
            set => SetProperty(ref selectedShape, value);
        }

        public BitmapImage CurrentImage
        {
            get => currentImage;
            set => SetProperty(ref currentImage, value);
        }

        public string SavedImagePath
        {
            get => savedImagePath;
            set => SetProperty(ref savedImagePath, value);
        }
        public Color? SelectedColor
        {
            get => selectedColor;
            set => SetProperty(ref selectedColor, value);
        }
        public Color? ShapeColor
        {
            get => shapeColor;
            set => SetProperty(ref shapeColor, value);
        }
        public string ShapeColorCode
        {
            get => shapeColorCode;
            set => SetProperty(ref shapeColorCode, value);
        }
        public Cursor Cursor
        {
            get => cursor;
            set => SetProperty(ref cursor, value);
        }

    }
}
