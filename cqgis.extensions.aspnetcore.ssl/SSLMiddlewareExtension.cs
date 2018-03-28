using Microsoft.AspNetCore.Builder;

namespace cqgis.extensions.aspnetcore.ssl
{
    public static class SSLMiddlewareExtension
    {
        /// <summary>
        /// 返回加密后的信息，放在管道的第一位。返回的内容用对称加密，用rsa对加密的内容进行签名
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseReturnSSL(this IApplicationBuilder app)
        {
            app.UseMiddleware<SSLMiddleware>();

            return app;
        }
         
    }
}
