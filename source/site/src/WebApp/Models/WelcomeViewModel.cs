namespace MyHomework.WebApp.Models
{
    using Microsoft.AspNetCore.Http;

    public class WelcomeViewModel : ViewModelBase
    {
        public WelcomeViewModel(HttpContext context, string returnUrl) 
            : base(context)
        {
            ReturnUrl = returnUrl;
        }

        public string ReturnUrl { get; set; }
    }
}
