using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorsInImage
{
    public class Solution
    {
        public void Solve(int A, List<int> B)
        {
            var Max = Math.Pow(B.Count(), A) % 1000000007;
            var temp = 0;
            foreach (var x in B)
            {

            }

        }
        public long FindValue(int x, int A)
        {
            if (x + 1 == A)
                return 1;
            else
            {
                var temp = (A - x);
                return FindValue(x + 1, A);
            }
        }
    }
    public class Program
    {
        private static CustomImage ImageObj = new CustomImage();
        public static Dictionary<Color, List<CustomPosition>> Positions { get; set; } = new Dictionary<Color, List<CustomPosition>>();
        static void Main()
        {

        }
        public static async Task<Image> ProcessImage(string location)
        {
            ImageObj.Image = new Bitmap(location);

            int c = 0;
            Image img = System.Drawing.Image.FromFile(location);

            await Task.Run(() =>
            {
                for (int i = 0; i < ImageObj.Image.Width; i++)
                {
                    for (int j = 0; j < ImageObj.Image.Height; j++)
                    {
                        var pixel = ImageObj.Image.GetPixel(i, j);
                        var color = ImageObj.GetColorName(pixel.R, pixel.G, pixel.B);
                        if (color == Color.Transparent)
                            continue;
                        if (Positions.ContainsKey(color))
                        {
                            //bool isDone = false;
                            //for (int y = 0; y < Positions[color].Count; y++)
                            //{
                            //    var rect = Positions[color][y].rect;
                            //    if (i >= rect.X && i <= rect.Width && j >= rect.Y && j <= rect.Height)
                            //    {
                            //        isDone = true;
                            //        break;
                            //    }
                            //}
                            //if (isDone)
                            //    continue;
                            continue;
                            //Positions[color].Add(pos);
                        }

                        var pos = new CustomPosition();
                        pos.x = i;
                        pos.y = j;
                        var tempWidth = ImageObj.Image.Width;
                        var tempHeight = ImageObj.Image.Height;
                        pos.UpperHeight = tempHeight;
                        int count = 0;
                        for (int w = i; w < tempWidth; w++)
                        {
                            bool check = false;
                            for (int h1 = j; h1 < tempHeight; h1++)
                            {
                                var tempPixel = ImageObj.Image.GetPixel(w, h1);
                                var tempColor = ImageObj.GetColorName(tempPixel.R, tempPixel.G, tempPixel.B);
                                if (tempColor != color)
                                {
                                    if (h1 != j)
                                        check = true;
                                    if (h1 > pos.LowerHeight)
                                        pos.LowerHeight = h1;
                                    break;
                                }
                            }
                            //Console.WriteLine("Upper height\n");
                            for (int h2 = j; h2 > 0; h2--)
                            {
                                //Console.WriteLine($"h2 - {h2}");
                                var tempPixel = ImageObj.Image.GetPixel(w, h2);
                                var tempColor = ImageObj.GetColorName(tempPixel.R, tempPixel.G, tempPixel.B);
                                if (tempColor != color)
                                {
                                    if (h2 != j)
                                        check = true;
                                    if (h2 < pos.UpperHeight)
                                        pos.UpperHeight = h2;
                                    break;
                                }
                            }
                            if (!check)
                            {
                                count++;
                                if (count > 30)
                                    break;
                            }
                            else
                            {
                                count = 0;
                            }

                            pos.width = w;
                        }

                        //Console.WriteLine(pos.UpperHeight);
                        //Console.WriteLine(pos.LowerHeight);
                        //Console.WriteLine(pos.x);
                        //Console.WriteLine(pos.y);
                        //Console.WriteLine(pos.rect.ToString());
                        //for (int l = i; l < tempWidth; l++)
                        //{
                        //    var tempPixel = ImageObj.Image.GetPixel(l, j);
                        //    var tempColor = ImageObj.GetColorName(tempPixel.R, tempPixel.G, tempPixel.B);
                        //    if (tempColor != color)
                        //    {
                        //        pos.width = l;
                        //        Console.WriteLine(l);
                        //        break;
                        //    }
                        //}
                        //for (int k = j; k < tempHeight; k++)
                        //{
                        //    var tempPixel = ImageObj.Image.GetPixel(i, k);
                        //    var tempColor = ImageObj.GetColorName(tempPixel.R, tempPixel.G, tempPixel.B);
                        //    if (tempColor != color)
                        //    {
                        //        pos.height = k; 
                        //        Console.WriteLine(k);
                        //        break;
                        //    }
                        //}
                        if (pos.UpperHeight == tempHeight)
                            pos.UpperHeight = 0;
                        if (Positions.ContainsKey(color))
                        {
                            //Positions[color].Add(pos);
                        }
                        else
                        {
                            Positions.Add(color, new List<CustomPosition> { pos });
                        }

                    }
                }
                var font = new Font(FontFamily.GenericSansSerif, 12);
                using (var g = Graphics.FromImage(img))
                {
                    foreach (var color in Positions.Keys)
                    {
                        foreach (var pos in Positions[color])
                        {
                            Console.WriteLine($"{color} - {pos.rect}");
                            g.DrawRectangle(Pens.Black, pos.rect);
                            g.DrawString(color.Name, font, Brushes.Black, pos.x + 10, pos.y - 50);
                        }
                    }
                    
                }
                //var fileName = location.Split('.')[0] + "Output.png";

                //img.Save(fileName);

            });
            return img;

        }
    }
    public class CustomImage
    {
        public Bitmap Image { get; set; }
        public Color GetColorName(int r, int g, int b)
        {
            if (r == 255 && g == 0 && b == 0)
            {
                return Color.Red;
            }
            else if (r == 0 && g == 0 && b == 255)
            {
                return Color.Blue;
            }
            else if (r == 0 && g == 255 && b == 0)
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
            else if (r == 255 && g == 255 && b == 0)
            {
                return Color.Yellow;
            }
            else if (r == 255 && g == 127 && b == 0)
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
