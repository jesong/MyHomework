namespace MyHomework.WebApp.Controllers
{
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using Middlewares.WeChatAuthenticationMiddlewares;
    using Models;
    using System.Threading.Tasks;
    using WeChat;

    [AllowAnonymous]
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Welcome(string returnUrl = null)
        {
            if (!Url.IsLocalUrl(returnUrl))
            {
                returnUrl = "/";
            }

            if (WeChatUtilities.IsWeChatInternalBrowser(HttpContext.Request) &&
                !HttpContext.User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(WeChatAuthenticationDefaults.AuthenticationScheme,
                    new AuthenticationProperties {
                        RedirectUri = string.Equals(returnUrl, "/", System.StringComparison.OrdinalIgnoreCase)
                            ? "HomeworkPublish/Index" : returnUrl
                    });
            }

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return new RedirectToActionResult("Index", "HomeworkPublish", null);
            }

            return View(new WelcomeViewModel(HttpContext, returnUrl));
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (!Url.IsLocalUrl(returnUrl))
            {
                returnUrl = "/";
            }

            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(WeChatAuthenticationDefaults.AuthenticationScheme,
                    new AuthenticationProperties { RedirectUri = returnUrl });
            }

            return Redirect(returnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            if (!Url.IsLocalUrl(returnUrl))
            {
                returnUrl = "/";
            }

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                await HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            return Redirect(returnUrl);
        }

        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            return View();
        }
    }
}
