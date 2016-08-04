namespace MyHomework.WebApp.Controllers
{
    using Basic;
    using DatabaseModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using System.Linq;

    [AllowAnonymous]
    //[Authorize(Policy = Globals.AuthorizePolicyMember)]
    public class HomeworkPublishController : Controller
    {
        private MyHomeworkDBContext myHomeworkDBContext;
        public HomeworkPublishController(MyHomeworkDBContext context)
        {
            myHomeworkDBContext = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = new HomeworkPublishViewModel(HttpContext);

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult GetHomeworks()
        {
            return new JsonResult(myHomeworkDBContext.Message.ToList());
        }

        [HttpPost]
        public IActionResult AddHomework(Message message)
        {
            return new JsonResult(null);
        }
    }
}
