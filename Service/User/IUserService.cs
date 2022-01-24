using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.User
{
    public interface IUserService
    {
        /// <summary>
        /// 取得管理者
        /// </summary>
        /// <returns></returns>
        public Task<EF.User?> GetAdmin();

        /// <summary>
        /// 取得使用者
        /// </summary>
        /// <param name="lineUserId"></param>
        /// <returns></returns>
        public Task<EF.User?> GetUser(string lineUserId);

        /// <summary>
        /// 取得使用者
        /// </summary>
        /// <param name="lineUserIds"></param>
        /// <returns></returns>
        public Task<EF.User[]> GetUsers(IEnumerable<string> lineUserIds);
    }
}
