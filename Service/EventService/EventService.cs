using EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.EventService
{
    public class EventService : IEventService
    {
        private readonly BookkeepingContext _db;

        public EventService(BookkeepingContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 取得花費項目
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<EF.Event> GetEvent(string eventName, int userId)
        {
            eventName = eventName.Trim();
            var payEvent = await _db.Events.FirstOrDefaultAsync(x => x.Name == eventName && x.UserId == userId);

            if (payEvent != null)
                return payEvent;

            payEvent = new Event
            {
                Name = eventName,
                UserId = userId
            };

            await _db.Events.AddAsync(payEvent);

            await _db.SaveChangesAsync();

            return payEvent;
        }

    }
}
