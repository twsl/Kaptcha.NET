using System;
using System.Drawing;
using KaptchaNET.Options;

namespace KaptchaNET.Effects
{
    public class LineEffect : IEffect
    {
        private readonly Random _rnd = new Random();
        private readonly CaptchaOptions _captchaOptions;

        public short Order => 15;

        public EffectType Type => EffectType.Foreground | EffectType.Background;

        public LineEffect(CaptchaOptions captchaOptions)
        {
            _captchaOptions = captchaOptions;
        }

        public Bitmap Apply(Bitmap image)
        {
            using (var g = Graphics.FromImage(image))
            {
                int count = _rnd.Next(3, 6);
                for (int i = 0; i < count; i++)
                {
                    var p1 = new Point(_rnd.Next(image.Width), _rnd.Next(image.Height));
                    var p2 = new Point(_rnd.Next(image.Width), _rnd.Next(image.Height));
                    int lineWidth = _rnd.Next(1, 6);
                    using (var pen = new Pen(_captchaOptions.ForegroundColor, lineWidth))
                    {
                        g.DrawLine(pen, p1, p2);
                    }
                }
                g.Save();
            }
            return image;
        }
    }
}
