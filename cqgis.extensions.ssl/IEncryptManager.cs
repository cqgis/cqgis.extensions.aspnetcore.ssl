namespace cqgis.extensions.ssl
{
    public interface IEncryptManager
    {

        /// <summary>
        /// 用私钥对信息进行签名
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        string SignInfo(string content);

        /// <summary>
        /// 用公钥验证签名
        /// </summary>
        /// <param name="content">原始内容</param>
        /// <param name="signinfo">签名信息</param>
        /// <returns></returns>
        bool Verify(string content, string signinfo);


        /// <summary>
        /// 用公钥加密信息
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        string Encrypt(string content);


        /// <summary>
        /// 用私钥解密信息
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        string DecryptData(string content);
    }
}
