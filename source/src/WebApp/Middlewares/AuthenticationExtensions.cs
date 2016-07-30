namespace MyHomework.WebApp.Middlewares
{
    using Configurations;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Options;
    using WeChatAuthenticationMiddlewares;

    public static class AuthenticationExtensions
    {
        public static IApplicationBuilder UseCustomAuthentication(this IApplicationBuilder app, IOptions<AppOptions> appOptions)
        {
            //app.UseCookieAuthentication(new CookieAuthenticationOptions()
            //{
            //    AutomaticChallenge = false
            //});
            
            app.UseWeChatAuthentication(options =>
            {
                options.CorpId = appOptions.Value.WeChatOptions.CorpId;
                options.AuthenticationScheme = WeChatAuthenticationDefaults.AuthenticationScheme;
                options.AutomaticAuthenticate = true;
                options.AutomaticChallenge = true;
            });

            return app;
        }
    }
}
