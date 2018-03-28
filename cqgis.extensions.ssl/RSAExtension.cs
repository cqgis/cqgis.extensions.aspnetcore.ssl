using System;
using System.Security.Cryptography;
using System.Xml;
using Microsoft.Extensions.DependencyInjection;

namespace cqgis.extensions.ssl
{
    public static class RSAExtension
    { 
        /// <summary>
        /// 添加加密相关组件
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="rsaKeyOptionBuilder"></param>
        /// <returns></returns>
        public static IServiceCollection AddRSAManager(this IServiceCollection serviceCollection,
            Action<RSAKeyOption> rsaKeyOptionBuilder)
        { 
            RSAKeyOption opt=new RSAKeyOption();
            rsaKeyOptionBuilder?.Invoke(opt);
            serviceCollection.AddSingleton(opt);
            
            serviceCollection.AddScoped<IEncryptManager, EncryptManager>(); 
            return serviceCollection;
        }



        /// <summary>
        /// 从xml中获取信息
        /// </summary>
        /// <param name="rsa"></param>
        /// <param name="xmlString"></param>
        public static void GetFromXmlString(this RSA rsa, string xmlString)
        {
            RSAParameters parameters = new RSAParameters();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);
            if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            {
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "Modulus": parameters.Modulus = Convert.FromBase64String(node.InnerText); break;
                        case "Exponent": parameters.Exponent = Convert.FromBase64String(node.InnerText); break;
                        case "P": parameters.P = Convert.FromBase64String(node.InnerText); break;
                        case "Q": parameters.Q = Convert.FromBase64String(node.InnerText); break;
                        case "DP": parameters.DP = Convert.FromBase64String(node.InnerText); break;
                        case "DQ": parameters.DQ = Convert.FromBase64String(node.InnerText); break;
                        case "InverseQ": parameters.InverseQ = Convert.FromBase64String(node.InnerText); break;
                        case "D": parameters.D = Convert.FromBase64String(node.InnerText); break;
                    }
                }
            }
            else
            {
                throw new Exception("Invalid XML RSA key.");
            }

            rsa.ImportParameters(parameters);
        }
    }
}
