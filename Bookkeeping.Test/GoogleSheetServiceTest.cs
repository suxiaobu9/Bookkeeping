using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Appsetting;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookkeeping.Test
{
    [TestClass]
    public class GoogleSheetServiceTest
    {
        private IOptions<GoogleSheetCredential> _googleSheetCredential { get; set; }
        private IOptions<GoogleSheetModel> _googleSheet { get; set; }

        private IGoogleSheetService _googleSheetService { get; set; }

        private string sheetName => "GoogleSheetTest";

        [TestInitialize]
        public void Init()
        {
            var googleSheetCredential = new GoogleSheetCredential();
            var googleSheet = new GoogleSheetModel();
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile(@"appsettings.json", false)
                .AddUserSecrets("136c518e-7f41-48ab-b641-3facb3f2782c")
                .Build();

            config.GetSection("GoogleSheetCredential").Bind(googleSheetCredential);
            config.GetSection("GoogleSheet").Bind(googleSheet);

            _googleSheetCredential = Options.Create(googleSheetCredential);
            _googleSheet = Options.Create(googleSheet);

            _googleSheetService = new GoogleSheetService(_googleSheetCredential, _googleSheet);
        }

        [TestMethod]
        public void Get_Google_Sheet_Total_Count()
        {
            var count = _googleSheetService.GetTotalColumnCount(sheetName, "A");
            Assert.IsNotNull(count);
        }

        [TestMethod]
        public void Get_Google_Sheet_Value()
        {
            var value = _googleSheetService.ReadValue($"{sheetName}!A1");
            Assert.IsTrue(value != null);

            Assert.AreEqual(value[0][0], "GoogleSheetValue");

        }

        [TestMethod]
        public void Write_Value_To_Google_Sheet()
        {
            var oldCount = _googleSheetService.GetTotalColumnCount(sheetName, "A");
            _googleSheetService.WriteValue($"{sheetName}!A{oldCount + 1}", new List<IList<object>> { new List<object> { oldCount + 1 } });
            var newCount = _googleSheetService.GetTotalColumnCount(sheetName, "A");
            Assert.IsTrue(oldCount < newCount);
        }

        [TestMethod]
        public void Template_Sheet_Exist()
        {
            var isExist = _googleSheetService.IsTemplateSheetExists();
            Assert.IsTrue(isExist);
        }

        [TestMethod]
        public void Sheet_Exist()
        {
            var isExist = _googleSheetService.IsSheetExist(sheetName);
            Assert.IsTrue(isExist);
        }

        [TestMethod]
        public void Create_And_Delete_Sheet_From_Template()
        {
            _googleSheetService.CreateSheetFromTemplate(DateTime.Now.ToString("yyyyMMdd"));
            _googleSheetService.DeleteSheet(DateTime.Now.ToString("yyyyMMdd"));

            Assert.IsTrue(true);
        }
    }
}
