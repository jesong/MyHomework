namespace MyHomework.WebApp.Middlewares.WeChatAuthenticationMiddlewares
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using System.Collections.Generic;

    public class WeChatAuthenticationOptions : RemoteAuthenticationOptions
    {
        public WeChatAuthenticationOptions()
            : this(WeChatAuthenticationDefaults.AuthenticationScheme)
        {
        }

        public WeChatAuthenticationOptions(string authenticationScheme)
        {
            AuthenticationScheme = authenticationScheme;
            AutomaticChallenge = true;
            DisplayName = WeChatAuthenticationDefaults.Caption;
            CallbackPath = new PathString("/signin-wechat");
            //Events = new OpenIdConnectEvents();
            Scope.Add("snsapi_base");
        }

        /// <summary>
        /// Gets the list of permissions to request.
        /// </summary>
        public ICollection<string> Scope { get; } = new HashSet<string>();

        public string CorpId { get; set; }
    }
}
