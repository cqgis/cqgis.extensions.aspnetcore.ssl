using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cqgis.extensions.ssl;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace cqgis.extensions.aspnetcore.ssl
{
    /// <summary>
    /// 使用RSA加密
    /// </summary>
    internal class SSLMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string heardToken = default;
        private readonly IEncryptManager em;
        private readonly ILogger<SSLMiddleware> logger;

        public SSLMiddleware(RequestDelegate next, SSLTokenOption tokenoption,
            IServiceProvider sp)
        {
            this._next = next;
            this.heardToken = tokenoption.HearderTokenKey;
            this.em = sp.GetRequiredService<IEncryptManager>();
            this.logger = sp.GetRequiredService<ILogger<SSLMiddleware>>();
        }

        public async Task Invoke(HttpContext context)
        {

            //处理header token
            var key = processhearder(context);

            //处理请求的body  
            processBody(context,key); 

            //请求 
            //加密返回
            await getResponse(context, key);
        }

        private void processBody(HttpContext context,string encryptKey)
        {
            if (context.Request.Method.ToUpper() != "POST") return;

            var requestbody = context.Request.Body; 
            requestbody.Seek(0, SeekOrigin.Begin);
            string json;
            using (var reader = new StreamReader(requestbody))
            {
                json = reader.ReadToEnd();
            }

            try
            {
                json = this.em.DecryptData(json).DecryptDes(encryptKey);
                var newBody = new MemoryStream(Encoding.UTF8.GetBytes(json));
                context.Request.Body = newBody;
            }
            catch (Exception e)
            {
                this.logger.LogException(e);
            }
            
        }

        /// <summary>
        /// 处理token的还原
        /// </summary>
        /// <param name="context"></param>
        private string processhearder(HttpContext context)
        {
            if (this.heardToken.IsNullorEmpty()) return string.Empty;
            if (!context.Request.Headers.ContainsKey(this.heardToken)) return string.Empty;

            var orignvalue = context.Request?.Headers[this.heardToken].FirstOrDefault();
            if (orignvalue == null) return string.Empty;

            //解密
            try
            {
                var value = this.em.DecryptData(orignvalue);
                var temp = value.Split(".");
                if (temp.Length != 2)
                {
                    context.Request.Headers[this.heardToken] = value;
                    return string.Empty;
                }

                var key = temp[1];
                context.Request.Headers[this.heardToken] = temp[0].DecryptDes(key);
                return key;
            }
            catch (Exception ey)
            {
                //
                this.logger.LogException(ey);
                return string.Empty;
            }
        }

        private async Task getResponse(HttpContext context, string encryptKey = "zhya")
        {
            if (encryptKey.IsNullorEmpty()) encryptKey = "zhya";

            var originBody = context.Response.Body;
            var newBody = new MemoryStream();
            context.Response.Body = newBody;


            await _next(context);

            newBody.Seek(0, SeekOrigin.Begin);
            string json;
            using (var reader = new StreamReader(newBody))
            {
                json = reader.ReadToEnd();
            }
            newBody.Dispose();
            var message = json.EncryptDes(encryptKey);  //加密

            var sign = em.SignInfo(message.MD5());  //签名
            var result = $"{message}.{sign}";
            context.Response.Body = originBody;
            await context.Response.WriteAsync(result);
        }
    }
}