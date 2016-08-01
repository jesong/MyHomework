namespace MyHomework.WebApp.Middlewares
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using Configurations;
    using Basic;

    public class HttpsRedirectionMiddleware
    {
        private readonly RequestDelegate next;
        private bool enableHttpsRedirection;

        public HttpsRedirectionMiddleware(RequestDelegate next, IOptions<AppOptions> options)
        {
            this.next = next;
            this.enableHttpsRedirection = options.Value.EnableHttpsRedirection;
        }

        public async Task Invoke(HttpContext context)
        {
            if (this.enableHttpsRedirection)
            {
                if (!context.Request.IsHttps)
                {
                    var uri = new Uri(UriHelper.GetDisplayUrl(context.Request));
                    var uriBuilder = new UriBuilder(uri)
                    {
                        Scheme = "https",
                        Port = -1, // use the default port
                    };
                    context.Response.Redirect(uriBuilder.Uri.ToString(), permanent: true);
                    return;
                }
                else
                {
                    context.Response.Headers[Globals.ResponseHeaderStrictTransportSecurityName] =
                        Globals.ResponseHeaderStrictTransportSecurityValue;
                }
            }

            await this.next.Invoke(context);
        }
    }
}