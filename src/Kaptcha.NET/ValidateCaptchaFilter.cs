using System;
using System.Threading.Tasks;
using KaptchaNET.Exceptions;
using KaptchaNET.Services.Validation;
using KaptchaNET.TagHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace KaptchaNET
{
    public class ValidateCaptchaFilter : IAsyncAuthorizationFilter
    {
        private readonly ICaptchaValidationService _service;
        private readonly ILogger<ValidateCaptchaFilter> _logger;

        public ValidateCaptchaFilter(ICaptchaValidationService service, ILogger<ValidateCaptchaFilter> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (ShouldValidate(context))
            {
                void invalidResponse() => context.ModelState.AddModelError(CaptchaTagHelper.CaptchaSolutionFieldName, _service.ValidationMessage);

                try
                {
                    if (!context.HttpContext.Request.HasFormContentType)
                    {
                        throw new MissingFieldException("The request doesn't have the form content type.");
                    }

                    string remoteIp = context.HttpContext.Connection?.RemoteIpAddress?.ToString();
                    Microsoft.AspNetCore.Http.IFormCollection form = await context.HttpContext.Request.ReadFormAsync();
                    Microsoft.Extensions.Primitives.StringValues response = form[CaptchaTagHelper.CaptchaSolutionFieldName];
                    string token = form[CaptchaTagHelper.CaptchaIdFieldName];

                    var descriptor = context?.ActionDescriptor as ControllerActionDescriptor;

                    var id = Guid.Parse(token);
                    var parameters = new CaptchaParameters()
                    {
                        ActionName = descriptor.ActionName,
                        ControllerName = descriptor.ControllerName,
                        IpAddress = remoteIp,
                        CaptchaId = id
                    };
                    await _service.ValidateAsync(parameters, response);
                }
                catch (CaptchaValidationException ex)
                {
                    if (ex.InvalidResponse)
                    {
                        _logger.LogError(ex, ex.Message);
                        invalidResponse();
                        return;
                    }
                    else
                    {
                        _logger.LogError(ex, "Unknown validation issue.");
                        invalidResponse();
                    }
                }
                catch
                {
                    _logger.LogError(new Exception(), "Unknown issue.");
                    context.Result = new BadRequestResult();
                }
            }
        }

        protected bool ShouldValidate(AuthorizationFilterContext context)
        {
            return string.Equals("POST", context.HttpContext.Request.Method, StringComparison.OrdinalIgnoreCase);
        }
    }
}
