using System.Drawing;

namespace KaptchaNET.Services.Effect
{
    public interface IEffectGeneratorService
    {
        Bitmap ApplyForegroundEffects(Bitmap image);

        Bitmap ApplyBackgroundEffects(Bitmap image);
    }
}
