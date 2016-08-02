namespace MyHomework.WebApp.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using Middlewares.WeChatAuthenticationMiddlewares;
    using Models;
    using WeChat;

    [AllowAnonymous]
    public class HomeController : Controller
    {
        public IActionResult Error()
        {
            return View();
        }
    }
}
