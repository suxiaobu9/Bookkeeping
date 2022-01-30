using isRock.LineBot;
using Model.Line;

namespace Service.Bookkeeping
{
    public interface IBookkeepingService
    {
        /// <summary>
        /// 記帳
        /// </summary>
        /// <param name="lineEvent"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<(bool isFlex, string message)> Accounting(Event lineEvent, EF.User user);

        /// <summary>
        /// 刪除帳務
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> DeleteAccounting(AccountingFlexMessageModel model, EF.User user);

    }
}
