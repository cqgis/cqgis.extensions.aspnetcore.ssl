using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using cqgis.extensions;
using cqgis.extensions.ssl;

namespace ssl.test
{
    public static class RSAExtension
    {
        const string publickey = "<RSAKeyValue><Modulus>5L7KS7UsYtST9dPfY8sX4wg28ecqtKSkZfgZhtd0Tj1idB7fnRHnxXwdDWRG94XmGgx81iBNZWDINEQSYbvQiRMZS64LZkaJLPawkUWOJL1YwAjeFzF33dL9ElGG6IDuyLbxsI70W5D3WdAncTpW+we90Yr+ctRcfSkqzLIdQtU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        /// <summary>
        /// 用公钥验证签名
        /// </summary>
        /// <param name="content">原始内容</param>
        /// <param name="signinfo">签名信息</param>
        /// <returns></returns>
        public static bool Verify(this string content, string signinfo)
        {
            content.CheckIsNullOrEmpty(nameof(content));
            signinfo.CheckIsNullOrEmpty(nameof(signinfo));

            var signinfobyte = signinfo.Frombase64TobBytes();
            var contentinfobyte = Encoding.UTF8.GetBytes(content);

            using (var pub_sra = new RSACryptoServiceProvider())
            {
                pub_sra.GetFromXmlString(publickey);
                using (var sh = SHA1.Create())
                {
                    var result = pub_sra.VerifyData(contentinfobyte, sh, signinfobyte);
                    return result;
                }
            }
        }


        /// <summary>
        /// 用公钥加密信息
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RSAEncrypt(this string content)
        {
            content.CheckIsNullOrEmpty(nameof(content));
            var bytes = Encoding.UTF8.GetBytes(content);

            using (var pub_sra = new RSACryptoServiceProvider())
            {
                pub_sra.GetFromXmlString(publickey);
                var MaxBlockSize = pub_sra.KeySize / 8 - 11;    //加密块最大长度限制;
                if (bytes.Length <= MaxBlockSize)
                {
                    var temp = pub_sra.Encrypt(bytes, false);
                    return temp.ToBase64String();
                }

                //分段加密

                using (MemoryStream PlaiStream = new MemoryStream(bytes))
                using (MemoryStream CrypStream = new MemoryStream())
                {
                    var Buffer = new Byte[MaxBlockSize];
                    var BlockSize = PlaiStream.Read(Buffer, 0, MaxBlockSize);

                    while (BlockSize > 0)
                    {
                        var ToEncrypt = new Byte[BlockSize];
                        Array.Copy(Buffer, 0, ToEncrypt, 0, BlockSize);

                        var Cryptograph = pub_sra.Encrypt(ToEncrypt, false);
                        CrypStream.Write(Cryptograph, 0, Cryptograph.Length);

                        BlockSize = PlaiStream.Read(Buffer, 0, MaxBlockSize);
                    }

                    return Convert.ToBase64String(CrypStream.ToArray(), Base64FormattingOptions.None);
                }
            }
        }



        /// <summary>
        /// 加密token值
        /// </summary>
        /// <param name="token"></param>
        /// <param name="encryptkey"></param>
        /// <returns></returns>
        public static string TokenEncrypt(this string token, string encryptkey)
        {
            token.CheckIsNullOrEmpty(nameof(token));
            return $"{token.EncryptDes(encryptkey)}.{encryptkey}".RSAEncrypt();
        }


        /// <summary>
        /// 加密内容信息
        /// </summary>
        /// <param name="tempcontent"></param>
        /// <param name="encryptkey"></param>
        /// <returns></returns>
        public static string ContentEncrypt(this string tempcontent, string encryptkey)
        {
            tempcontent.CheckIsNullOrEmpty(nameof(tempcontent));
            return tempcontent.EncryptDes(encryptkey).RSAEncrypt();
        }


        /// <summary>
        /// 验证签名并返回解密后的信息
        /// </summary>
        /// <param name="content"></param>
        /// <param name="encryptkey"></param>
        /// <returns></returns>
        public static string GetDesEncrypContent(this string content, string encryptkey)
        {
            var infos = content.Split("."); 
            if (infos.Length != 2) return content;
             
            var c = infos[0];
            var signa = infos[1];

            if (c.MD5().Verify(signa) != true)
                throw new Exception("签名验证不通过");
             
            var value = c.DecryptDes(encryptkey);
            return value;
        }
    }
}

