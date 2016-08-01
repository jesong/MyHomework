namespace MyHomework.WebApp.Middlewares.WeChatAuthenticationMiddlewares
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Options;
    using System;

    public static class WeChatAuthenticationAppBuilderExtensions
    {
        public static IApplicationBuilder UseWeChatAuthentication(this IApplicationBuilder app, Action<WeChatAuthenticationOptions> configureOptions)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            var options = new WeChatAuthenticationOptions();
            if (configureOptions != null)
            {
                configureOptions(options);
            }

            return app.UseMiddleware<WeChatAuthenticationMiddleware>(Options.Create(options));
        }
    }
}
