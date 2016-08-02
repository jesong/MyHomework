namespace MyHomework.WebApp.Models
{
    using Microsoft.AspNetCore.Http;

    public class HomeworkPublishViewModel : ViewModelBase
    {
        public HomeworkPublishViewModel(HttpContext context) 
            : base(context)
        {
        }
    }
}
