namespace MyHomework.WebApp.Middlewares.WeChatAuthenticationMiddlewares
{
    public class WeChatAuthenticationDefaults
    {
        public static readonly string AuthenticationType = "WeChatAuthentication";
        public static readonly string AuthenticationScheme = "WeChat";
        public static readonly string ClaimIssuer = "WeChat";

        public static readonly string ClaimType_Department = "Department";
        public static readonly string ClaimType_Avatar = "Avatar";
        public static readonly string ClaimType_Status = "Status";

        public static readonly string Caption = "WeChat";

        public static readonly string WeChatInternalAuthEndPoint = "https://open.weixin.qq.com/connect/oauth2/authorize";
        public static readonly string WeChatMemberAuthEndPoint = "https://qy.weixin.qq.com/cgi-bin/loginpage";
    }
}
