namespace MyHomework.WebApp.Controllers
{
    using Basic;
    using DatabaseModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Net.Http.Headers;
    using Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [AllowAnonymous]
    //[Authorize(Policy = Globals.AuthorizePolicyMember)]
    public class HomeworkPublishController : Controller
    {
        private MyHomeworkDBContext myHomeworkDBContext;
        private BlobStorageManager blobStorageManager;
        public HomeworkPublishController(MyHomeworkDBContext context, BlobStorageManager blobStorageManager)
        {
            this.myHomeworkDBContext = context;
            this.blobStorageManager = blobStorageManager;
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

        [HttpPost]
        public async Task<IActionResult> UploadFile()
        {
            var files = Request.Form.Files;
            List<dynamic> resultList = new List<dynamic>();
            foreach (var file in files)
            {
                var fileName = ContentDispositionHeaderValue
                                .Parse(file.ContentDisposition)
                                .FileName
                                .Trim('"');
                

                var url = await blobStorageManager.Upload(file, fileName);

                resultList.Add(new { fileName = fileName, url = url.AbsoluteUri });
            }

            return new JsonResult(resultList);
        }
    }
}
