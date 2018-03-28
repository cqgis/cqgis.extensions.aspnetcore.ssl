using System;
using Microsoft.Extensions.Logging;

namespace cqgis.extensions.aspnetcore.ssl
{
    internal static class LoggerExtension
    {
        /// <summary>日志记录异常的方法</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="ey"></param>
        public static void LogException<T>(this ILogger<T> logger, Exception ey)
        {
            var error = $"{ey?.Message}\r\n{ey?.StackTrace}\r\n{ey?.InnerException?.Message}";
            logger.LogError(error);
        }
    }
}