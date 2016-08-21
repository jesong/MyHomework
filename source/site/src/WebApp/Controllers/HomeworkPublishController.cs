namespace MyHomework.WebApp.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Net.Http.Headers;
    using MyHomework.WebApp.Basic;
    using MyHomework.WebApp.DatabaseModels;
    using MyHomework.WebApp.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using WeChat;

    [AllowAnonymous]
    //[Authorize(Policy = Globals.AuthorizePolicyMember)]
    public class HomeworkPublishController : Controller
    {
        private MyHomeworkDBContext myHomeworkDBContext;
        private BlobStorageManager blobStorageManager;
        private WeChatApi wechatApi;

        public HomeworkPublishController(MyHomeworkDBContext dbContext, BlobStorageManager blobStorageManager, WeChatApi wechatApi)
        {
            this.myHomeworkDBContext = dbContext;
            this.blobStorageManager = blobStorageManager;
            this.wechatApi = wechatApi;
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
            var messages = myHomeworkDBContext.Message.Include(message=>message.Attachment).
                OrderByDescending(message=>message.CreatedDateTime).ToList();
            return Ok(messages);
        }

        [HttpPost]
        public async Task<IActionResult> AddHomework(Message message)
        {
            //TODO: change to correct user
            message.CreatedBy = "super user";
            message.CreatedDateTime = DateTime.UtcNow;

            //myHomeworkDBContext.Message.Add(message);
            //myHomeworkDBContext.SaveChanges();

            await wechatApi.SendMessage("有一条新作业", "有一条新作业发布了",
                new Uri("https://myhomeworkweb.azurewebsites.net/homeworkpublish/index"), null);

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
