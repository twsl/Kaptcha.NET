using System;
using System.IO;
using System.Threading.Tasks;
using KaptchaNET.Services.CaptchaGenerator;
using KaptchaNET.Services.Storage;
using KaptchaNET.Services.Validation;
using Microsoft.AspNetCore.Mvc;

namespace KaptchaNET.Controllers
{
    public class CaptchaController : Controller
    {
        private readonly ICaptchaGeneratorService _generator;
        private readonly ICaptchaValidationService _validation;
        private readonly ICaptchaStorageService _storage;

        public CaptchaController(ICaptchaGeneratorService generator, ICaptchaValidationService validation, ICaptchaStorageService storage)
        {
            _generator = generator;
            _validation = validation;
            _storage = storage;
        }

        public virtual async Task<IActionResult> CreateCaptcha()
        {
            Captcha captcha = await _generator.CreateCaptchaAsync();
            return Ok(captcha.Id);
        }

        public virtual async Task<IActionResult> GetCaptcha(Guid id)
        {
            Captcha captcha = await _storage.GetCaptchaAsync(id);
            using (var ms = new MemoryStream())
            {
                captcha.Image.Save(ms, _generator.Options.ImageFormat);
                byte[] b = ms.ToArray();
                string imageFormatHeader = $"image/{_generator.Options.ImageFormat.ToString().ToLower()}";
                return File(b, imageFormatHeader);
            }
        }

        public virtual async Task<IActionResult> ValidateCaptcha(Guid id, string solution)
        {
            try
            {
                var parameters = new CaptchaParameters
                {
                    CaptchaId = id,
                    ControllerName = nameof(CaptchaController),
                    ActionName = nameof(ValidateCaptcha),
                    IpAddress = Request.HttpContext.Connection?.RemoteIpAddress?.ToString()

                };
                await _validation.ValidateAsync(parameters, solution);
            }
            catch
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
