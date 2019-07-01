using System;
using System.Threading.Tasks;
using KaptchaNET.Exceptions;
using KaptchaNET.Options;
using KaptchaNET.Services.Storage;
using Microsoft.Extensions.Options;

namespace KaptchaNET.Services.Validation
{
    public class CaptchaValidationService : ICaptchaValidationService
    {
        private readonly ICaptchaStorageService _storage;
        private readonly CaptchaOptions _captchaOptions;

        public string ValidationMessage => "The captcha solution is invalid.";

        public CaptchaValidationService(ICaptchaStorageService storage, IOptions<CaptchaOptions> captchaOptions)
        {
            _storage = storage;
            _captchaOptions = captchaOptions?.Value;
        }

        public Task ValidateAsync(CaptchaParameters parameters, string solution)
        {
            Captcha captcha = _storage.GetCaptcha(parameters.CaptchaId);
            if (string.IsNullOrWhiteSpace(solution))
            {
                throw new CaptchaValidationException("Empty solution.");
            }
            TimeSpan diff = DateTime.Now - captcha.Created;
            if (diff > _captchaOptions.Timeout)
            {
                throw new CaptchaTimeoutException();
            }
            if (captcha.Solution != solution)
            {
                throw new CaptchaValidationException("Invalid solution.");
            }
            return Task.CompletedTask;
        }
    }
}
