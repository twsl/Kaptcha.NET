using System;
using System.Diagnostics;
using System.Threading.Tasks;
using KaptchaNET.Services.CaptchaGenerator;
using KaptchaNET.Services.Storage;
using KaptchaNET.Services.Validation;
using KaptchaNET.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace KaptchaNET.Web.Controllers
{
    public class HomeController : KaptchaNET.Controllers.CaptchaController
    {
        public HomeController(ICaptchaGeneratorService generator, ICaptchaValidationService validation, ICaptchaStorageService storage)
            : base(generator, validation, storage)
        {
        }

        public override Task<IActionResult> CreateCaptcha()
        {
            return base.CreateCaptcha();
        }

        public override Task<IActionResult> GetCaptcha(Guid id)
        {
            return base.GetCaptcha(id);
        }

        public override Task<IActionResult> ValidateCaptcha(Guid id, string solution)
        {
            return base.ValidateCaptcha(id, solution);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateCaptcha]
        public IActionResult Test(TestViewModel model)
        {
            if (ModelState.IsValid)
            {
                ViewData["TestValue"] = model?.TestValue;
                ViewData["TestValueLink"] = model?.TestValueLink;
                ModelState.Clear();
            }
            return View(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
