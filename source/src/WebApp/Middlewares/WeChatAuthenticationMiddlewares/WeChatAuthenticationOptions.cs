namespace MyHomework.WebApp.Middlewares.WeChatAuthenticationMiddlewares
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Options;

    public class WeChatAuthenticationOptions : AuthenticationOptions, IOptions<WeChatAuthenticationOptions>
    {
        public WeChatAuthenticationOptions()
        {
            this.AuthenticationScheme = WeChatAuthenticationDefaults.AuthenticationScheme;
        }
        public WeChatAuthenticationOptions Value
        {
            get
            {
                return this;
            }
        }

        public string CorpId { get; set; }
    }
}
