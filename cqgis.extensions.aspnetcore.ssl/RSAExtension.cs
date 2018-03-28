using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Cryptography;
using System.Xml;
using cqgis.extensions.ssl;

namespace cqgis.extensions.aspnetcore.ssl
{
    public static class RSAExtension
    { 
        /// <summary>
        /// 添加加密相关组件
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="rsaKeyOptionBuilder"></param>
        /// <param name="sslTokenOptionBuilder"></param>
        /// <returns></returns>
        public static IServiceCollection AddAspNetCoreRSAManager(this IServiceCollection serviceCollection,
            Action<RSAKeyOption> rsaKeyOptionBuilder,
            Action<SSLTokenOption> sslTokenOptionBuilder=null)
        { 
            SSLTokenOption sllopt=new SSLTokenOption();
            sslTokenOptionBuilder?.Invoke(sllopt);
            serviceCollection.AddSingleton(sllopt);

            serviceCollection.AddRSAManager(rsaKeyOptionBuilder);
            
            return serviceCollection;
        }

        
    }
}
