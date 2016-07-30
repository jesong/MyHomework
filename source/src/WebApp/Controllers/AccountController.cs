namespace MyHomework.WebApp.Controllers
{
    using Microsoft.AspNetCore.Http.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using Middlewares.WeChatAuthenticationMiddlewares;

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
        public IActionResult Logout(string returnUrl = null)
        {
            if (!Url.IsLocalUrl(returnUrl))
            {
                returnUrl = "/";
            }

            return Redirect(returnUrl);
        }
    }
}
