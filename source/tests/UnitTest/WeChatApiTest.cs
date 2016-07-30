using Microsoft.Extensions.Options;
using Moq;
using MyHomework.WebApp.Configurations;
using MyHomework.WebApp.WeChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest
{
    public class WeChatApiTest
    {
        WeChatApi api;
        public WeChatApiTest()
        {
            var appOptionsMock = new Mock<IOptions<AppOptions>>();

            var options = new AppOptions()
            {
                WeChatOptions = new WeChatOptions()
                {
                    CorpId = "wxd17fdbdaf75e0342",
                    CorpSecret = "pDHVlKS09yz8m6S96ndHHkFwtT2IT_rHbkqiuvbAJNZ_Qvh7pRQNKimTrf_OU05S"
                }
            };

            appOptionsMock.Setup(x => x.Value).Returns(options);
            api = new WeChatApi(appOptionsMock.Object);
        }

        [Fact]
        public async Task Test_GetAccessTokenAsync()
        {
            var accessToken = await api.GetAccessTokenAsync();

            Assert.NotNull(accessToken);
        }

        [Fact]
        public async Task Test_GetUserInfoByUserId()
        {
            var userInfo = await api.GetUserInfoByUserId("09");

            Assert.NotNull(userInfo);
        }
    }
}
