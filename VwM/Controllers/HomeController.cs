using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VwM.Extensions;
using VwM.ViewModels;

namespace VwM.Controllers
{
    public class HomeController : MyController<HomeController>
    {
        private readonly string _requestCookieName;
        private readonly Authorization.IContext _authCtx;


        #region ctor
        public HomeController(
            ILogger<HomeController> logger,
            IStringLocalizer<HomeController> localizer,
            IOptions<RequestLocalizationOptions> requestLczOptions,
            Authorization.IContext authenticationContext) : base (logger, localizer)
        {
            var cookieCultureProvider = requestLczOptions.Value
                .RequestCultureProviders
                .OfType<CookieRequestCultureProvider>()
                .First();
            _requestCookieName = cookieCultureProvider.CookieName;
            _authCtx = authenticationContext;
        }
        #endregion


        public IActionResult Index()
        {
            return View();
        }


        public IActionResult AccessDenied()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userEntry = _authCtx.GetUser(model.Username, model.Password);

            if (userEntry == null)
            {
                ModelState.AddModelError("", _lcz["InvalidLoginPassword"]);
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.WindowsAccountName, userEntry.Login),
                new Claim(ClaimTypes.Name, userEntry.DisplayName),
                new Claim(ClaimTypes.Role, userEntry.Role)
            };
            // TODO: add roles https://gunnarpeipman.com/aspnet-core-custom-authentication/
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                IsPersistent = model.RememberMe,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);

            _logger.LogInformation($"User '{userEntry.Login}' logged in.");

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(SanitizeReturnUrl(returnUrl));

            return RedirectToAction("Index");
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            var login = User.GetLogin();
            _logger.LogInformation($"User '{login}' logged out.");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }


        // https://stackoverflow.com/a/45283580
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(culture))
                culture = "en-US";

            Response.Cookies.Append(
                _requestCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = false,
                    HttpOnly = false
                }
            );

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(SanitizeReturnUrl(returnUrl));

            return RedirectToAction("Index");
        }
    }
}
