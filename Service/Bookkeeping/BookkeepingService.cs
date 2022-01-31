using EF;
using Microsoft.EntityFrameworkCore;
using Model.Line;
using Model.StaticData;
using Service.EventService;
using System.Text.RegularExpressions;

namespace Service.Bookkeeping
{
    public class BookkeepingService : IBookkeepingService
    {
        private readonly BookkeepingContext _db;
        private readonly IEventService _eventService;
        public BookkeepingService(BookkeepingContext db,
                                    IEventService eventService)
        {
            _db = db;
            _eventService = eventService;
        }

        /// <summary>
        /// 記帳
        /// </summary>
        /// <param name="lineEvent"></param>
        /// <returns></returns>
        public async Task<(bool isFlex, string message)> Accounting(isRock.LineBot.Event lineEvent, EF.User user)
        {
            var message = lineEvent.message.text;

            if (string.IsNullOrWhiteSpace(message))
                return (false, $"沒有輸入資料");

            var messsageSplit = message.Split(Environment.NewLine.ToCharArray())
                                        .Where(x => !string.IsNullOrWhiteSpace(x))
                                        .ToArray();

            if (messsageSplit == null || messsageSplit.Length == 0)
                return (false, $"沒有輸入資料");

            if (messsageSplit.Length > 2)
                return (false, $"格式錯誤 !{Environment.NewLine}金額{Environment.NewLine}說明");

            var utcNow = DateTime.UtcNow;

            var eventName = "其他";
            int amount = 0;
            EF.Event payEvent;
            switch (messsageSplit.Length)
            {
                case 1:

                    if (string.IsNullOrWhiteSpace(messsageSplit[0]))
                        return (false, "請輸入金額 !");

                    // 正負整數
                    if (Regex.Match(messsageSplit[0], @"^-?\d+$").Success)
                    {
                        payEvent = await _eventService.CreateAndGetEvent(eventName, user.Id);
                        amount = Convert.ToInt32(messsageSplit[0]);
                        break;
                    }

                    var regexParamList = new List<(string regexParam, string regexGetInt)>
                    {
                        // 數字開頭，帶文字 ex. 1000吃大餐
                        (@"^-?\d+.+\n*$", @"^-?\d+"),

                        // 文字開頭，帶數字 ex. 吃大餐1000
                        (@".+\d+$\n*$", @"-?\d+$\n*"),
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

                    return (false, "請輸入金額 !");
                case 2:
                    if (!int.TryParse(messsageSplit[0], out amount))
                    {
                        if (!int.TryParse(messsageSplit[1], out amount))
                            return (false, $"格式錯誤 !{Environment.NewLine}金額{Environment.NewLine}說明");

                        eventName = messsageSplit[0];
                    }
                    else
                    {
                        eventName = messsageSplit[1];
                    }

                    payEvent = await _eventService.CreateAndGetEvent(eventName, user.Id);

                    break;
                default:
                    return (false, "訊息長度異常 !");
            }

            var accounting = new Accounting
            {
                AccountDate = utcNow,
                CreateDate = utcNow,
                Amount = amount,
                UserId = user.Id,
                EventId = payEvent.Id,
            };
            _db.Accountings.Add(accounting);

            await _db.SaveChangesAsync();

            var twNowDate = utcNow.AddHours(8).Date;

            DateTime startDate = new DateTime(twNowDate.Year, twNowDate.Month, 1).AddHours(-8),
                endDate = startDate.AddMonths(1).AddMilliseconds(-1);

            var monthlyAccountings = await _db.Accountings.AsNoTracking()
                .Where(x => startDate <= x.AccountDate &&
                                        x.AccountDate <= endDate &&
                                        x.UserId == user.Id)
                .ToArrayAsync();

            var flexMessageModel = new AccountingFlexMessageModel
            (
                accounting.Id,
                monthlyAccountings.Where(x => x.Amount > 0).Sum(x => x.Amount),
                monthlyAccountings.Where(x => x.Amount < 0).Sum(x => x.Amount) * -1,
                payEvent.Name,
                amount,
                utcNow.AddHours(8),
                false
            );

            return (true, LineFlexTemplate.AccountingFlexMessageTemplate(flexMessageModel));
        }

        /// <summary>
        /// 刪除帳務
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> DeleteAccounting(AccountingFlexMessageModel model, EF.User user)
        {
            var utcNow = DateTime.UtcNow;
            if (utcNow > model.Deadline)
                return "刪除失敗，請重新刪除，並於 2 分鐘內按下確定";

            var accounting = await _db.Accountings.FirstOrDefaultAsync(x => x.UserId == user.Id && x.Id == model.AccountId);

            if (accounting == null)
                return "刪除成功";

            _db.Accountings.Remove(accounting);

            await _db.SaveChangesAsync();

            return "刪除成功";
        }
    }
}
