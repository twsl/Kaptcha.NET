using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using KaptchaNET.Effects;
using KaptchaNET.Options;
using Microsoft.Extensions.Options;

namespace KaptchaNET.Services.Effect
{
    public class EffectGeneratorService : IEffectGeneratorService
    {
        private readonly List<IEffect> _effects;
        private readonly EffectOptions _effectOptions;
        private readonly CaptchaOptions _captchaOptions;

        public EffectGeneratorService(IOptions<EffectOptions> effectOptions, IOptions<CaptchaOptions> captchaOptions)
        {
            _effectOptions = effectOptions?.Value;
            _captchaOptions = captchaOptions?.Value;
            _effects = new List<IEffect>()
            {
                new BlobEffect(_captchaOptions),
                new BoxEffect(_captchaOptions),
                new LineEffect(_captchaOptions),
                new NoiseEffect(_captchaOptions),
                new WaveEffect(_captchaOptions),
                new RippleEffect(_captchaOptions)
            };
            _effects = _effects.Where(e => _effectOptions.TryGetValue(e.GetType().Name, out bool enabled) ? enabled : false).ToList();
            _effects.Sort((a, b) => a.Order.CompareTo(b.Order));
        }

        public Bitmap ApplyBackgroundEffects(Bitmap image)
        {
            IEnumerable<IEffect> effects = _effects.Where(e => (e.Type & EffectType.Background) == EffectType.Background);
            foreach (IEffect effect in effects)
            {
                image = effect.Apply(image);
            }
            return image;
        }

        public Bitmap ApplyForegroundEffects(Bitmap image)
        {
            IEnumerable<IEffect> effects = _effects.Where(e => (e.Type & EffectType.Foreground) == EffectType.Foreground);
            foreach (IEffect effect in effects)
            {
                image = effect.Apply(image);
            }
            return image;
        }
    }
}
