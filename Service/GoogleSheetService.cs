using ExtLib;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Requests;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Options;
using Model.Appsetting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service
{
    public class GoogleSheetService : IGoogleSheetService
    {
        private readonly SheetsService _sheetsService;
        private readonly GoogleSheetModel _googleSheetModel;

        public GoogleSheetService(
             IOptions<GoogleSheetCredential> googleSheetCredential,
             IOptions<GoogleSheetModel> googleSheetModel)
        {
            var credentialString = googleSheetCredential.Value.Ext_ToJson();
            _sheetsService = OpenSheet(credentialString);
            _googleSheetModel = googleSheetModel.Value;
        }
        private SheetsService OpenSheet(string credentialString)
        {

            var credentialBytes = Encoding.UTF8.GetBytes(credentialString);

            // If modifying these scopes, delete your previously saved credentials
            // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
            string[] Scopes = { SheetsService.Scope.Spreadsheets };
            string ApplicationName = "Bookkeeping Google Sheet";

            UserCredential credential;

            using (Stream stream = new MemoryStream(credentialBytes))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            return service;
        }

        public IList<IList<object>> ReadValue(string range)
        {
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    _sheetsService.Spreadsheets.Values.Get(_googleSheetModel.SpreadSheetId, range);

            ValueRange response = request.Execute();
            IList<IList<object>> values = response.Values;

            return values;
        }

        public int? GetTotalColumnCount(string tableName, string startColumn)
        {
            //設定讀取最後一行位置
            var range = $"{tableName}!{startColumn}:{startColumn}";

            //最後一行位置
            IList<IList<object>> values = ReadValue(range);

            return values?.Count;
        }

        /// <summary>
        /// 從LineBot寫入資料至GoogleSheet
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="startColumn"></param>
        /// <param name="insertValue"></param>
        /// <param name="sheetId"></param>
        /// <param name="credentialString"></param>
        /// <param name="row"></param>
        public void WriteValue(string range, List<IList<object>> insertValue)
        {
            var writenRange = new ValueRange
            {
                Range = range,
                Values = insertValue,
                MajorDimension = "ROWS"
            };

            SpreadsheetsResource.ValuesResource.UpdateRequest upd
            = _sheetsService.Spreadsheets.Values.Update(writenRange, _googleSheetModel.SpreadSheetId, range);

            upd.ValueInputOption =
                SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

            UpdateValuesResponse responses = upd.Execute();
        }

        private Spreadsheet GetAllSheets()
        {
            var request = _sheetsService.Spreadsheets.Get(_googleSheetModel.SpreadSheetId);
            var response = request.Execute();
            return response;
        }

        /// <summary>
        /// 範本是否存在
        /// </summary>
        /// <returns></returns>
        public bool IsTemplateSheetExists()
        {
            var response = GetAllSheets();
            return response.Sheets.Any(x => x.Properties.SheetId.ToString() == _googleSheetModel.TemplateSheetId);
        }

        public bool IsSheetExist(string name)
        {
            var response = GetAllSheets();
            return response.Sheets.Any(x => x.Properties.Title == name);
        }

        /// <summary>
        /// 複製template建立新的Sheet
        /// </summary>
        public void CreateSheetFromTemplate(string newSheetName)
        {
            var copyRequestBody = new CopySheetToAnotherSpreadsheetRequest
            {
                DestinationSpreadsheetId = _googleSheetModel.SpreadSheetId
            };

            var copyRequest = _sheetsService.Spreadsheets.Sheets.CopyTo(copyRequestBody, _googleSheetModel.SpreadSheetId, Convert.ToInt32(_googleSheetModel.TemplateSheetId));
            var copyResponse = copyRequest.Execute();
            var cloneSheetId = copyResponse.SheetId;

            var renameRequestBody = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request>
                {
                    new Request
                    {
                        UpdateSheetProperties = new UpdateSheetPropertiesRequest
                        {
                            Fields= "title",
                            Properties = new SheetProperties
                            {
                                SheetId = cloneSheetId,
                                Title = newSheetName
                            }
                        }
                    }
                }
            };

            var renameRequest = _sheetsService.Spreadsheets.BatchUpdate(renameRequestBody, _googleSheetModel.SpreadSheetId);
            renameRequest.Execute();
        }

        /// <summary>
        /// 刪除sheet
        /// </summary>
        /// <param name="name"></param>
        public void DeleteSheet(string name)
        {
            var targetSheet = GetAllSheets().Sheets.FirstOrDefault(x => x.Properties.Title == name);
            if (targetSheet == null)
                return;

            var deleteRequestBody = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request>
                {
                    new Request
                    {
                        DeleteSheet = new DeleteSheetRequest { SheetId = targetSheet.Properties.SheetId }
                    }
                }
            };

            var deleteRequest = _sheetsService.Spreadsheets.BatchUpdate(deleteRequestBody, _googleSheetModel.SpreadSheetId);
            deleteRequest.Execute();

        }
    }
}
