using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace cqgis.extensions.ssl
{

    internal  sealed class EncryptManager : IEncryptManager
    {
        private readonly string privatekey;
        private readonly string publickey;

         
        public EncryptManager(RSAKeyOption keyoption)
        {
            (keyoption?.PublicKey).CheckIsNullOrEmpty(nameof(publickey));
            (keyoption?.PrivateKey).CheckIsNullOrEmpty(nameof(privatekey));

            keyoption.CheckIsNull(nameof(keyoption));

            Debug.Assert(keyoption != null, nameof(keyoption) + " != null");

            privatekey = keyoption.PrivateKey;
            publickey = keyoption.PublicKey;
        }


        /// <summary>
        /// 用私钥对信息进行签名
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string SignInfo(string content)
        {
            content.CheckIsNullOrEmpty(nameof(content));
            var bytes = Encoding.UTF8.GetBytes(content);

            using (var pri_rsa = new RSACryptoServiceProvider())
            {
                pri_rsa.GetFromXmlString(privatekey);
                using (var sh = SHA1.Create())
                {
                    byte[] signData = pri_rsa.SignData(bytes, sh);
                    return Convert.ToBase64String(signData);
                }

            }
        }


        /// <summary>
        /// 用公钥验证签名
        /// </summary>
        /// <param name="content">原始内容</param>
        /// <param name="signinfo">签名信息</param>
        /// <returns></returns>
        public bool Verify(string content, string signinfo)
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
        public string Encrypt(string content)
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
        /// 用私钥解密信息
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string DecryptData(string content)
        {
            content.CheckIsNullOrEmpty(nameof(content));
            var bytes = content.Frombase64TobBytes();

            using (var pri_rsa = new RSACryptoServiceProvider())
            {
                pri_rsa.GetFromXmlString(privatekey);
                var MaxBlockSize = pri_rsa.KeySize / 8;    //解密块最大长度限制
                if (bytes.Length <= MaxBlockSize)
                {
                    var temp = pri_rsa.Decrypt(bytes, false);
                    return Encoding.UTF8.GetString(temp);
                }

                using (MemoryStream CrypStream = new MemoryStream(bytes))
                using (MemoryStream PlaiStream = new MemoryStream())
                {
                    var Buffer = new Byte[MaxBlockSize];
                    var BlockSize = CrypStream.Read(Buffer, 0, MaxBlockSize);

                    while (BlockSize > 0)
                    {
                        var ToEncrypt = new Byte[BlockSize];
                        Array.Copy(Buffer, 0, ToEncrypt, 0, BlockSize);

                        var plaintext = pri_rsa.Decrypt(ToEncrypt, false);
                        PlaiStream.Write(plaintext, 0, plaintext.Length);

                        BlockSize = CrypStream.Read(Buffer, 0, MaxBlockSize);
                    }

                    return Encoding.UTF8.GetString(PlaiStream.ToArray());
                }

            }
        }


    }
}
