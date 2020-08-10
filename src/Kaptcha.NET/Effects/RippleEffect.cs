using System;
using System.Drawing;
using KaptchaNET.Extensions;
using KaptchaNET.Options;

namespace KaptchaNET.Effects
{
    public class RippleEffect : IEffect
    {
        private readonly Random _rnd = new Random();
        private readonly CaptchaOptions _captchaOptions;

        public short Order => 1;

        public EffectType Type => EffectType.Foreground;

        public int Yperiod { get; set; } = 10;
        public int Yamplitude { get; set; } = 4;
        public int Xperiod { get; set; } = 10;
        public int Xamplitude { get; set; } = 8;

        public RippleEffect(CaptchaOptions captchaOptions) => _captchaOptions = captchaOptions;

        public Bitmap Apply(Bitmap image)
        {
            float cent = (float)((_rnd.NextDouble() * 0.2) + 0.4);
            image = image.Apply((x, y, src) =>
            {
                float uvX = x / (float)image.Width;
                float uvY = y / (float)image.Height;

                float disToCen = (float)Math.Sqrt(((uvX - cent) * (uvX - cent)) + ((uvY - cent) * (uvY - cent)));
                uvX -= disToCen * 0.2f;
                uvY += (float)Math.Sin(uvX * 16) * 0.05f;

                uvX += 0.1f * _captchaOptions.Scale;

                return image.GetColorSample((int)(uvX * image.Width), (int)(uvY * image.Height));
            });
            return image;
        }
    }
}
