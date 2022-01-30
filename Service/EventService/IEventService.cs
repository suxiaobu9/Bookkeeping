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
        public Task<EF.Event> CreateAndGetEvent(string eventName, int userId);
    }
}
