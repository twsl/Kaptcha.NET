using System.Threading.Tasks;
using KaptchaNET.Options;

namespace KaptchaNET.Services.CaptchaGenerator
{
    public interface ICaptchaGeneratorService
    {
        CaptchaOptions Options { get; }

        Task<Captcha> CreateCaptchaAsync();
    }
}
