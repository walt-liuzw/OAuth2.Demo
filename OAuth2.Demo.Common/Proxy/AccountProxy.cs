using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beisen.UserFramework.Models;
using Beisen.UserFramework.Service;

namespace OAuth2.Demo.Common.Proxy
{
    public static class AccountProxy
    {
        public static UserInfoModel GetAssociatedUsersByLoginName(string loginName)
        {
            var args = new AssociatedUsersGetByLoginNameArgs
            {
                LoginName = loginName
            };
            var options = AssociatedUsersGetByLoginNameOptions.Default;
            var result = AccountService.Instance.GetAssociatedUsersByLoginName(args, options);
            if (result.Ok)
            {
                var user = result.State[0];
                return new UserInfoModel()
                {
                    TenantId = user.TenantId,
                    UserId = user.UserId
                };
            }
            return null;
        }
    }

    public class UserInfoModel
    {
        public int TenantId { get; set; }
        public int UserId { get; set; }
    }
}
