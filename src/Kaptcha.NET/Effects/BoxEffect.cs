using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using KaptchaNET.Options;

namespace KaptchaNET.Effects
{
    public class BoxEffect : IEffect
    {
        private readonly Random _rnd = new Random();
        private readonly CaptchaOptions _captchaOptions;

        public short Order => 20;

        public EffectType Type => EffectType.Background;

        public BoxEffect(CaptchaOptions captchaOptions)
        {
            _captchaOptions = captchaOptions;
        }

        public Bitmap Apply(Bitmap image)
        {
            using (var g = Graphics.FromImage(image))
            {
                var rectImage = new Bitmap(image.Width, image.Height);
                using (var gRect = Graphics.FromImage(rectImage))
                {
                    gRect.Clear(Color.Transparent);

                    int count = _rnd.Next(5, 10);
                    for (int i = 0; i < count; i++)
                    {
                        var p = new Point(_rnd.Next(image.Width), _rnd.Next(image.Height));

                        int h = _rnd.Next(1, image.Height / 5);
                        int w = _rnd.Next(1, image.Width / 5);
                        gRect.DrawRectangle(new Pen(Color.FromArgb(byte.MaxValue, _captchaOptions.ForegroundColor), _rnd.Next(1, 4)), new Rectangle(p, new Size(w, h)));

                        var matrix = new Matrix();
                        int degree = _rnd.Next(_captchaOptions.MinRotationAngle, _captchaOptions.MaxRotationAngle);

                        matrix.RotateAt(degree, new PointF(p.X + (w / 2), p.Y + (h / 2)), MatrixOrder.Append);
                        gRect.Transform = matrix;

                        gRect.Save();
                    }
                }
                g.DrawImage(rectImage, new Point(0, 0));
                g.Save();
            }
            return image;
        }
    }
}
