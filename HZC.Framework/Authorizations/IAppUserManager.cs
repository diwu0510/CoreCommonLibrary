using System;
using System.Collections.Generic;
using System.Text;

namespace HZC.Framework.Authorizations
{
    public interface IApplicationUserManager<TUser, TPrimaryKey> where TUser : IAppUser<TPrimaryKey>
    {
        /// <summary>
        /// 用户名密码获取用户
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pw"></param>
        /// <returns></returns>
        TUser GetUserByAccountAndPassword(string account, string pw);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="originPw">原始密码</param>
        /// <param name="newPw">新密码</param>
        /// <returns></returns>
        Result ChangePw(TPrimaryKey id, string originPw, string newPw);
    }
}
