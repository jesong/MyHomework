namespace MyHomework.WebApp.Middlewares
{
    using Configurations;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using System.Threading.Tasks;
    using WeChatAuthenticationMiddlewares;

    public static class AuthenticationExtensions
    {
        public static IApplicationBuilder UseCustomAuthentication(this IApplicationBuilder app, IOptions<AppOptions> appOptions)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                LoginPath = new PathString("/account/welcome"),
                LogoutPath = new PathString("/account/logout"),
                AccessDeniedPath = new PathString("/account/register"),
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
                options.Events = new RemoteAuthenticationEvents
                {
                    OnRemoteFailure = OnWeChatAuthenticationFailed
                };
            });

            return app;
        }

        private static Task OnWeChatAuthenticationFailed(FailureContext context)
        {
            context.HandleResponse();
            context.Response.Redirect("/account/register");
            return Task.FromResult(0);
        }
    }
}
