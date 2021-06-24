using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface ILineBotMessageService
    {
        public void Process(isRock.LineBot.Event lineEvent);
    }
}
