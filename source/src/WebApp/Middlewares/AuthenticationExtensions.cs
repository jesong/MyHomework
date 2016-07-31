namespace MyHomework.WebApp.Middlewares
{
    using Configurations;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using WeChatAuthenticationMiddlewares;

    public static class AuthenticationExtensions
    {
        public static IApplicationBuilder UseCustomAuthentication(this IApplicationBuilder app, IOptions<AppOptions> appOptions)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                LoginPath = new PathString("/account/login"),
                ReturnUrlParameter = "returnUrl",
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });

            app.UseWeChatAuthentication(options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.CorpId = appOptions.Value.WeChatOptions.CorpId;
                options.AuthenticationScheme = WeChatAuthenticationDefaults.AuthenticationScheme;
                options.AutomaticAuthenticate = false;
                options.AutomaticChallenge = false;
            });

            return app;
        }
    }
}
