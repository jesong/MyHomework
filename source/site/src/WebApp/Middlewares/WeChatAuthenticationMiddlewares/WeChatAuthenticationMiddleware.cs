namespace MyHomework.WebApp.Middlewares.WeChatAuthenticationMiddlewares
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MyHomework.WebApp.WeChat;
    using System.Text.Encodings.Web;

    public class WeChatAuthenticationMiddleware : AuthenticationMiddleware<WeChatAuthenticationOptions>
    {
        private readonly WeChatApi weChatApi;
        public WeChatAuthenticationMiddleware(RequestDelegate next,
            IOptions<WeChatAuthenticationOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            WeChatApi weChatApi) 
            : base(next, options, loggerFactory, encoder)
        {
            this.weChatApi = weChatApi;
        }

        protected override AuthenticationHandler<WeChatAuthenticationOptions> CreateHandler()
        {
            return new WeChatAuthenticationHandler(weChatApi);
        }
    }
}
