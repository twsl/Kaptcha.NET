using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using KaptchaNET.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace KaptchaNET.Services.Storage
{
    public class CaptchaStorageService : ICaptchaStorageService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly CaptchaOptions _captchaOptions;
        private readonly DistributedCacheEntryOptions _cacheOptions;

        public CaptchaStorageService(IDistributedCache distributedCache, IOptions<CaptchaOptions> captchaOptions)
        {
            _distributedCache = distributedCache;
            _captchaOptions = captchaOptions?.Value;
            _cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _captchaOptions.Timeout
            };
        }

        public Guid SaveCaptcha(Captcha captcha)
        {
            byte[] buffer = ObjectToByteArray(captcha);
            _distributedCache.Set(captcha?.Id.ToString(), buffer, _cacheOptions);
            return captcha.Id;
        }

        public async Task<Guid> SaveCaptchaAsync(Captcha captcha)
        {
            byte[] buffer = ObjectToByteArray(captcha);
            await _distributedCache.SetAsync(captcha.Id.ToString(), buffer, _cacheOptions);
            return captcha.Id;
        }

        public Captcha GetCaptcha(Guid id)
        {
            byte[] buffer = _distributedCache.Get(id.ToString());
            var captcha = (Captcha)ByteArrayToObject(buffer);
            return captcha;
        }

        public async Task<Captcha> GetCaptchaAsync(Guid id)
        {
            byte[] buffer = await _distributedCache.GetAsync(id.ToString());
            var captcha = (Captcha)ByteArrayToObject(buffer);
            return captcha;
        }

        private static byte[] ObjectToByteArray(object obj)
        {
            var formatter = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, obj);
                byte[] result = ms.ToArray();
                return result;
            }
        }

        private static object ByteArrayToObject(byte[] arrBytes)
        {
            var formatter = new BinaryFormatter();
            using (var memStream = new MemoryStream())
            {
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                object obj = formatter.Deserialize(memStream);
                return obj;
            }
        }
    }
}
