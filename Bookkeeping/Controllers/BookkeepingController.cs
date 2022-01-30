using isRock.LineBot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Model.AppSettings;
using Service.Bookkeeping;
using Service.User;
using Utility.LineVerify;
using System.Text.Json;
using System.Text;
using Model.Line;
using Model.StaticData;

namespace Bookkeeping.Controllers
{
    /// <summary>
    /// ngrok http 5144 -host-header="localhost:5144"
    /// </summary>
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BookkeepingController : LineWebHookControllerBase
    {
        private readonly IUserService _userService;
        private readonly IBookkeepingService _bookkeepingService;
        public BookkeepingController(IUserService userService,
                                    IBookkeepingService bookkeepingService,
                                    IOptions<LineBot> linebot)
        {
            _userService = userService;
            _bookkeepingService = bookkeepingService;
            this.ChannelAccessToken = linebot.Value.ChannelAccessToken;
        }


        [HttpPost]
        [Route("Accounting")]
        [LineVerifySignature]
        [ApiExplorerSettings(IgnoreApi = false)]
        public async Task<IActionResult> Index()
        {
            if (this.ReceivedMessage == null)
            {
                var admin = await _userService.GetAdmin();

                if (admin != null)
                    this.PushMessage(admin.LineUserId, "ReceivedMessage = null");
                return Ok();
            }

            try
            {
                var lineEvents = this.ReceivedMessage.events
                    .Where(x => x != null &&
                                (x.type.ToLower() == "postback" || (
                                x.type.ToLower() == "message" &&
                                x.message.type == "text")))
                    .ToArray();

                var users = await _userService.GetUsers(lineEvents.Select(x => x.source.userId));

                foreach (var item in lineEvents)
                {
                    var user = users.FirstOrDefault(x => x.LineUserId == item.source.userId);
                    if (user == null)
                        continue;

                    if (item.type.ToLower() == "postback")
                    {
                        await PostBack(item, user);
                        continue;
                    }

                    var (isFlex, message) = await _bookkeepingService.Accounting(item, user);

                    if (isFlex)
                        this.ReplyMessageWithJSON(item.replyToken, message);
                    else
                        this.ReplyMessage(item.replyToken, message);
                }
            }
            catch (Exception ex)
            {
                var admin = await _userService.GetAdmin();

                if (admin != null)
                    this.PushMessage(admin.LineUserId, ex.Message);
            }

            return Ok();
        }

        /// <summary>
        /// 刪除的 Flex Message PostBack
        /// </summary>
        /// <param name="lineEvent"></param>
        /// <returns></returns>
        private async Task PostBack(Event lineEvent, EF.User user)
        {
            var jsonData = Encoding.UTF8.GetString(Convert.FromBase64String(lineEvent.postback.data));
            var model = JsonSerializer.Deserialize<AccountingFlexMessageModel>(jsonData);

            if (model == null)
                return;

            if (!model.IsConfirm)
            {
                model.IsConfirm = true;
                model.Deadline = DateTime.UtcNow.AddMinutes(2);
                ReplyMessageWithJSON(lineEvent.replyToken, LineFlexTemplate.DeleteAccountingComfirm(model));
                return;
            }

            var message = await _bookkeepingService.DeleteAccounting(model, user);

            ReplyMessage(lineEvent.replyToken, message);

        }
    }
}
