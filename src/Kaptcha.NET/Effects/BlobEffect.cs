using System;
using System.Drawing;
using KaptchaNET.Options;

namespace KaptchaNET.Effects
{
    public class BlobEffect : IEffect
    {
        private readonly Random _rnd = new Random();
        private readonly CaptchaOptions _captchaOptions;

        public short Order => 5;

        public EffectType Type => EffectType.Background;

        public BlobEffect(CaptchaOptions captchaOptions) => _captchaOptions = captchaOptions;

        public Bitmap Apply(Bitmap image)
        {
            using (var g = Graphics.FromImage(image))
            {
                int count = _rnd.Next(2, 7);
                for (int i = 0; i < count; i++)
                {
                    int randX = _rnd.Next(image.Width / 20, image.Width / 2);
                    int randY = _rnd.Next(image.Height / 20, image.Height / 2);

                    int minHeight = image.Height / 20;
                    int minWidth = image.Width / 20;
                    int maxHeight = image.Height / 5;
                    int maxWidth = image.Width / 5;

                    int randH = _rnd.Next(minHeight, maxHeight);
                    int randW = _rnd.Next(minWidth, maxWidth);
                    g.FillEllipse(new SolidBrush(Color.FromArgb(150, _captchaOptions.ForegroundColor)), randX, randY, randW, randH);
                }
                g.Save();
            }
            return image;
        }
    }
}
