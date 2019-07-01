using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace KaptchaNET
{
    [Serializable]
    public class Captcha : ISerializable
    {
        public Guid Id { get; set; }

        public string Solution { get; set; }

        public DateTime Created { get; set; }

        public Image Image { get; set; }

        public Captcha()
        {
            Created = DateTime.Now;
        }

        protected Captcha(SerializationInfo info, StreamingContext context)
        {
            byte[] imageData = (byte[])info.GetValue(nameof(Image), typeof(byte[]));
            int height = (int)info.GetValue($"{nameof(Image)}{nameof(Image.Height)}", typeof(int));
            int width = (int)info.GetValue($"{nameof(Image)}{nameof(Image.Width)}", typeof(int));

            var bitmap = new Bitmap(width, height);
            BitmapData bitmapData =
                bitmap.LockBits(
                    new Rectangle(new Point(), bitmap.Size),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format24bppRgb);
            Marshal.Copy(imageData, 0, bitmapData.Scan0, imageData.Length);
            bitmap.UnlockBits(bitmapData);

            Image = bitmap;

            Solution = info.GetString(nameof(Solution));
            Created = info.GetDateTime(nameof(Created));
            Id = Guid.Parse(info.GetString(nameof(Id)));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var bitmap = Image as Bitmap;
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(new Point(), bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            int byteCount = bitmapData.Stride * bitmap.Height;
            byte[] bitmapBytes = new byte[byteCount];
            Marshal.Copy(bitmapData.Scan0, bitmapBytes, 0, byteCount);
            bitmap.UnlockBits(bitmapData);

            info.AddValue(nameof(Image), bitmapBytes);
            info.AddValue($"{nameof(Image)}{nameof(Image.Height)}", bitmap.Height);
            info.AddValue($"{nameof(Image)}{nameof(Image.Width)}", bitmap.Width);


            info.AddValue(nameof(Created), Created);
            info.AddValue(nameof(Solution), Solution);
            info.AddValue(nameof(Id), Id);
        }
    }
}
