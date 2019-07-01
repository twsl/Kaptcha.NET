using System.Drawing;

namespace KaptchaNET.Effects
{
    public interface IEffect
    {
        short Order { get; }

        EffectType Type { get; }

        Bitmap Apply(Bitmap image);
    }
}
