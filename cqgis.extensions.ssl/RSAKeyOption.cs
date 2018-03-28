namespace cqgis.extensions.ssl
{
    /// <summary>
    /// 非对称加密的配置信息
    /// </summary>
    public class RSAKeyOption
    {
        internal RSAKeyOption()
        {

        }

        /// <summary>
        /// 私有密钥加密后的字符串
        /// </summary>
        public string PrivateKey_Des { get; set; }

        /// <summary>
        /// 公有密钥加密后的字符串
        /// </summary>
        public string PublicKey_Des { get; set; }


        /// <summary>
        /// 密钥加密的盐值 
        /// </summary>
        public string DecryptKey { get; set; } = "ssllockkey";


        /// <summary>
        /// 返回解密后的私钥
        /// </summary>
        public string PrivateKey => PrivateKey_Des.IsNullorEmpty() ? string.Empty : PrivateKey_Des.DecryptDes(DecryptKey);

        /// <summary>
        /// 返回解密后的公钥
        /// </summary>
        public string PublicKey => PublicKey_Des.IsNullorEmpty() ? string.Empty : PublicKey_Des.DecryptDes(DecryptKey);

    }
}
