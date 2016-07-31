namespace MyHomework.WebApp.Models
{
    using Microsoft.AspNetCore.Http;
    using WeChat;

    public abstract class ViewModelBase
    {
        protected HttpContext Context { get; private set; }
        public ViewModelBase(HttpContext context)
        {
            this.Context = context;
        }

        public bool IsInWeChatBrowser
        {
            get
            {
                return WeChatUtilities.IsWeChatInternalBrowser(this.Context.Request);
            }
        }
    }
}
