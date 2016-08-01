
namespace MyHomework.WebApp.WeChat
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    public static class WeChatUtilities
    {
        public static bool IsWeChatInternalBrowser(HttpRequest request)
        {
            StringValues authHeader;

            if (request.Headers.TryGetValue("User-Agent", out authHeader) &&
                authHeader.Any())
            {
                return authHeader[0].Contains("MicroMessenger");
            }

            return false;
        }
    }
}
