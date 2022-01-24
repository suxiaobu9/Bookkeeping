﻿using EF;
using Microsoft.EntityFrameworkCore;
using Service.EventService;
using Service.User;

namespace Service.Bookkeeping
{
    public class BookkeepingService : IBookkeepingService
    {
        private readonly BookkeepingContext _db;
        private readonly IUserService _userService;
        private readonly IEventService _eventService;
        public BookkeepingService(BookkeepingContext db,
                                    IUserService userService,
                                    IEventService eventService)
        {
            _db = db;
            _userService = userService;
            _eventService = eventService;
        }

        /// <summary>
        /// 記帳
        /// </summary>
        /// <param name="lineEvent"></param>
        /// <returns></returns>
        public async Task<string> Accounting(isRock.LineBot.Event lineEvent,EF.User user)
        {
            var message = lineEvent.message.text;

            if (string.IsNullOrWhiteSpace(message))
                return $"沒有輸入資料";

            var messsageSplit = message.Split(Environment.NewLine.ToCharArray())
                                        .Where(x => !string.IsNullOrWhiteSpace(x))
                                        .ToArray();

            if (messsageSplit == null || messsageSplit.Length == 0)
                return $"沒有輸入資料";

            if (messsageSplit.Length > 2)
                return $"格式錯誤 !{Environment.NewLine}金額{Environment.NewLine}說明";

            var now = DateTime.UtcNow;

            var eventName = "其他";
            int amount = 0;
            EF.Event payEvent;
            switch (messsageSplit.Length)
            {
                case 1:
                    if (string.IsNullOrWhiteSpace(messsageSplit[0]) || !int.TryParse(messsageSplit[0], out amount))
                        return "請輸入金額 !";

                    payEvent = await _eventService.GetEvent(eventName, user.Id);

                    break;
                case 2:
                    if (!int.TryParse(messsageSplit[0], out amount))
                    {
                        if (!int.TryParse(messsageSplit[1], out amount))
                            return $"格式錯誤 !{Environment.NewLine}金額{Environment.NewLine}說明";

                        eventName = messsageSplit[0];
                    }
                    else
                    {
                        eventName = messsageSplit[1];
                    }

                    payEvent = await _eventService.GetEvent(eventName, user.Id);

                    break;
                default:
                    return "訊息長度異常 !";
            }

            _db.Accountings.Add(new Accounting
            {
                AccountDate = now,
                CreateDate = now,
                Amount = amount,
                UserId = user.Id,
                EventId = payEvent.Id,
            });

            await _db.SaveChangesAsync();

            return $"金額 : {amount}{Environment.NewLine}用途 : {eventName}";

        }
    }
}
