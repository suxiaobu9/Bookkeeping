using isRock.LineBot;

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
        public Task<string> Accounting(Event lineEvent, EF.User user);
    }
}
