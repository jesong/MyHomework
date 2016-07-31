namespace MyHomework.WebApp.Models
{
    using Microsoft.AspNetCore.Http;

    public class HomeViewModel : ViewModelBase
    {
        public HomeViewModel(HttpContext context) 
            : base(context)
        {
        }
    }
}
