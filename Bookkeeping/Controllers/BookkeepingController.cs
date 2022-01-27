using isRock.LineBot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Model.AppSettings;
using OfficeOpenXml;
using Service.Bookkeeping;
using Service.User;
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

        [HttpGet]
        [Route("Sync")]
        public async Task<IActionResult> Index1()
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(new FileInfo(@"C:\Users\Bu9\Downloads\2020 - 記帳.xlsx"));

            var aaa = package.Workbook.Worksheets;
            foreach(var item in aaa)
            {

            }

                return Ok();
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
                                x.replyToken != "00000000000000000000000000000000" &&
                                x.type.ToLower() == "message" &&
                                x.message.type == "text")
                    .ToArray();

                var users = await _userService.GetUsers(lineEvents.Select(x => x.source.userId));

                foreach (var item in lineEvents)
                {
                    var user = users.FirstOrDefault(x => x.LineUserId == item.source.userId);
                    if (user == null)
                        continue;

                    var message = await _bookkeepingService.Accounting(item, user);

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
    }
}
