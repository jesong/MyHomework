namespace MyHomework.WebApp.Middlewares.WeChatAuthenticationMiddlewares
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http.Authentication;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Http.Features.Authentication;
    using Microsoft.Extensions.Primitives;
    using MyHomework.WebApp.WeChat;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class WeChatAuthenticationHandler : AuthenticationHandler<WeChatAuthenticationOptions>
    {
        private static readonly string WeChatInternalAuthEndPoint = "https://open.weixin.qq.com/connect/oauth2/authorize";
        private static readonly string WeChatMemberAuthEndPoint = "https://qy.weixin.qq.com/cgi-bin/loginpage";

        private readonly WeChatApi weChatApi;

        public WeChatAuthenticationHandler(WeChatApi weChatApi)
        {
            this.weChatApi = weChatApi;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                UserInfo userInfo;
                if (WeChatUtilities.IsWeChatInternalBrowser(Request))
                {
                    StringValues code;
                    if (Request.Query.TryGetValue("code", out code))
                    {
                        var userId = await this.weChatApi.GetUserIdByCode(code);
                        userInfo = await this.weChatApi.GetUserInfoByUserId(userId);
                    }
                    else
                    {
                        return AuthenticateResult.Fail("invalid code");
                    }
                }
                else
                {
                    StringValues authCode;
                    if (Request.Query.TryGetValue("auth_code", out authCode))
                    {
                        userInfo = await this.weChatApi.GetLoginUserInfoByAuthCode(authCode);
                    }
                    else
                    {
                        return AuthenticateResult.Fail("invalid code");
                    }
                }

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(WeChatAuthenticationDefaults.AuthenticationType);
                claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userInfo.UserId, ClaimValueTypes.String, WeChatAuthenticationDefaults.ClaimIssuer));

                claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, userInfo.UserName, ClaimValueTypes.String, WeChatAuthenticationDefaults.ClaimIssuer));

                claimsIdentity.AddClaim(new Claim(WeChatAuthenticationDefaults.ClaimType_Avatar, userInfo.Avatar, ClaimValueTypes.String, WeChatAuthenticationDefaults.ClaimIssuer));

                string departments = string.Empty;
                foreach(var department in userInfo.DepartmentIds)
                {
                    departments += string.Format("{0},", department);
                }
                departments.TrimEnd(',');
                claimsIdentity.AddClaim(new Claim(WeChatAuthenticationDefaults.ClaimType_Department,
                    departments, ClaimValueTypes.String, WeChatAuthenticationDefaults.ClaimIssuer));

                claimsIdentity.AddClaim(new Claim(WeChatAuthenticationDefaults.ClaimType_Status,
                    userInfo.Status.ToString(), ClaimValueTypes.String, WeChatAuthenticationDefaults.ClaimIssuer));

                var authenticationTicket = new AuthenticationTicket(
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties(),
                    this.Options.AuthenticationScheme);

                return AuthenticateResult.Success(authenticationTicket);
            }
            catch(Exception e)
            {
                return AuthenticateResult.Fail(e);
            }
        }

        protected override Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var properties = new AuthenticationProperties(context.Properties);
            if (string.IsNullOrEmpty(properties.RedirectUri))
            {
                properties.RedirectUri = CurrentUri;
            }

            var redirectUri = BuildRedirectUri(properties.RedirectUri);

            var authorizationEndpoint = BuildChallengeUrl(redirectUri);
            Response.Redirect(authorizationEndpoint);
            return Task.FromResult(true);
        }



        private string BuildChallengeUrl(string redirectUrl)
        {
            if (WeChatUtilities.IsWeChatInternalBrowser(Request))
            {
                var queryBuilder = new QueryBuilder()
                {
                    { "appId", Options.CorpId},
                    { "redirect_uri", redirectUrl},
                    { "response_type", "code" },
                    { "scope", "snsapi_base" },
                 };

                return WeChatInternalAuthEndPoint + queryBuilder.ToString() + "#wechat_redirect";
            }
            else
            {
                var queryBuilder = new QueryBuilder()
                {
                    { "corp_id", Options.CorpId},
                    { "redirect_uri", redirectUrl},
                 };

                return WeChatMemberAuthEndPoint + queryBuilder.ToString();

            }
        }

    }
}
