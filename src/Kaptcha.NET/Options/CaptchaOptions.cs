using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace KaptchaNET.Options
{
    public class CaptchaOptions
    {
        /// <summary>
        /// The width of the captcha in pixel.
        /// </summary>
        public int Width { get; set; } = 250;

        /// <summary>
        /// The height of the captcha in pixel.
        /// </summary>
        public int Height { get; set; } = 100;

        /// <summary>
        /// The minimum length of the word.
        /// </summary>
        public int MinWordLength { get; set; } = 5;

        /// <summary>
        /// The maximum length of the word.
        /// </summary>
        public int MaxWordLength { get; set; } = 8;

        /// <summary>
        /// The color of the background.
        /// </summary>
        public Color BackgroundColor { get; set; } = Color.White;

        /// <summary>
        /// The color of the foreground.
        /// </summary>
        public Color ForegroundColor { get; set; } = Color.Black;

        /// <summary>
        /// Gets or sets the internal image size factor (for better image quality).
        /// </summary>
        public float Scale { get; set; } = 2;

        /// <summary>
        /// The maximum letter rotation clockwise in degree.
        /// </summary>
        public int MaxRotationAngle { get; set; } = 30;

        /// <summary>
        /// The minimum letter rotation counter clockwise in degree.
        /// </summary>
        public int MinRotationAngle { get; set; } = -30;

        /// <summary>
        /// Timeout for captcha validation.
        /// </summary>
        public TimeSpan Timeout { get; set; } = new TimeSpan(0, 5, 0);

        /// <summary>
        /// Image format for the captcha.
        /// </summary>
        public ImageFormat ImageFormat { get; set; } = ImageFormat.Png;

        /// <summary>
        /// Charset used for text generation.
        /// </summary>
        public char[] Charset { get; set; } = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
    }
}
