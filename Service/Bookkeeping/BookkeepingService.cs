using EF;
using Microsoft.EntityFrameworkCore;
using Service.EventService;
using Service.User;
using System.Text.RegularExpressions;

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
        public async Task<string> Accounting(isRock.LineBot.Event lineEvent, EF.User user)
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

                    if (string.IsNullOrWhiteSpace(messsageSplit[0]))
                        return "請輸入金額 !";

                    // 純數字
                    if (Regex.Match(messsageSplit[0], @"^\d+$").Success)
                    {
                        payEvent = await _eventService.GetEvent(eventName, user.Id);
                        break;
                    }

                    var regexParamList = new List<(string regexParam, string regexGetInt)>
                    {
                        // 數字開頭，帶文字 ex. 1000吃大餐
                        (@"^\d+.+\n*$", @"^\d+"),

                        // 文字開頭，帶數字 ex. 吃大餐1000
                        (@".+\d+$\n*$", @"\d+$\n*"),
                    };

                    foreach (var (regexParam, regexGetInt) in regexParamList)
                    {
                        var regexMatch = Regex.Match(messsageSplit[0], regexGetInt);

                        if (!regexMatch.Success)
                            continue;

                        regexMatch = Regex.Match(messsageSplit[0], regexGetInt);

                        if (!regexMatch.Success)
                            continue;

                        messsageSplit = new string[]
                        {
                            messsageSplit[0].Substring(regexMatch.Index, regexMatch.Length),
                            messsageSplit[0].Remove(regexMatch.Index,regexMatch.Length)
                        };

                        goto case 2;
                    }

                    return "請輸入金額 !";
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
