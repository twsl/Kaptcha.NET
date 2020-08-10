using System;
using System.Text;
using System.Threading.Tasks;
using KaptchaNET.Services.CaptchaGenerator;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace KaptchaNET.TagHelpers
{
    [HtmlTargetElement("captcha", TagStructure = TagStructure.NormalOrSelfClosing, Attributes = CaptchaIdAttributeName)]
    [HtmlTargetElement("captcha", TagStructure = TagStructure.NormalOrSelfClosing, Attributes = CaptchaLinkAttributeName)]

    [HtmlTargetElement("captcha", TagStructure = TagStructure.NormalOrSelfClosing, Attributes = ActionAttributeName)]
    [HtmlTargetElement("captcha", TagStructure = TagStructure.NormalOrSelfClosing, Attributes = AntiforgeryAttributeName)]
    [HtmlTargetElement("captcha", TagStructure = TagStructure.NormalOrSelfClosing, Attributes = AreaAttributeName)]
    [HtmlTargetElement("captcha", TagStructure = TagStructure.NormalOrSelfClosing, Attributes = PageAttributeName)]
    [HtmlTargetElement("captcha", TagStructure = TagStructure.NormalOrSelfClosing, Attributes = PageHandlerAttributeName)]
    [HtmlTargetElement("captcha", TagStructure = TagStructure.NormalOrSelfClosing, Attributes = FragmentAttributeName)]
    [HtmlTargetElement("captcha", TagStructure = TagStructure.NormalOrSelfClosing, Attributes = ControllerAttributeName)]
    [HtmlTargetElement("captcha", TagStructure = TagStructure.NormalOrSelfClosing, Attributes = RouteAttributeName)]
    public class CaptchaLinkTagHelper : CaptchaTagHelper
    {
        private const string CaptchaIdAttributeName = "captcha-id";
        private const string CaptchaLinkAttributeName = "captcha-link";
        private const string ActionAttributeName = "asp-action";
        private const string AntiforgeryAttributeName = "asp-antiforgery";
        private const string AreaAttributeName = "asp-area";
        private const string PageAttributeName = "asp-page";
        private const string PageHandlerAttributeName = "asp-page-handler";
        private const string FragmentAttributeName = "asp-fragment";
        private const string ControllerAttributeName = "asp-controller";
        private const string RouteAttributeName = "asp-route";

        protected IHtmlGenerator Generator { get; }

        /// <summary>
        /// The ID identifying the captcha.
        /// </summary>
        [HtmlAttributeName(CaptchaIdAttributeName)]
        public string CaptchaId { get; set; }

        /// <summary>
        /// The direct link to the captcha.
        /// </summary>
        [HtmlAttributeName(CaptchaLinkAttributeName)]
        public string CaptchaLink { get; set; }

        /// <summary>
        /// The name of the action method.
        /// </summary>
        [HtmlAttributeName(ActionAttributeName)]
        public string Action { get; set; }

        /// <summary>
        /// The name of the controller.
        /// </summary>
        [HtmlAttributeName(ControllerAttributeName)]
        public string Controller { get; set; }

        /// <summary>
        /// The name of the area.
        /// </summary>
        [HtmlAttributeName(AreaAttributeName)]
        public string Area { get; set; }

        /// <summary>
        /// The name of the page.
        /// </summary>
        [HtmlAttributeName(PageAttributeName)]
        public string Page { get; set; }

        /// <summary>
        /// The name of the page handler.
        /// </summary>
        [HtmlAttributeName(PageHandlerAttributeName)]
        public string PageHandler { get; set; }

        /// <summary>
        /// Gets or sets the URL fragment.
        /// </summary>
        [HtmlAttributeName(FragmentAttributeName)]
        public string Fragment { get; set; }

        /// <summary>
        /// Name of the route.
        /// </summary>
        /// <remarks>
        /// Must be <c>null</c> if <see cref="Action"/> or <see cref="Controller"/> is non-<c>null</c>.
        /// </remarks>
        [HtmlAttributeName(RouteAttributeName)]
        public string Route { get; set; }

        public CaptchaLinkTagHelper(ICaptchaGeneratorService generator, IHtmlGenerator htmlGenerator) : base(generator) => Generator = htmlGenerator;
        public override void Init(TagHelperContext context)
        {
            //base.Init(context);
        }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output) => base.ProcessAsync(context, output);

        protected override async Task<string> WriteCaptcha()
        {
            if (CaptchaId == null)
            {
                Captcha captcha = await _generator.CreateCaptchaAsync();
                CaptchaId = captcha.Id.ToString();
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

            string link = BuildCaptchaLink();
            sb.Append($"<img id=\"captcha\" class=\"captcha\" {sizeString} alt=\"captcha\" src=\"{link}\" />");
            sb.Append($"<input name=\"{CaptchaIdFieldName}\" type=\"hidden\" value=\"{CaptchaId}\">");
            return sb.ToString();
        }

        public virtual string BuildCaptchaLink()
        {
            bool notNull = Action != null ||
                Controller != null ||
                Area != null ||
                Page != null ||
                PageHandler != null ||
                Fragment != null ||
                Route != null;

            if (string.IsNullOrWhiteSpace(CaptchaLink) && notNull)
            {
                bool routeLink = Route != null;
                bool actionLink = Controller != null || Action != null;
                bool pageLink = Page != null || PageHandler != null;

                if ((routeLink && actionLink) || (routeLink && pageLink) || (actionLink && pageLink))
                {
                    string message = string.Join(
                        Environment.NewLine,
                        RouteAttributeName,
                        ControllerAttributeName + ", " + ActionAttributeName,
                        PageAttributeName);

                    throw new InvalidOperationException(message);
                }

                var routeValues = new RouteValueDictionary
                {
                    { "id", CaptchaId }
                };

                TagBuilder tagBuilder = pageLink
                    ? Generator.GeneratePageLink(ViewContext, string.Empty, Page, PageHandler, null, null, Fragment, routeValues, null)
                    : routeLink
                        ? Generator.GenerateRouteLink(ViewContext, string.Empty, Route, null, null, Fragment, routeValues, null)
                        : Generator.GenerateActionLink(ViewContext, string.Empty, Action, Controller, null, null, Fragment, routeValues, null);

                if (tagBuilder.Attributes.TryGetValue("href", out string link))
                {
                    return link;
                }
            }
            return CaptchaLink;
        }

        public override string ToString()
        {
            return nameof(CaptchaLinkTagHelper);
        }
    }
}
