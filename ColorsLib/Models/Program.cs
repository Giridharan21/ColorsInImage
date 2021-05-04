using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace ColorsLib.Models
{
    public static class ExtensionMethods
    {
        public static bool ContainsColor(this IEnumerable<Color> list,Color c)
        {
            foreach (var i in list)
            {
                if (Program.CompareColors(i, c))
                    return true;
            }
            return false;
        }
    }
    
    public class Program
    {
        private static CustomImage ImageObj = new CustomImage();
        public static Dictionary<Color, List<CustomPosition>> Positions { get; set; } = new Dictionary<Color, List<CustomPosition>>();
        public static Color AdditionalColor { get; set; }
        public static async Task<Image> ProcessImage(string location,bool markInBlack = true , AdditionalColor additional = null)
        {
            ImageObj = new CustomImage();
            
            ImageObj.Image = new Bitmap(location);
            //Image img = System.Drawing.Image.FromFile(location);
            Positions = new Dictionary<Color, List<CustomPosition>>();
            AdditionalColor = additional?.colorCode ?? Color.Transparent;
            await Task.Run(async () =>
            {
                for (int i = 0; i < ImageObj.Image.Width; i++)
                {
                    for (int j = 0; j < ImageObj.Image.Height; j++)
                    {
                        var pixel = ImageObj.Image.GetPixel(i, j);
                        var color = ImageObj.GetColorName(pixel.R, pixel.G, pixel.B);
                        if (additional !=null && additional.colorCode == pixel)
                        {
                            if (Positions.ContainsKey(additional.colorCode))
                            {
                                if (IsMarkedRegion(i, j, Positions[additional.colorCode]))
                                    continue;
                                var pos = await GetCustomPos(i, j, additional.colorCode);
                                Positions[additional.colorCode].Add(pos);
                            }
                            else
                            {
                                var pos = await GetCustomPos(i, j, additional.colorCode);
                                Positions.Add(additional.colorCode, new List<CustomPosition> { pos });
                            }
                        }
                        if (color == Color.Transparent)
                            continue;
                        if (Positions.ContainsKey(color))
                        {
                            if(IsMarkedRegion(i,j,Positions[color]))
                                continue;
                            var pos = await GetCustomPos(i, j, color);
                            Positions[color].Add(pos);
                        }
                        else
                        {
                            var pos = await GetCustomPos(i, j, color);
                            Positions.Add(color, new List<CustomPosition> { pos });
                        }
                    }
                }
                var font = new Font(FontFamily.GenericSansSerif, 10);
                
                using (var g = Graphics.FromImage(ImageObj.Image))
                {
                    foreach (var color in Positions.Keys)
                    {
                        foreach (var pos in Positions[color])
                        {
                            Console.WriteLine($"{color} - {pos.Rect}");
                            g.DrawRectangle(new Pen(markInBlack? Color.Black:Color.LightGray), pos.Rect);
                            g.DrawString(color.Name, font, markInBlack ? Brushes.Black : Brushes.White, (float)(pos.X + 10), (float)(pos.Y - 50));
                        }
                    }

                }
                //var fileName = location.Split('.')[0] + "Output.png";

                //img.Save(fileName);

            });
            //return new Bitmap(location);
            return ImageObj.Image;

        }

        public static async Task<List<Color>> GetColors(string location)
        {
            var colors = new List<Color>();
            //Image img = System.Drawing.Image.FromFile(location);
            ImageObj = new CustomImage();
            await Task.Run(() =>
            {
                ImageObj.Image = new Bitmap(location);
                var w = ImageObj.Image.Size.Width;
                var h = ImageObj.Image.Size.Height;
                var count = GetCount((long)w *(long) h);
                for (int i = 0; i < ImageObj.Image.Width; i+=count)
                {
                    for (int j = 0; j < ImageObj.Image.Height; j++)
                    {
                        var pixel = ImageObj.Image.GetPixel(i, j);
                        if (!colors.ContainsColor(pixel))
                            colors.Add(pixel);


                    }
                }
            });
            //return new Bitmap(location);
            return colors;

        }

        private static int GetCount(long v)
        {

            if (v < 1000000)
                return 1;
            else if (v < 10000000)
                return 3;
            else if (v < 100000000)
                return 10;
            else if (v < 1000000000)
                return 50;
            else if (v < 10000000000)
                return 500;
            else if (v < 100000000000)
                return 1000;
            else if (v < 1000000000000)
                return 1500;
            else if (v < 10000000000000)
                return 3000;
            else if (v < 100000000000000)
                return 5000;
            else if (v < 1000000000000000)
                return 10000;
            else if (v < 10000000000000000)
                return 20000;
            else if (v < 100000000000000000)
                return 40000;
            else if (v < 1000000000000000000)
                return 50000;
            else
                return 80000;
        }
        public static bool CompareColors(Color c1,Color c2)
        {
            var r = Math.Abs(c1.R - c2.R);
            var g = Math.Abs(c1.G - c2.G);
            var b= Math.Abs(c1.B - c2.B);
            return r <= 20 && g <= 20 && b <= 20;

        }
        public static async Task<(Image img, Dictionary<Color, List<CustomPosition>> pos)> ProcessImage2(string location, bool markInBlack = true, int limit = 0, AdditionalColor additional = null)
        {
            ImageObj = new CustomImage();

            ImageObj.Image = new Bitmap(location);
            //Image img = System.Drawing.Image.FromFile(location);
            Positions = new Dictionary<Color, List<CustomPosition>>();
            var w = ImageObj.Image.Size.Width;
            var h = ImageObj.Image.Size.Height;
            var count = GetCount((long)w * (long)h);
            AdditionalColor = additional?.colorCode ?? Color.Transparent;
            int lc = 0;
            await Task.Run(async () =>
            {
                for (int i = 0; i < ImageObj.Image.Width; i+=count)
                {
                    for (int j = 0; j < ImageObj.Image.Height; j++)
                    {
                        
                        var pixel = ImageObj.Image.GetPixel(i, j);
                        var color = ImageObj.GetColorName(pixel.R, pixel.G, pixel.B);
                        if (additional != null && CompareColors(additional.colorCode ,pixel))
                        {
                            if (Positions.ContainsKey(additional.colorCode))
                            {
                                if (IsMarkedRegion(i, j, Positions[additional.colorCode]))
                                    continue;
                                var pos = await GetCustomPos(i, j, additional.colorCode);
                                Positions[additional.colorCode].Add(pos);
                                lc++;
                            }
                            else
                            {
                                var pos = await GetCustomPos(i, j, additional.colorCode);
                                Positions.Add(additional.colorCode, new List<CustomPosition> { pos });
                                lc++;
                            }
                        }
                        if (lc >= limit)
                            break;
                        if (color == Color.Transparent || additional !=null)
                            continue;
                        if (Positions.ContainsKey(color))
                        {
                            if (IsMarkedRegion(i, j, Positions[color]))
                                continue;
                            var pos = await GetCustomPos(i, j, color);
                            Positions[color].Add(pos);
                            lc++;
                        }
                        else
                        {
                            var pos = await GetCustomPos(i, j, color);
                            Positions.Add(color, new List<CustomPosition> { pos });
                            lc++;
                        }
                        if (lc >= limit)
                            break;
                    }
                }
                
                

            });
            //return new Bitmap(location);
            return (ImageObj.Image, Positions);

        }

        public static async Task<(Image Img,Dictionary<Color, List<CustomPosition>> Pos)> ProcessImage2(string location, AdditionalColor additional = null)
        {
            ImageObj = new CustomImage();

            ImageObj.Image = new Bitmap(location);
            //Image img = System.Drawing.Image.FromFile(location);
            Positions = new Dictionary<Color, List<CustomPosition>>();
            AdditionalColor = additional?.colorCode ?? Color.Transparent;
            await Task.Run(async () =>
            {
                for (int i = 0; i < ImageObj.Image.Width; i++)
                {
                    for (int j = 0; j < ImageObj.Image.Height; j++)
                    {
                        var pixel = ImageObj.Image.GetPixel(i, j);
                        
                        var color = ImageObj.GetColorName(pixel.R, pixel.G, pixel.B);
                        if (additional != null && additional.colorCode == pixel)
                        {
                            if (Positions.ContainsKey(additional.colorCode))
                            {
                                if (IsMarkedRegion(i, j, Positions[additional.colorCode]))
                                    continue;
                                var pos = await GetCustomPos(i, j, additional.colorCode);
                                Positions[additional.colorCode].Add(pos);
                            }
                            else
                            {
                                var pos = await GetCustomPos(i, j, additional.colorCode);
                                Positions.Add(additional.colorCode, new List<CustomPosition> { pos });
                            }
                        }
                        if (color == Color.Transparent)
                            continue;
                        if (Positions.ContainsKey(color))
                        {
                            if (IsMarkedRegion(i, j, Positions[color]))
                                continue;
                            var pos = await GetCustomPos(i, j, color);
                            Positions[color].Add(pos);
                        }
                        else
                        {
                            var pos = await GetCustomPos(i, j, color);
                            Positions.Add(color, new List<CustomPosition> { pos });
                        }

                    }
                }
                
                //var fileName = location.Split('.')[0] + "Output.png";

                //img.Save(fileName);

            });
            //return new Bitmap(location);
            return (ImageObj.Image,Positions);

        }

        private static bool IsMarkedRegion(int x, int y, List<CustomPosition> positons)
        {
            for (int i = 0; i < positons.Count; i++)
            {
                var temp = positons[i].Rect;

                if (x >= temp.X-50 && x <= temp.X + temp.Width +50 && y >= temp.Y -50 && y <=temp.Y + temp.Height +50)
                    return true;
            }
            return false;
        }

        private static async Task<CustomPosition> GetCustomPos(int i,int j,Color color)
        {
            var pos = new CustomPosition();
            await Task.Run(() =>
            {
                pos.X = i;
                pos.Y = j;
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
                        if (tempPixel == AdditionalColor)
                            continue;
                        if (tempColor != color)
                        {
                            if (h1 != j)
                                check = true;
                            if (h1 > pos.LowerHeight)
                                pos.LowerHeight = h1;
                            break;
                        }
                    }
                    for (int h2 = j; h2 > 0; h2--)
                    {
                        var tempPixel = ImageObj.Image.GetPixel(w, h2);
                        var tempColor = ImageObj.GetColorName(tempPixel.R, tempPixel.G, tempPixel.B);
                        if (tempPixel == AdditionalColor)
                            continue;
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
                    pos.Width = w;
                }

                if (pos.UpperHeight == tempHeight)
                    pos.UpperHeight = 0;

                
            });
            return pos;
        }
    }
}
