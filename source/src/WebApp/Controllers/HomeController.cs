namespace MyHomework.WebApp.Controllers
{
    using Microsoft.AspNetCore.Http.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using Middlewares.WeChatAuthenticationMiddlewares;
    using Models;
    using WeChat;

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if(WeChatUtilities.IsWeChatInternalBrowser(HttpContext.Request) &&
                !HttpContext.User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(WeChatAuthenticationDefaults.AuthenticationScheme,
                    new AuthenticationProperties { RedirectUri = "/" });
            }
            return View(new HomeViewModel(HttpContext));
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
