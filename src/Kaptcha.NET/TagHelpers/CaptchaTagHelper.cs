using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using KaptchaNET.Extensions;
using KaptchaNET.Services.CaptchaGenerator;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace KaptchaNET.TagHelpers
{
    [HtmlTargetElement("captcha", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class CaptchaTagHelper : TagHelper
    {
        protected readonly ICaptchaGeneratorService _generator;

        private const string CaptchaWidthAttributeName = "width";
        private const string CaptchaHeightAttributeName = "height";
        private const string CaptchaPlaceholderAttributeName = "placeholder";

        public static string CaptchaSolutionFieldName => "captchaSolution";
        public static string CaptchaIdFieldName => "captchaToken";

        /// <summary>
        /// The weight for the captcha image.
        /// </summary>
        [HtmlAttributeName(CaptchaWidthAttributeName)]
        public int? Width { get; set; }

        /// <summary>
        /// The height for the captcha image.
        /// </summary>
        [HtmlAttributeName(CaptchaHeightAttributeName)]
        public int? Height { get; set; }

        /// <summary>
        /// The placeholder text for the solution field.
        /// </summary>
        [HtmlAttributeName(CaptchaPlaceholderAttributeName)]
        public string Placeholder { get; set; } = "Captcha solution";

        /// <summary>
        /// Gets or sets the <see cref="Microsoft.AspNetCore.Mvc.Rendering.ViewContext" /> for the current request.
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public CaptchaTagHelper(ICaptchaGeneratorService generator)
        {
            _generator = generator;
        }

        public override void Init(TagHelperContext context)
        {
            base.Init(context);
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (!output.Content.IsModified)
            {
                output.TagName = "div";
                output.TagMode = TagMode.StartTagAndEndTag;

                string attrName = "class";
                string captchaGroup = "captchaGroup";
                if (output.Attributes.TryGetAttribute(attrName, out TagHelperAttribute attr))
                {
                    output.Attributes.SetAttribute(attr.Name, new HtmlString($"{captchaGroup} {attr.Value}"));
                }
                else
                {
                    output.Attributes.Add(attrName, captchaGroup);
                }

                var sb = new StringBuilder();

                string captchaSource = await WriteCaptcha();
                sb.Append(captchaSource);
                sb.Append(CreateFieldForm());
                output.Content.AppendHtml(sb.ToString());
            }
        }

        protected virtual string CreateFieldForm()
        {
            var sb = new StringBuilder();
            sb.Append("<div class=\"form-group\">");
            sb.Append($"<label id=\"{CaptchaSolutionFieldName}Label\" class=\"{CaptchaSolutionFieldName}Label\" for=\"{CaptchaSolutionFieldName}\">{Placeholder}</label>");
            sb.Append($"<input id=\"{CaptchaSolutionFieldName}\" class=\"form-control {CaptchaSolutionFieldName}\" name=\"{CaptchaSolutionFieldName}\" type=\"text\" placeholder=\"{Placeholder}\" value=\"\">");
            sb.Append("</div>");
            return sb.ToString();
        }

        protected virtual async Task<string> WriteCaptcha()
        {
            Captcha captcha = await _generator.CreateCaptchaAsync();
            string baseEncoded = string.Empty;
            using (var ms = new MemoryStream())
            {
                captcha.Image.Save(ms, _generator.Options.ImageFormat);
                byte[] b = ms.ToArray();
                string imageFormatString = _generator.Options.ImageFormat.GetImageFormatName().ToLower() ?? "png";
                baseEncoded = $"data:image/{imageFormatString};base64," + Convert.ToBase64String(b);
            }

            var sb = new StringBuilder();
            string sizeString = string.Empty;
            if (Width != null || Height != 0)
            {
                sizeString += $"width=\"{Width}\" ";
            }

            if (Height != null)
            {
                sizeString += $"height=\"{Height}\" ";
            }

            sb.Append($"<img id=\"captcha\" class=\"captcha\" {sizeString} alt=\"captcha\" src=\"{baseEncoded}\" />");
            sb.Append($"<input name=\"{CaptchaIdFieldName}\" type=\"hidden\" value=\"{captcha.Id}\">");
            return sb.ToString();
        }

        public override string ToString()
        {
            return nameof(CaptchaTagHelper);
        }
    }
}
