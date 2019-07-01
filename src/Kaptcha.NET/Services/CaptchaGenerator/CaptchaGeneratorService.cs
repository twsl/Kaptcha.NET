using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Threading.Tasks;
using KaptchaNET.Options;
using KaptchaNET.Services.Effect;
using KaptchaNET.Services.FontGenerator;
using KaptchaNET.Services.KeyGenerator;
using KaptchaNET.Services.Storage;
using Microsoft.Extensions.Options;

namespace KaptchaNET.Services.CaptchaGenerator
{
    public class CaptchaGeneratorService : ICaptchaGeneratorService
    {
        private readonly ICaptchaStorageService _storage;
        private readonly IKeyGeneratorService _key;
        private readonly IFontGeneratorService _font;
        private readonly IEffectGeneratorService _effect;
        private static readonly Random _rnd = new Random();

        public CaptchaOptions Options { get; }

        public CaptchaGeneratorService(ICaptchaStorageService storage, IOptions<CaptchaOptions> captchaOptions,
            IKeyGeneratorService key, IFontGeneratorService font, IEffectGeneratorService effect)
        {
            _storage = storage;
            Options = captchaOptions?.Value;
            _key = key;
            _font = font;
            _effect = effect;
        }

        public Task<Captcha> CreateCaptchaAsync()
        {
            string text = _key.GenerateKey();

            Bitmap image = CreateImage(text);
            var captcha = new Captcha
            {
                Id = Guid.NewGuid(),
                Created = DateTime.Now,
                Solution = text,
                Image = image
            };
            _storage.SaveCaptcha(captcha);
            return Task.FromResult(captcha);
        }

        public Bitmap CreateImage(string text)
        {
            int lettersMissing = Options.MaxWordLength - text.Length;
            float fontSizefactor = 1 + (lettersMissing * 0.1F);
            float scale = Options.Scale * fontSizefactor;
            Font font = _font.GetFont(scale);

            List<Image> letters = GetTextAsImageList(text, font);
            double width = Math.Ceiling(letters.Sum(l => _font.GetSpacing(l.Width)));

            SizeF size = MeasureString(text, font);

            float scaledWidth = Options.Width * Options.Scale;

            Debug.Assert(width <= scaledWidth);

            Bitmap image = ImageAllocate();
            using (var graphics = Graphics.FromImage(image))
            {
                using (var foreBrush = new SolidBrush(Options.ForegroundColor))
                {
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    //graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

                    image = _effect.ApplyBackgroundEffects(image);

                    float lastLetterLeft = (float)((scaledWidth / 2) - (width / 2));
                    for (int i = 0; i < letters.Count; i++)
                    {
                        Image letter = letters[i];

                        Image character = GetDebugImage(letter);

                        float newWidth = _font.GetSpacing(character.Width);
                        float offset = (newWidth - character.Width) / 2;
                        int startingPoint = (int)Math.Round(lastLetterLeft + offset);

                        var matrix = new Matrix();
                        int degree = _rnd.Next(Options.MinRotationAngle, Options.MaxRotationAngle);

                        matrix.RotateAt(degree, new PointF(startingPoint + (character.Width / 2), image.Height / 2), MatrixOrder.Append);
                        graphics.Transform = matrix;

                        int twentyPercentHeight = character.Height / 5;
                        int imageOffset = _rnd.Next(-twentyPercentHeight, twentyPercentHeight);
                        graphics.DrawImage(character, new Point(startingPoint, (image.Height / 2) - (character.Height / 2) + imageOffset));
                        lastLetterLeft += newWidth;
                    }
                    graphics.Save();

                    image = _effect.ApplyForegroundEffects(image);

                    return image;
                }
            }
        }

        protected virtual Bitmap ImageAllocate()
        {
            int imageWidth = (int)Math.Ceiling(Options.Width * Options.Scale);
            int imageHeight = (int)Math.Ceiling(Options.Height * Options.Scale);
            var image = new Bitmap(imageWidth, imageHeight);
            using (var graphics = Graphics.FromImage(image))
            {
                using (var sb = new SolidBrush(Options.BackgroundColor))
                {
                    graphics.FillRectangle(sb, 0, 0, imageWidth, imageHeight);
                }
            }

            return image;
        }

        private List<Image> GetTextAsImageList(string text, Font font)
        {
            var letters = new List<Image>();
            for (int i = 0; i < text.Length; i++)
            {
                var letterSize = MeasureString(text[i].ToString(), font).ToSize();
                Image image = new Bitmap(letterSize.Width, letterSize.Height);
                var g = Graphics.FromImage(image);

                g.SmoothingMode = SmoothingMode.HighQuality;
                //g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;

                g.DrawString(text[i].ToString(), font, new SolidBrush(Options.ForegroundColor), 0, 0, StringFormat.GenericTypographic);
                letters.Add(image);
            }
            return letters;
        }

        private Image GetDebugImage(Image image)
        {
            Image result = new Bitmap(image);
            var g = Graphics.FromImage(result);
#if DEBUG
            int lineWidth = (int)Math.Truncate(Options.Scale);
            using (var pen = new Pen(Color.Red, lineWidth))
            {
                g.DrawRectangle(pen, new Rectangle(0, 0, image.Width - 1, image.Height - 1));
                g.DrawRectangle(pen, image.Width / 2, image.Height / 2, 1, 1);
            }
#endif
            return result;
        }

        private static SizeF MeasureString(string s, Font font)
        {
            SizeF result;
            using (var img = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(img))
                {
                    g.TextRenderingHint = TextRenderingHint.AntiAlias;
                    result = g.MeasureString(s, font, int.MaxValue, StringFormat.GenericTypographic);
                }
            }
            return result;
        }
    }
}
