using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using SmartElectionFrontend.Models;

namespace SmartElectionFrontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly Client client = new Client("http://localhost:56010", new System.Net.Http.HttpClient());
        public static string UserId { get; private set; } = "";
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        public async Task<IActionResult> PostLogin(LoginViewModel model)
        {
            model.RememberMe = false;
            try
            {
                var user = await client.PostLoginAsyncAsync(model);
                UserId = user;
                return RedirectToAction("Index");
            }
            catch (SwaggerException)
            {
                return RedirectToAction("Login");
            }
        }

        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        public async Task<IActionResult> Logout()
        {
            await client.LogOutAsync();
            UserId = "";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EditInfo()
        {
            var user = await client.GetUserAsync(UserId);
            return View(new EditViewModel()
            {
                Country = user.Country,
                Email = user.Email,
                Id = user.Id
            });
        }

        public async Task<IActionResult> PutEditInfo(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                await client.PutUserAsync(model.Id, model);
                return RedirectToAction("Index");
            }
            return RedirectToAction("EditInfo");
        }

        public async Task<IActionResult> PostRegister(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await client.PostRegisterAsyncAsync(model);
                UserId = user;
                return RedirectToAction("Index");
            }
            return RedirectToAction("Register");
        }
    }
}
