using System;
using System.Drawing;
using KaptchaNET.Options;

namespace KaptchaNET.Effects
{
    public class NoiseEffect : IEffect
    {
        private readonly Random _rnd = new Random();
        private readonly CaptchaOptions _captchaOptions;

        public short Order => 10;

        public EffectType Type => EffectType.Foreground;

        public NoiseEffect(CaptchaOptions captchaOptions)
        {
            _captchaOptions = captchaOptions;
        }

        public Bitmap Apply(Bitmap image)
        {
            using (var g = Graphics.FromImage(image))
            {
                int min = image.Width * image.Height * 2;
                int max = image.Width * image.Height * 4;
                int count = _rnd.Next(min, max) / 100;
                for (int i = 0; i < count; i++)
                {
                    int w = _rnd.Next(1, 4);
                    int h = _rnd.Next(1, 4);
                    var p = new Point(_rnd.Next(image.Width), _rnd.Next(image.Height));
                    g.DrawRectangle(new Pen(_captchaOptions.ForegroundColor), new Rectangle(p, new Size(w, h)));
                    g.FillRectangle(new SolidBrush(_captchaOptions.ForegroundColor), new Rectangle(p, new Size(w, h)));
                }
                g.Save();
            }
            return image;
        }
    }
}
