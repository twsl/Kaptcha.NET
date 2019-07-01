using System;
using System.Drawing;

namespace KaptchaNET.Extensions
{
    public static class BitmapExtensions
    {
        public static Bitmap Apply(this Bitmap btmp, Func<int, int, Color, Color> func)
        {
            var img = btmp.Clone() as Bitmap;
            for (int y = 0; y < img.Height; ++y)
            {
                for (int x = 0; x < img.Width; ++x)
                {
                    Color z = img.GetPixel(x, y); // old
                    Color c = func(x, y, z); // new
                    img.SetPixel(x, y, c);
                }
            }
            return img;
        }

        public static Color GetColorSample(this Bitmap img, int x, int y)
        {
            int uvx = TrimValue(x, 0, img.Width - 1);
            int uvy = TrimValue(y, 0, img.Height - 1);
            return img.GetPixel(uvx, uvy);
        }

        private static int TrimValue(int x, int min, int max)
        {
            if (x < min)
            {
                return min;
            }
            else if (x > max)
            {
                return max;
            }
            else
            {
                return x;
            }
        }

    }
}
