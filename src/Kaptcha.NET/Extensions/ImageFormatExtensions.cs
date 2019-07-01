using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;

namespace KaptchaNET.Extensions
{
    public static class ImageFormatExtensions
    {
        private static readonly Dictionary<Guid, string> _knownImageFormats =
               (from p in typeof(ImageFormat).GetProperties(BindingFlags.Static | BindingFlags.Public)
                where p.PropertyType == typeof(ImageFormat)
                let value = (ImageFormat)p.GetValue(null, null)
                select new { value.Guid, Name = value.ToString() })
               .ToDictionary(p => p.Guid, p => p.Name);

        public static string GetImageFormatName(this ImageFormat format)
        {
            return _knownImageFormats.TryGetValue(format.Guid, out string name) ? name : null;
        }
    }
}
