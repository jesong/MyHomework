namespace MyHomework.WebApp.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Net.Http.Headers;
    using MyHomework.WebApp.Basic;
    using MyHomework.WebApp.DatabaseModels;
    using MyHomework.WebApp.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [AllowAnonymous]
    //[Authorize(Policy = Globals.AuthorizePolicyMember)]
    public class HomeworkPublishController : Controller
    {
        private MyHomeworkDBContext myHomeworkDBContext;
        private BlobStorageManager blobStorageManager;
        public HomeworkPublishController(MyHomeworkDBContext dbContext, BlobStorageManager blobStorageManager)
        {
            this.myHomeworkDBContext = dbContext;
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
            //TODO: change to correct user
            message.CreatedBy = "super user";
            message.CreatedDateTime = DateTime.UtcNow;

            myHomeworkDBContext.Message.Add(message);
            myHomeworkDBContext.SaveChanges();

            return Ok();
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

            return Ok(resultList);
        }
    }
}
