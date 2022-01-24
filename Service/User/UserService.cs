using EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.User
{
    public class UserService : IUserService
    {
        private readonly BookkeepingContext _db;
        public UserService(BookkeepingContext db)
        {
            _db = db;
        }
        /// <summary>
        /// 取得管理者
        /// </summary>
        /// <returns></returns>
        public async Task<EF.User?> GetAdmin()
        {
            return await _db.Users.FirstOrDefaultAsync(x => x.IsAdmin);
        }

        /// <summary>
        /// 取得使用者
        /// </summary>
        /// <param name="lineUserId"></param>
        /// <returns></returns>
        public async Task<EF.User?> GetUser(string lineUserId)
        {
            return await _db.Users.FirstOrDefaultAsync(x => x.LineUserId == lineUserId);
        }

        /// <summary>
        /// 取得使用者
        /// </summary>
        /// <param name="lineUserIds"></param>
        /// <returns></returns>
        public async Task<EF.User[]> GetUsers(IEnumerable<string> lineUserIds)
        {
            return await _db.Users.Where(x => lineUserIds.Contains(x.LineUserId)).ToArrayAsync();
        }
    }
}
