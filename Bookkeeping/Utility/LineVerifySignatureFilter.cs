using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Model.Appsetting;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Hosting;

namespace Bookkeeping.Utility
{
    public class LineVerifySignatureFilter : IAuthorizationFilter
    {
        public LineVerifySignatureFilter(IOptions<LineBot> linebot,
            IWebHostEnvironment env)
        {
            _lineBot = linebot.Value;
            _env = env;
        }

        private readonly LineBot _lineBot;
        private readonly IWebHostEnvironment _env;

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //if (_env.IsDevelopment())
            //    return;

            context.HttpContext.Request.EnableBuffering();

            string requestBody = new StreamReader(context.HttpContext.Request.Body).ReadToEndAsync().Result;
            context.HttpContext.Request.Body.Position = 0;

            var xLineSignature = context.HttpContext.Request.Headers["X-Line-Signature"].ToString();
            var channelSecret = Encoding.UTF8.GetBytes(_lineBot.ChannelSecret);
            var body = Encoding.UTF8.GetBytes(requestBody);

            using (HMACSHA256 hmac = new HMACSHA256(channelSecret))
            {
                var hash = hmac.ComputeHash(body, 0, body.Length);
                var xLineBytes = Convert.FromBase64String(xLineSignature);
                if (!SlowEquals(xLineBytes, hash))
                {
                    //context.Result = new ForbidResult();
                }
            }
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
                diff |= (uint)(a[i] ^ b[i]);
            return diff == 0;
        }
    }
}
