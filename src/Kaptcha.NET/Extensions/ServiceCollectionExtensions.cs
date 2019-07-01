using KaptchaNET.Options;
using KaptchaNET.Services.CaptchaGenerator;
using KaptchaNET.Services.Effect;
using KaptchaNET.Services.FontGenerator;
using KaptchaNET.Services.KeyGenerator;
using KaptchaNET.Services.Storage;
using KaptchaNET.Services.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KaptchaNET.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCatpcha(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCatpchaOptions(configuration);
            services.AddCaptchaFontGenerator<FontGeneratorService>();
            services.AddCaptchaEffect<EffectGeneratorService>();
            services.AddCaptchaKeyGenerator<KeyGeneratorService>();
            services.AddCatpchaGenerator<CaptchaGeneratorService>();
            services.AddCaptchaStorage<CaptchaStorageService>();
            services.AddCaptchaValidator<CaptchaValidationService>();
            services.AddScoped(typeof(ValidateCaptchaFilter));
            return services;
        }

        public static IServiceCollection AddCatpchaOptions(this IServiceCollection services, IConfiguration configuration)
        {
            IConfigurationSection captchaOptions = configuration.GetSection(nameof(CaptchaOptions));
            if (captchaOptions != null)
            {
                services.Configure<CaptchaOptions>(captchaOptions);
            }

            IConfigurationSection fontOptions = configuration.GetSection(nameof(FontOptions));
            if (fontOptions != null)
            {
                services.Configure<FontOptions>(fontOptions);
            }

            IConfigurationSection effectOptions = configuration.GetSection(nameof(EffectOptions));
            if (effectOptions != null)
            {
                services.Configure<EffectOptions>(effectOptions);
            }

            return services;
        }

        public static IServiceCollection AddCatpchaGenerator<T>(this IServiceCollection services) where T : class, ICaptchaGeneratorService
        {
            services.AddSingleton<ICaptchaGeneratorService, T>();
            return services;
        }

        public static IServiceCollection AddCaptchaEffect<T>(this IServiceCollection services) where T : class, IEffectGeneratorService
        {
            services.AddSingleton<IEffectGeneratorService, T>();
            return services;
        }

        public static IServiceCollection AddCaptchaFontGenerator<T>(this IServiceCollection services) where T : class, IFontGeneratorService
        {
            services.AddSingleton<IFontGeneratorService, T>();
            return services;
        }

        public static IServiceCollection AddCaptchaKeyGenerator<T>(this IServiceCollection services) where T : class, IKeyGeneratorService
        {
            services.AddSingleton<IKeyGeneratorService, T>();
            return services;
        }

        public static IServiceCollection AddCaptchaStorage<T>(this IServiceCollection services) where T : class, ICaptchaStorageService
        {
            services.AddSingleton<ICaptchaStorageService, T>();
            return services;
        }

        public static IServiceCollection AddCaptchaValidator<T>(this IServiceCollection services) where T : class, ICaptchaValidationService
        {
            services.AddSingleton<ICaptchaValidationService, T>();
            return services;
        }
    }
}
