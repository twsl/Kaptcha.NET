using System.Drawing;

namespace KaptchaNET.Options
{
    public class FontOptions
    {
        /// <summary>
        /// The relative space between characters.
        /// </summary>
        public float Spacing { get; set; } = 1.25f;

        /// <summary>
        /// The minimum font size.
        /// </summary>
        public int MinSize { get; set; } = 22;

        /// <summary>
        /// The maximum font size.
        /// </summary>
        public int MaxSize { get; set; } = 30;

        /// <summary>
        /// The font family.
        /// </summary>
        public FontFamily FontFamily { get; set; } = new FontFamily("Arial");

        /// <summary>
        /// The font style.
        /// </summary>
        public FontStyle FontStyle { get; set; } = FontStyle.Regular;
    }
}
