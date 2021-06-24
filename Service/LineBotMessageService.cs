using ExtLib;
using Microsoft.Extensions.Options;
using Model.Appsetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class LineBotMessageService : ILineBotMessageService
    {
        public LineBotMessageService(IOptions<LineBot> lintBot)
        {
            _lineBot = lintBot.Value;
        }

        private readonly LineBot _lineBot;

        public void Process(isRock.LineBot.Event lineEvent)
        {
            var message = lineEvent.message.text;
            var textSplitData = message.Split('\n')
                                .Where(x => !x.Ext_IsNullOrEmpty())
                                .ToList<object>();
            var workSheetData = new List<IList<object>>();
            var twNow = DateTime.UtcNow.AddHours(8);
            var nowDay = twNow.ToString("dd");

            if (textSplitData.Count > 4)
                throw new Exception("格式錯誤！");

            switch(textSplitData.Count)
            {
                case 1:
                    textSplitData.InsertRange(0, new List<object> { nowDay, "現金" });
                    textSplitData.Add("其他");
                    break;
                case 2:
                    textSplitData.InsertRange(0, new List<object> { nowDay, "現金" });
                    break;
                case 3:
                    textSplitData.Insert(0,  nowDay);
                    break;
            }

            workSheetData.Add(textSplitData);

            var tableName = twNow.ToString("MM月");

            var startColumn = (_lineBot.Bo == lineEvent.source.userId ? "A" : "F");



        }
    }
}
