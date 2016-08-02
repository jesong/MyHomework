﻿namespace MyHomework.WebApp.Controllers
{
    using Basic;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = Globals.AuthorizePolicyMember)]
    public class HomeworkPublishController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
