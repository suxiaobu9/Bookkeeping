using Bookkeeping.Utility;
using isRock.LineBot;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Model.Appsetting;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookkeeping.Controllers
{
    /// <summary>
    /// ngrok http 5000 -host-header="localhost:5000"
    /// </summary>
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BookkeepingController : LineWebHookControllerBase
    {
        public BookkeepingController(IOptions<LineBot> lintBot,
            ILineBotMessageService lineBotMessageService)
        {
            _lineBot = lintBot.Value;
            this.ChannelAccessToken = _lineBot.ChannelAccessToken;
            _lineBotMessageService = lineBotMessageService;
        }

        private readonly LineBot _lineBot;
        private readonly ILineBotMessageService _lineBotMessageService;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [LineVerifySignature]
        [ApiExplorerSettings(IgnoreApi = false)]

        public IActionResult Post()
        {
            if (this.ReceivedMessage == null)
            {
                this.PushMessage(_lineBot.AdminUserId, "ReceivedMessage = null");
                return Ok();
            }

            foreach (var lineEvent in this.ReceivedMessage.events)
            {
                try
                {
                    if ((lineEvent == null || lineEvent.replyToken == "00000000000000000000000000000000") ||
                        (lineEvent.type.ToLower() != "message" || lineEvent.message.type != "text") ||
                        lineEvent.source.userId != _lineBot.Bo &&
                        lineEvent.source.userId != _lineBot.Chien)
                        continue;

                    _lineBotMessageService.Process(lineEvent);

                    ReplySuccess(lineEvent.replyToken);
                }
                catch (Exception ex)
                {
                    var msg = "發生錯誤:\n" + ex.Message;
                    this.ReplyMessage(lineEvent.replyToken, msg);
                    this.PushMessage(_lineBot.AdminUserId, msg);
                }
            }

            return Ok();
        }

        private void ReplySuccess(string replyToken)
        {
            this.ReplyMessage(replyToken, "輸入成功！");
        }
    }
}
