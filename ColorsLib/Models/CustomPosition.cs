using System.Drawing;

namespace ColorsLib.Models
{
    public class CustomPosition
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double UpperHeight { get; set; }
        public double LowerHeight { get; set; }
        public Rectangle Rect
        {
            get
            {
                return new Rectangle(X.ToInt() - 10, Y.ToInt() - 10, Width.ToInt() - X.ToInt() + 20, LowerHeight.ToInt() - UpperHeight.ToInt() + 20);
            }
        }
        public void ResizeRect(double x1, double y1)
        {
            X *= x1;
            Y *= y1;
            //LowerHeight *= y1;
            //UpperHeight *= y1;
            //Width *= x1;
        }
    }
    public static class ExtensionMethod
    {
        public static int ToInt(this double x)
        {
            return (int)x;
        }
    }
}
