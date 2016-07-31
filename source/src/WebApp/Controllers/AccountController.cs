namespace MyHomework.WebApp.Controllers
{
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Http.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using Middlewares.WeChatAuthenticationMiddlewares;
    using System.Threading.Tasks;

    public class AccountController : Controller
    {
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
