using EF;
using isRock.LineBot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Model.AppSettings;
using OfficeOpenXml;
using Service.Bookkeeping;
using Service.EventService;
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
        private readonly IEventService _eventService;
        private readonly BookkeepingContext _db;
        private readonly ILogger<BookkeepingController> _logger;
        public BookkeepingController(IUserService userService,
                                    IBookkeepingService bookkeepingService,
                                    IEventService eventService,
                                    IOptions<LineBot> linebot,
                                    ILogger<BookkeepingController> logger,
                                    BookkeepingContext db)
        {
            _userService = userService;
            _bookkeepingService = bookkeepingService;
            _eventService = eventService;
            _db = db;
            this.ChannelAccessToken = linebot.Value.ChannelAccessToken;
            _logger = logger;
        }

        [HttpGet]
        [Route("Sync")]
        public async Task<IActionResult> Index1()
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var fileNames = new string[]
            {
                @"C:\Users\Bu9\Downloads\2020 - 記帳.xlsx",
                @"C:\Users\Bu9\Downloads\2021 - 記帳.xlsx",
                @"C:\Users\Bu9\Downloads\2022 - 記帳.xlsx",
            };


            foreach (var fileName in fileNames)
            {
                Console.WriteLine("1234");
                var fileInfo = new FileInfo(fileName);

                var year = Convert.ToInt32(fileInfo.Name[..4]);

                using var package = new ExcelPackage(fileInfo);

                foreach (var worksheet in package.Workbook.Worksheets)
                {
                    var month = Convert.ToInt32(worksheet.Name[..2]);

                    for (var i = 4; ; i++)
                    {
                        _logger.LogInformation($"{year} - {month} - {i}");
                        if (worksheet.Cells[$"A{i}"].Value == null || string.IsNullOrWhiteSpace(worksheet.Cells[$"A{i}"].Value.ToString()))
                            break;
                        var aa = worksheet.Cells[$"C{i}"].Value;
                        var payEvent = await _eventService.GetEvent(worksheet.Cells[$"D{i}"].Value.ToString(), 1);
                        var pay = Convert.ToInt32(worksheet.Cells[$"C{i}"].Value.ToString());
                        var day = Convert.ToInt32(worksheet.Cells[$"A{i}"].Value.ToString());
                        var accountDate = new DateTime(year, month, day);
                        await _db.Accountings.AddAsync(new Accounting
                        {
                            AccountDate = accountDate,
                            Amount = pay,
                            CreateDate = DateTime.UtcNow,
                            EventId = payEvent.Id,
                            UserId = 1
                        });


                        if (worksheet.Cells[$"F{i}"].Value == null || string.IsNullOrWhiteSpace(worksheet.Cells[$"F{i}"].Value.ToString()))
                            continue;

                        var payEvent1 = await _eventService.GetEvent(worksheet.Cells[$"I{i}"].Value.ToString(), 3);
                        var pay1 = Convert.ToInt32(worksheet.Cells[$"H{i}"].Value.ToString());
                        var day1 = Convert.ToInt32(worksheet.Cells[$"F{i}"].Value.ToString());
                        var accountDate1 = new DateTime(year, month, day);
                        await _db.Accountings.AddAsync(new Accounting
                        {
                            AccountDate = accountDate1,
                            Amount = pay1,
                            CreateDate = DateTime.UtcNow,
                            EventId = payEvent1.Id,
                            UserId = 3
                        });

                    }

                }


            }

            await _db.SaveChangesAsync();

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
