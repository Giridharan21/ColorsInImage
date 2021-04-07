using System.Drawing;

namespace ColorsInImage
{
    public class CustomPosition
    {
        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int UpperHeight { get; set; }
        public int LowerHeight { get; set; }
        public Rectangle rect
        {
            get {
                return new Rectangle(x-10, y-10, width-x+20, LowerHeight-UpperHeight+20);
            }
        }
    }
    
}
