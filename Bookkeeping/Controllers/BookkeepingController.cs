using isRock.LineBot;
using Microsoft.AspNetCore.Mvc;
using Utility.LineVerify;

namespace Bookkeeping.Controllers
{
    /// <summary>
    /// ngrok http 5000 -host-header="localhost:5000"
    /// </summary>
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BookkeepingController : LineWebHookControllerBase
    {
        [HttpPost]
        [LineVerifySignature]
        [ApiExplorerSettings(IgnoreApi = false)]
        public IActionResult Index()
        {
            if(this.ReceivedMessage == null)
            {
                //this.PushMessage(_lineBot.AdminUserId, "ReceivedMessage = null");
                return Ok();

            }

            return Ok();
        }
    }
}
