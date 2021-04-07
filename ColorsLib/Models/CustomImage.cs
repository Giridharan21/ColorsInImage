using System.Drawing;

namespace ColorsLib.Models
{
    public class CustomImage
    {
        public Bitmap Image { get; set; }
        public Color GetColorName(int r, int g, int b)
        {
            // b >= 0 && b <= 50
            // g >= 0 && g <= 50
            // r >= 0 && r<=50 
            if (r == 255 && g >= 0 && g <= 50 && b >= 0 && b <= 50)
            {
                return Color.Red;
            }
            else if (r >= 0 && r<=50 && g >= 0 && g<=50 && b == 255)
            {
                return Color.Blue;
            }
            else if (r >= 0 && r <= 50 && g == 255 && b >= 0 && b <= 50)
            {
                return Color.Green;
            }//148, 0, 211

            else if (r == 148 && g == 0 && b == 211)
            {
                return Color.Violet;
            }
            //75, 0, 130
            else if (r == 75 && g == 0 && b == 130)
            {
                return Color.Indigo;
            }
            //255, 234, 100 to 255,255,0
            else if (r == 255 && g >= 234 && g <= 255 && b >= 0 && b <= 100)
            {
                return Color.Yellow;
            }
            else if (r == 255 && g == 127 && b >= 0 && b <= 70)
            {
                return Color.Orange;
            }
            //if (r>=220 && r <= 255)
            //{
            //    if (g >= 0 && g <= 50 && b >= 0 && b <= 50)
            //        return Color.Red;
            //    else
            //        return Color.Transparent;
            //}
            else
            {
                return Color.Transparent;
            }
        }
    }
}
