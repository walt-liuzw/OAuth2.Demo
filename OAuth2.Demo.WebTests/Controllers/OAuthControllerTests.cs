using Microsoft.VisualStudio.TestTools.UnitTesting;
using OAuth2.Demo.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuth2.Demo.Web.Controllers.Tests
{
    [TestClass()]
    public class OAuthControllerTests
    {
        private OAuthController _controller = new OAuthController();

        /// <summary>
        /// 授权码模式 获取Code
        /// </summary>
        [TestMethod()]
        public void GetCodeTest()
        {
            string clientId = "1";//客户端Id
            string state = "1";//客户端当前状态
            string redirectUri = "";//表示重定向URI
            string scope = "";//表示申请的权限范围
            var code = _controller.GetCode(clientId, state, redirectUri, scope);
            Assert.Fail();
        }
        /// <summary>
        /// 授权码模式 获取令牌
        /// </summary>
        [TestMethod()]
        public void GetAuthorizationCodeTest()
        {
            //上一步的code
            string code = "";

            string clientId = "1";//客户端Id
            string state = "1";//客户端当前状态
            string redirectUri = "";//表示重定向URI
            string scope = "";//表示申请的权限范围
            var token = _controller.GetAuthorizationCode(code,state,clientId,redirectUri);
            Assert.Fail();
        }

        /// <summary>
        /// 验证令牌
        /// </summary>
        [TestMethod()]
        public void GetValidationAuthorizationTest()
        {
            //上一步的token
            string token = "";

            string clientId = "1";//客户端Id
            var aCode = _controller.GetValidationAuthorization(token, clientId);
            Assert.Fail();
        }

        /// <summary>
        /// 简化模式 获取Token
        /// </summary>
        [TestMethod()]
        public void GetTokenTest()
        {
            string clientId = "1";//客户端Id
            string state = "1";//客户端当前状态
            string redirectUri = "";//表示重定向URI

            var token = _controller.GetToken(clientId, state, redirectUri);
            Assert.Fail();
        }

        /// <summary>
        /// 密码模式 获取Token
        /// </summary>
        [TestMethod()]
        public void GetTokenByUserTest()
        {
            string name = "liuzhiwei";
            string password = "111111";

            var token = _controller.GetTokenByUser(name, password);
            Assert.Fail();
        }

        /// <summary>
        /// 客户端模式 获取Token
        /// </summary>
        [TestMethod()]
        public void GetTokenByClientTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ValidateUserTest()
        {
            var newToken = "fbbae66e4e16a0f7cde29fc95e1cb371";
            var token = "d502dde84762bc7da02613305eda3255";
            string loginName = "liuzhiwei@bs.com";
            var a = _controller.ValidatedCaldavUser(loginName, newToken);
        }

        [TestMethod()]
        public void GenerateTokenTest()
        {
            int tenantId = 100002;
            int userId = 112664957;
            string loginName = "liuzhiwei@bs.com";
            var a = _controller.GenerateToken(tenantId, userId, loginName);
        }
    }
}