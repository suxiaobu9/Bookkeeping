using Model.Line;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Model.StaticData
{
    public static class LineFlexTemplate
    {
        private static string FlexMessage(string message) => $@"
[{{
    ""type"": ""flex"",
    ""altText"": ""記帳明細"",
    ""contents"":
        {message}
}}]

";

        public static string AccountingFlexMessageTemplate(AccountingFlexMessageModel model) =>
            FlexMessage($@"
{{
  ""type"": ""bubble"",
  ""body"": {{
    ""type"": ""box"",
    ""layout"": ""vertical"",
    ""contents"": [
      {{
        ""type"": ""text"",
        ""text"": ""DETAIL"",
        ""weight"": ""bold"",
        ""color"": ""#1DB446"",
        ""size"": ""sm""
      }},
      {{
        ""type"": ""text"",
        ""text"": ""記帳明細"",
        ""weight"": ""bold"",
        ""size"": ""xxl"",
        ""margin"": ""md""
      }},
      {{
        ""type"": ""box"",
        ""layout"": ""horizontal"",
        ""contents"": [
          {{
            ""type"": ""text"",
            ""text"": ""本月花費"",
            ""size"": ""xs"",
            ""color"": ""#aaaaaa"",
            ""wrap"": true,
            ""flex"": 0
          }},
          {{
            ""type"": ""text"",
            ""text"": ""{model.MonthlyPay}"",
            ""size"": ""xs"",
            ""color"": ""#aaaaaa"",
            ""align"": ""end""
          }}
        ],
        ""offsetTop"": ""10px"",
        ""paddingStart"": ""10px""
      }},
      {{
        ""type"": ""box"",
        ""layout"": ""horizontal"",
        ""contents"": [
          {{
            ""type"": ""text"",
            ""text"": ""本月收入"",
            ""size"": ""xs"",
            ""color"": ""#aaaaaa"",
            ""wrap"": true,
            ""flex"": 0
          }},
          {{
            ""type"": ""text"",
            ""text"": ""{model.MonthlyIncome}"",
            ""size"": ""xs"",
            ""color"": ""#aaaaaa"",
            ""align"": ""end""
          }}
        ],
        ""offsetTop"": ""10px"",
        ""paddingStart"": ""10px""
      }},
      {{
        ""type"": ""separator"",
        ""margin"": ""xxl""
      }},
      {{
        ""type"": ""box"",
        ""layout"": ""vertical"",
        ""margin"": ""xxl"",
        ""spacing"": ""sm"",
        ""contents"": [
          {{
            ""type"": ""box"",
            ""layout"": ""horizontal"",
            ""contents"": [
              {{
                ""type"": ""text"",
                ""text"": ""用途"",
                ""size"": ""sm"",
                ""color"": ""#555555"",
                ""flex"": 0
              }},
              {{
                ""type"": ""text"",
                ""text"": ""{model.EventName}"",
                ""size"": ""sm"",
                ""color"": ""#111111"",
                ""align"": ""end""
              }}
            ]
          }},
          {{
            ""type"": ""box"",
            ""layout"": ""horizontal"",
            ""contents"": [
              {{
                ""type"": ""text"",
                ""text"": ""金額"",
                ""size"": ""sm"",
                ""color"": ""#555555"",
                ""flex"": 0
              }},
              {{
                ""type"": ""text"",
                ""text"": ""{model.Pay}"",
                ""size"": ""sm"",
                ""color"": ""#111111"",
                ""align"": ""end""
              }}
            ]
          }}
        ]
      }},
      {{
        ""type"": ""separator"",
        ""margin"": ""xxl""
      }},
      {{
        ""type"": ""box"",
        ""layout"": ""horizontal"",
        ""contents"": [
          {{
            ""type"": ""button"",
            ""action"": {{
              ""type"": ""postback"",
              ""label"": ""刪除"",
              ""data"": ""{Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(model)))}""
            }},
            ""margin"": ""5px"",
            ""style"": ""primary"",
            ""height"": ""sm"",
            ""color"": ""#DC3545""
          }}
        ],
        ""margin"": ""md"",
        ""width"": ""90px""
      }},
      {{
        ""type"": ""box"",
        ""layout"": ""horizontal"",
        ""margin"": ""md"",
        ""contents"": [
          {{
            ""type"": ""text"",
            ""text"": ""{model.CreateDate:yyyy-MM-dd HH:mm}"",
            ""color"": ""#aaaaaa"",
            ""size"": ""xxs"",
            ""align"": ""end""
          }}
        ]
      }}
    ]
  }},
  ""styles"": {{
    ""footer"": {{
      ""separator"": true
    }}
  }}
}}");

        public static string DeleteAccountingComfirm(AccountingFlexMessageModel model) =>
            FlexMessage($@"
{{
  ""type"": ""bubble"",
  ""body"": {{
    ""type"": ""box"",
    ""layout"": ""vertical"",
    ""contents"": [
      {{
        ""type"": ""text"",
        ""text"": ""確定要刪除 ? "",
        ""size"": ""xl"",
        ""weight"": ""bold""
      }},
      {{
        ""type"": ""separator"",
        ""margin"": ""5px""
      }},
      {{
    ""type"": ""box"",
        ""layout"": ""horizontal"",
        ""contents"": [
          {{
        ""type"": ""text"",
            ""text"": ""用途"",
            ""flex"": 0,
            ""size"": ""sm""
          }},
          {{
        ""type"": ""text"",
            ""text"": ""{model.EventName}"",
            ""align"": ""end"",
            ""size"": ""sm""
          }}
        ],
        ""paddingTop"": ""10px""
      }},
      {{
    ""type"": ""box"",
        ""layout"": ""horizontal"",
        ""contents"": [
          {{
        ""type"": ""text"",
            ""text"": ""金額"",
            ""flex"": 0,
            ""size"": ""sm""
          }},
          {{
        ""type"": ""text"",
            ""text"": ""{model.Pay}"",
            ""align"": ""end"",
            ""size"": ""sm""
          }}
        ],
        ""paddingBottom"": ""15px"",
        ""paddingTop"": ""5px""
      }},
      {{
    ""type"": ""separator""
      }},
      {{
    ""type"": ""box"",
        ""layout"": ""horizontal"",
        ""contents"": [
          {{
        ""type"": ""button"",
            ""action"": {{
            ""type"": ""postback"",
              ""label"": ""確定"",
              ""data"": ""{Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(model)))}""
            }},
            ""color"": ""#DC3545"",
            ""style"": ""primary"",
            ""height"": ""sm""
          }}
        ],
        ""width"": ""90px"",
        ""margin"": ""10px""
      }}
    ]
  }}
}}");
    }
}
