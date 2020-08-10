using System;
using System.Drawing;
using System.Drawing.Imaging;
using KaptchaNET.Options;

namespace KaptchaNET.Effects
{
    public class WaveEffect : IEffect
    {
        private readonly Random _rnd = new Random();
        private readonly CaptchaOptions _captchaOptions;

        public short Order => 0;

        public EffectType Type => EffectType.Foreground;

        public int Yperiod { get; set; } = 10;
        public int Yamplitude { get; set; } = 4;
        public int Xperiod { get; set; } = 10;
        public int Xamplitude { get; set; } = 8;

        public WaveEffect(CaptchaOptions captchaOptions) => _captchaOptions = captchaOptions;

        public Bitmap Apply(Bitmap image)
        {
            //int backColorArgb = _captchaOptions.BackgroundColor.ToArgb();
            //var dict = new Dictionary<Tuple<int, int>, int>();
            //float xp = _captchaOptions.Scale * Xperiod * _rnd.Next(1, 3);
            //float yp = _captchaOptions.Scale * Yperiod * _rnd.Next(1, 2);

            //image = image.Apply((x, y, src) =>
            //{

            //    int amplitudeX = 0;
            //    int amplitudeY = 0;
            //    var xt0 = new Tuple<int, int>(x, 0);
            //    var yt0 = new Tuple<int, int>(0, y);
            //    int k = _rnd.Next(0, 100);
            //    if (!dict.TryGetValue(xt0, out amplitudeX))
            //    {
            //        amplitudeX =  unchecked(Convert.ToInt32(Math.Sin(k + (x * 1.00 / xp)) * _captchaOptions.Scale * Xamplitude));
            //        dict.Add(xt0, amplitudeX);
            //    }
            //    if (!dict.TryGetValue(yt0, out amplitudeY))
            //    {
            //        amplitudeY = 0;// unchecked(Convert.ToInt32(Math.Sin(k + (y * 1.00 / yp)) * _captchaOptions.Scale * Yamplitude));
            //        dict.Add(yt0, amplitudeY);
            //    }

            //    return image.GetColorSample(x + amplitudeX, y + amplitudeY);
            //});
            //return image;




            int imageWidth = image.Width;
            int imageHeight = image.Height;
            int backColorArgb = _captchaOptions.BackgroundColor.ToArgb();
            BitmapData imageData = image.LockBits(new Rectangle(0, 0, imageWidth, imageHeight), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            try
            {
                unsafe
                {
                    int* imageDataPtr = (int*)imageData.Scan0;
                    {
                        // wave x
                        float xp = _captchaOptions.Scale * Xperiod * _rnd.Next(1, 3);
                        int k = _rnd.Next(0, 100);
                        for (int i = 1; i < imageWidth; i++)
                        {
                            int amplitude = unchecked(Convert.ToInt32(Math.Sin(k + (i * 1.00 / xp)) * _captchaOptions.Scale * Xamplitude));
                            for (int j = 0; j < imageHeight; j++)
                            {
                                bool setColor = j + amplitude >= 0 && j + amplitude < imageHeight;

                                imageDataPtr[(j * imageWidth) + i - 1] = setColor ? imageDataPtr[((j + amplitude) * imageWidth) + i] : backColorArgb;

                            }
                        }
                    }
                    {
                        // wave y
                        float yp = _captchaOptions.Scale * Yperiod * _rnd.Next(1, 2);
                        int k = _rnd.Next(0, 100);
                        for (int i = 1; i < imageHeight; i++)
                        {
                            int amplitude = unchecked(Convert.ToInt32(Math.Sin(k + (i * 1.00 / yp)) * _captchaOptions.Scale * Yamplitude));
                            for (int j = 0; j < imageWidth; j++)
                            {
                                bool setColor = j + amplitude >= 0 && j + amplitude < imageWidth;

                                imageDataPtr[((i - 1) * imageWidth) + j] = setColor ? imageDataPtr[(i * imageWidth) + j + amplitude] : backColorArgb;

                            }
                        }
                    }
                }
            }
            catch
            {
            }
            finally
            {
                image.UnlockBits(imageData);
            }
            return image;
        }
    }
}
