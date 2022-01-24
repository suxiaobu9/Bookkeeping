using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.EventService
{
    public interface IEventService
    {
        /// <summary>
        /// 取得花費項目
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<EF.Event> GetEvent(string eventName, int userId);
    }
}
