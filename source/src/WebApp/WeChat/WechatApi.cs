namespace MyHomework.WebApp.WeChat
{
    using Microsoft.Extensions.Options;
    using MyHomework.WebApp.Configurations;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public class WeChatApi
    {
        private readonly AppOptions options;
        private HttpClient client;
        private AccessToken accessToken;

        public WeChatApi(IOptions<AppOptions> options)
        {
            this.options = options.Value;
            this.client = new HttpClient();
        }

        #region AccessToken
        public async Task<AccessToken> GetAccessTokenAsync()
        {
            if(this.accessToken == null || this.accessToken.Expires < DateTime.UtcNow)
            {
                this.accessToken = await GetAccessTokenAsyncInternal();
            }

            return this.accessToken;
        }

        private async Task<AccessToken> GetAccessTokenAsyncInternal()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get,
                string.Format("https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}",
                options.WeChatOptions.CorpId, options.WeChatOptions.CorpSecret));

            DateTime now = DateTime.UtcNow;

            var response = await client.SendAsync(request);

            if(!response.IsSuccessStatusCode)
            {
                throw new Exception("Wechat server error in GetAccessToken: " + response.StatusCode);
            }

            var token = JObject.Parse(await response.Content.ReadAsStringAsync());
            
            if(token["access_token"] != null &&
                !string.IsNullOrWhiteSpace(token["access_token"].Value<string>()))
            {
                int expiresIn = token["expires_in"].Value<int>();
                return new AccessToken()
                {
                    Token = token["access_token"].Value<string>(),
                    Expires = now.AddSeconds(expiresIn)
                };
            }

            return null;
        }
        #endregion Accesstoken

        #region UerInfo
        public async Task<string> GetUserIdByCode(string code)
        {
            if(string.IsNullOrWhiteSpace(code))
            {
                throw new Exception("Invalid code in GetUserIdByCode: " + code);
            }
            
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get,
                string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token={0}&code={1}",
                (await this.GetAccessTokenAsync()).Token, code));

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Wechat server error in GetUserIdByCode: " + response.StatusCode);
            }

            var responseString = await response.Content.ReadAsStringAsync();

            var user = JObject.Parse(responseString);

            if (user["UserId"] != null && string.IsNullOrWhiteSpace(user["UserId"].Value<string>()))
            {
                throw new Exception("Wechat server error in GetUserIdByCode:" + responseString);
            }

            return user["UserId"].Value<string>();
        }

        public async Task<UserInfo> GetUserInfoByUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new Exception("Invalid userId in GetUserInfoByUserId: " + userId);
            }

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get,
                string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/get?access_token={0}&userid={1}",
                (await this.GetAccessTokenAsync()).Token, userId));

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Wechat server error in GetUserInfoByUserId: " + response.StatusCode);
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var userInfo = JObject.Parse(responseString);

            if (userInfo["errcode"] == null ||
                userInfo["errcode"].Value<int>() != 0 ||
                userInfo["userid"] == null ||
                string.IsNullOrWhiteSpace(userInfo["userid"].Value<string>()) ||
                userInfo["name"] == null ||
                string.IsNullOrWhiteSpace(userInfo["name"].Value<string>()) ||
                userInfo["status"] == null ||
                string.IsNullOrWhiteSpace(userInfo["status"].Value<string>()))
            {
                throw new Exception("Wechat server error in GetUserInfoByUserId:" + responseString);
            }

            var departmentArray = userInfo["department"].Value<JArray>();
            return new UserInfo()
            {
                UserId = userInfo["userid"].Value<string>(),
                UserName = userInfo["name"].Value<string>(),
                Avatar = userInfo["avatar"] == null ? string.Empty : userInfo["avatar"].Value<string>(),
                Status = (UserStatus)userInfo["status"].Value<int>(),
                DepartmentIds = departmentArray.Select(o => (int)o).ToArray()
            };
        }

        public async Task<UserInfo> GetLoginUserInfoByAuthCode(string authCode)
        {
            if (string.IsNullOrWhiteSpace(authCode))
            {
                throw new Exception("Invalid authCode in GetLoginUserInfoByAuthCode: " + authCode);
            }

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post,
                string.Format("https://qyapi.weixin.qq.com/cgi-bin/service/get_login_info?access_token={0}",
                (await this.GetAccessTokenAsync()).Token));

            request.Content = new StringContent(
                JsonConvert.SerializeObject(new { auth_code = authCode }),
                Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Wechat server error in GetLoginUserInfoByAuthCode: " + response.StatusCode);
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var userInfo = JObject.Parse(responseString);

            if(userInfo["user_info"] == null ||
                userInfo["user_info"].Value<JToken>() == null)
            {
                throw new Exception("Wechat server error in GetLoginUserInfoByAuthCode:" + responseString);
            }

            var userInfoDetail = userInfo["user_info"].Value<JToken>();
            if (userInfoDetail["userid"] == null ||
                string.IsNullOrWhiteSpace(userInfoDetail["userid"].Value<string>()))
            {
                throw new Exception("Wechat server error in GetLoginUserInfoByAuthCode:" + responseString);
            }

            var userId = userInfoDetail["userid"].Value<string>();

            return await this.GetUserInfoByUserId(userId);
        }
        #endregion UserInfo

    }
}
