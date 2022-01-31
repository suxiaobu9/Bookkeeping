using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Model.Line
{
    public class AccountingFlexMessageModel
    {
        public AccountingFlexMessageModel(int AccountId, int MonthlyPay,int MonthlyIncome, string EventName, int Pay, DateTime CreateDate, bool IsConfirm, DateTime? Deadline = null)
        {
            this.AccountId = AccountId;
            this.MonthlyPay = MonthlyPay;
            this.MonthlyIncome = MonthlyIncome;
            this.EventName = EventName;
            this.Pay = Pay;
            this.CreateDate = CreateDate;
            this.IsConfirm = IsConfirm;
            this.Deadline = Deadline;

        }

        /// <summary>
        /// 帳務 Id
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// 本月花費
        /// </summary>
        public int MonthlyPay { get; set; }

        /// <summary>
        /// 本月收入
        /// </summary>
        public int MonthlyIncome { get; set; }

        /// <summary>
        /// 用途
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// 花費金額
        /// </summary>
        public int Pay { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 是否確定可刪除
        /// </summary>
        public bool IsConfirm { get; set; }

        /// <summary>
        /// 可刪除期限
        /// </summary>
        public DateTime? Deadline { get; set; }
    }
}
