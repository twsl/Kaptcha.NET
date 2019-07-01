using System.Drawing;

namespace KaptchaNET.Services.FontGenerator
{
    public interface IFontGeneratorService
    {
        Font GetFont(float scale);
        float GetSpacing(int width);
    }
}
