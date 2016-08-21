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

        public async Task<UserInfo> GetUserInfoByUserId(string userId, UserInfo userInfo = null)
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
            var jUserInfo = JObject.Parse(responseString);

            if (jUserInfo["errcode"] == null ||
                jUserInfo["errcode"].Value<int>() != 0 ||
                jUserInfo["userid"] == null ||
                string.IsNullOrWhiteSpace(jUserInfo["userid"].Value<string>()) ||
                jUserInfo["name"] == null ||
                string.IsNullOrWhiteSpace(jUserInfo["name"].Value<string>()) ||
                jUserInfo["status"] == null ||
                string.IsNullOrWhiteSpace(jUserInfo["status"].Value<string>()))
            {
                throw new Exception("Wechat server error in GetUserInfoByUserId:" + responseString);
            }

            var departmentArray = jUserInfo["department"].Value<JArray>();
            if (userInfo == null)
            {
                userInfo = new UserInfo();
            }

            userInfo.UserId = jUserInfo["userid"].Value<string>();
            userInfo.UserName = jUserInfo["name"].Value<string>();
            userInfo.Avatar = jUserInfo["avatar"] == null ? string.Empty : jUserInfo["avatar"].Value<string>();
            userInfo.Status = (UserStatus)jUserInfo["status"].Value<int>();
            userInfo.DepartmentIds = departmentArray.Select(o => (int)o).ToArray();

            return userInfo;
        }

        public async Task<LoginUserInfo> GetLoginUserInfoByAuthCode(string authCode)
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
            var jUserInfo = JObject.Parse(responseString);

            if(jUserInfo["usertype"] == null ||
                string.IsNullOrEmpty(jUserInfo["usertype"].Value<string>()))
            {
                throw new Exception("Wechat server error in GetLoginUserInfoByAuthCode:" + responseString);
            }

            var userType = (UserType)(jUserInfo["usertype"].Value<int>());

            if (jUserInfo["user_info"] == null ||
                jUserInfo["user_info"].Value<JToken>() == null)
            {
                throw new Exception("Wechat server error in GetLoginUserInfoByAuthCode:" + responseString);
            }

            var userInfoDetail = jUserInfo["user_info"].Value<JToken>();
            if (userInfoDetail["userid"] == null ||
                string.IsNullOrWhiteSpace(userInfoDetail["userid"].Value<string>()))
            {
                throw new Exception("Wechat server error in GetLoginUserInfoByAuthCode:" + responseString);
            }

            var userId = userInfoDetail["userid"].Value<string>();

            return (LoginUserInfo)(await this.GetUserInfoByUserId(userId, new LoginUserInfo() {
                UserType = userType
            }));
        }
        #endregion UserInfo

        #region Message
        public async Task SendMessage(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new Exception("Invalid text in SendMessage: " + text);
            }

            var msg = new
            {
                touser = "@all",
                msgtype = "text",
                agentid = this.options.WeChatOptions.HomeworkPublishTargetAgentId,
                text = new
                {
                    content = text
                }
            };

            await this.SendMessageInternal(msg);
        }

        public async Task SendMessage(string title, string description, Uri url, Uri picUrl)
        {
            if (string.IsNullOrWhiteSpace(title) &&
                string.IsNullOrWhiteSpace(description) &&
                url == null &&
                picUrl == null)
            {
                throw new Exception(string.Format("Invalid argument in SendMessage: {0}, {1}, {2}, {3}",
                    title, description, url, picUrl));
            }
            
            var msg = new
            {
                touser = "@all",
                msgtype = "news",
                agentid = this.options.WeChatOptions.HomeworkPublishTargetAgentId,
                news = new
                {
                    articles = new dynamic[] {
                        new {
                            title = title,
                            description = description,
                            url = url == null ? null : url.AbsoluteUri,
                            picurl = picUrl == null ? null : picUrl.AbsoluteUri
                        }
                    }
                }
            };

            await this.SendMessageInternal(msg);
        }

        private async Task SendMessageInternal(object msg)
        {
            if (msg == null)
            {
                throw new Exception("Invalid msg in SendMessageInternal.");
            }

            var uri = string.Format("https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={0}",
                (await this.GetAccessTokenAsync()).Token);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);

            request.Content = new StringContent(
                JsonConvert.SerializeObject(msg),
                Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Wechat server error in SendMessage: " + response.StatusCode);
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var jMessageReply = JObject.Parse(responseString);

            var errorCode = jMessageReply["errcode"].Value<int>();
            if (errorCode != 0)
            {
                throw new Exception("Wechat server error in SendMessage: " + errorCode);
            }
        }
        #endregion
    }
}
