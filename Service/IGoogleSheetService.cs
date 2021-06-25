using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IGoogleSheetService
    {
        /// <summary>
        /// 寫入資料至GoogleSheet
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="startColumn"></param>
        /// <param name="insertValue"></param>
        /// <param name="sheetId"></param>
        /// <param name="credentialString"></param>
        /// <param name="row"></param>
        public void WriteValue(string range, List<IList<object>> insertValue);

        /// <summary>
        /// 取得行的總數
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="startColumn"></param>
        /// <returns></returns>
        int? GetTotalColumnCount(string tableName, string startColumn);

        /// <summary>
        /// 讀取值 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="range"></param>
        /// <param name="sheetId"></param>
        /// <param name="credentialString"></param>
        /// <returns></returns>
        public IList<IList<object>> ReadValue(string range);
    }
}
