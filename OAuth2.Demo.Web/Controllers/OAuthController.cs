using OAuth2.Demo.Common;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using OAuth2.Demo.Common.Common;
using OAuth2.Demo.Common.Proxy;

namespace OAuth2.Demo.Web.Controllers
{
    [RoutePrefix("api/oauth2")]
    public class OAuthController : ApiController
    {
        #region 配置
        //code有效时间/分钟
        int codeValidTime = 10;
        //token有效时间/分钟
        int accesstokenValidTime = 60;

        private int tenantId = 100000;

        dynamic returnData = new ExpandoObject();
        public OAuthController()
        {
            returnData.Massage = "";
            returnData.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            returnData.Note = "";
            returnData.Status = 0;
        }
        #endregion

        #region 授权码模式
        /// <summary>
        /// 1、【授权码模式】获取Code【有效时间：10min，获取令牌必备使用1次后失效】
        /// </summary>
        /// <param name="clientId">表示客户端的ID，必选项</param>
        /// <param name="redirectUri">表示重定向URI，可选项</param>
        /// <param name="scope">表示申请的权限范围，可选项</param>
        /// <param name="state">表示客户端的当前状态，可以指定任意值，认证服务器会原封不动地返回这个值</param>
        /// <returns>Code</returns>
        [Route("GetCode")]
        public dynamic GetCode(string clientId, string state, string redirectUri = "", string scope = "")
        {
            string code = Guid.NewGuid().ToString("N");
            returnData.Massage = code;
            returnData.Note = state;
            RedisCacheHelper.Add(tenantId,"GetCode" + clientId, code, DateTime.Now.AddMinutes(codeValidTime));
            if (!string.IsNullOrEmpty(redirectUri))
            {
                HttpContext.Current.Response.Redirect(redirectUri + "?code=" + code + "&state=" + state, true);
            }
            return returnData;
        }

        /// <summary>
        /// 2、【授权码模式】获取令牌【有效时间：1h，获取资源接口可重复使用】
        /// </summary>
        /// <param name="code">表示上一步获得的授权码，必选项</param>
        /// <param name="state">表示客户端的当前状态，可以指定任意值，认证服务器会原封不动地返回这个值</param>
        /// <param name="clientId">表示客户端ID，必选项</param>
        /// <param name="redirectUri">表示重定向URI，必选项，且必须与A步骤中的该参数值保持一致</param>
        /// <returns>AuthorizationCode</returns>
        [Route("GetAuthorizationCode")]
        public dynamic GetAuthorizationCode(string code, string state, string clientId, string redirectUri = "")
        {
            string redisCode = RedisCacheHelper.Get<string>(tenantId,"GetCode" + clientId) ?? "";
            returnData.Note = state;
            if (string.IsNullOrEmpty(redisCode))
            {
                returnData.Massage = "code过期";
                return returnData;
            }
            if (!redisCode.Equals(code))
            {
                returnData.Massage = "code有误";
                return returnData;
            }
            string accesstoken = Guid.NewGuid().ToString("N");
            returnData.Massage = accesstoken;
            RedisCacheHelper.Add(tenantId,"GetAuthorizationCode" + clientId, accesstoken, DateTime.Now.AddMinutes(accesstokenValidTime));

            RedisCacheHelper.Remove(tenantId, "GetCode" + clientId);
            if (!string.IsNullOrEmpty(redirectUri))
            {
                HttpContext.Current.Response.Redirect(redirectUri + "?code=" + code + "&state=" + state, true);
            }
            return returnData;
        }

        /// <summary>
        /// 3、验证令牌
        /// </summary>
        /// <param name="accessToken">令牌</param>
        /// <param name="clientId">表示客户端ID，必选项</param>
        /// <returns>Bool AuthorizationCode</returns>
        [Route("GetValidationAuthorization")]
        public dynamic GetValidationAuthorization(string accessToken, string clientId)
        {
            string redisAccesstoken = RedisCacheHelper.Get<string>(tenantId, "GetAuthorizationCode" + clientId) ?? "";
            if (accessToken.Equals(redisAccesstoken))
            {
                returnData.Massage = true;
                return returnData;
            }
            else
            {
                returnData.Massage = false;
                return returnData;
            }
        }
        #endregion

        #region 简化模式
        /// <summary>
        /// 4、【简化模式】获取Token【有效时间：1h，获取资源接口可重复使用】
        /// </summary>
        /// <param name="clientId">表示客户端的ID，必选项</param>
        /// <param name="redirectUri">表示重定向URI，可选项</param>
        /// <param name="state">表示客户端的当前状态，可以指定任意值，认证服务器会原封不动地返回这个值</param>
        /// <returns>Code</returns>
        [Route("GetToken")]
        public dynamic GetToken(string clientId, string state, string redirectUri = "")
        {
            string token = Guid.NewGuid().ToString("N");
            returnData.Massage = token;
            returnData.Note = state;
            RedisCacheHelper.Add(tenantId, "GetAuthorizationCode" + clientId, token, DateTime.Now.AddMinutes(accesstokenValidTime));
            if (!string.IsNullOrEmpty(redirectUri))
            {
                HttpContext.Current.Response.Redirect(redirectUri + "?token=" + token + "&state=" + state, true);
            }
            return returnData;
        }

        #endregion

        #region 密码模式
        /// <summary>
        /// 5、【密码模式】获取Token【有效时间：1h，获取资源接口可重复使用】
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="passWord">密码</param>
        /// <returns></returns>
        [Route("GetTokenByUser")]
        public dynamic GetTokenByUser(string userName, string passWord)
        {
            int clientId = 0;
            if (userName == passWord)
                clientId = 1;
            else
            {
                returnData.Massage = "用户名密码有误(Test相同即可)";
                return returnData;
            }
            string token = Guid.NewGuid().ToString("N");
            returnData.Massage = token;
            RedisCacheHelper.Add(tenantId, "GetAuthorizationCode" + clientId, token, DateTime.Now.AddMinutes(accesstokenValidTime));
            return returnData;
        }
        #endregion

        #region 客户端模式
        /// <summary>
        /// 6、【客户端模式】获取Token【有效时间：1h，获取资源接口可重复使用】
        /// </summary>
        /// <returns></returns>
        [Route("GetTokenByClient")]
        public dynamic GetTokenByClient()
        {
            int clientId = 1;
            string token = Guid.NewGuid().ToString("N");
            returnData.Massage = token;
            RedisCacheHelper.Add(tenantId, "GetAuthorizationCode" + clientId, token, DateTime.Now.AddMinutes(accesstokenValidTime));
            return returnData;
        }
        #endregion


        public dynamic ValidatedCaldavUser(string loginName, string token)
        {
            //判断这个人是否在北森数据库
            var user = AccountProxy.GetAssociatedUsersByLoginName(loginName);
            if (user == null)
            {
                return null;
            }

            
            var userGuid = RedisCacheHelper.Get<string>(user.TenantId, MakeUserGuidKey(loginName));
            if (userGuid == null)
            {
                return null;
            }

            if (SecurityHelper.GetMd5($"{user.TenantId}_{user.UserId}_Schedule_{userGuid}") == token)
            {
                //校验通过，去获取数据
                return "获取到北森的数据了！！！";
            }

            return null;
        }

        /// <summary>
        /// 生成Token 保存在redis里
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        public string GenerateToken(int tenantId, int userId, string loginName)
        {
            var guid = Guid.NewGuid().ToString("N");
            var value = SecurityHelper.GetMd5($"{tenantId}_{userId}_Schedule_{guid}");

            RedisCacheHelper.PipelineSet(tenantId, MakeUserGuidKey(loginName), guid,
                () => { RedisCacheHelper.PipelineSet(tenantId, MakeUserTokenKey(loginName), value); });

            return value;
        }
        private string MakeUserGuidKey(string loginName)
        {
            return $"{loginName}:guid";
        }
        private string MakeUserTokenKey(string loginName)
        {
            return $"{loginName}:token";
        }

    }
}
