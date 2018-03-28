using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using cqgis.extensions.data;
using Newtonsoft.Json;

namespace ssl.test
{
    public static class HttpClientExtension
    {
        /// <summary>
        /// 用Get方法返回某个接口提供的内容，并反序列化成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_client"></param>
        /// <param name="url"></param>
        /// <param name="encryptKey">用于加密的密钥</param>
        /// <returns></returns>
        public static async Task<ServiceMessage<T>> GetResultAsync<T>(this HttpClient _client, string url,string encryptKey)
        {
            var temp = await _client.GetAsync(url);
            temp.EnsureSuccessStatusCode(); 
            var tempresult = await temp.Content.ReadAsStringAsync(); 
            var result =
                JsonConvert.DeserializeObject<ServiceMessage<T>>(tempresult.GetDesEncrypContent(encryptKey));

            return result;
        }


        /// <summary>
        /// Post请求数据，并从Json字符串返回结果对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <param name="encryptKey"></param>
        /// <param name="pas"></param>
        /// <returns></returns>
        public static async Task<ServiceMessage<T>> SendPostAsync<T>(this HttpClient client, string url,
            string encryptKey,
            dynamic pas = null)
        {
            if (pas == null)
                pas = new Dictionary<string, string>();
            string temp = JsonConvert.SerializeObject(pas).ToString();
            temp = temp.ContentEncrypt(encryptKey);

            var stringcontent = new StringContent(temp, Encoding.UTF8, "application/json");

            return await SendPostContentAsync<T>(client, url, encryptKey, stringcontent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <param name="stringcontent"></param>
        /// <param name="encryptKey"></param>
        /// <returns></returns>
        public static async Task<ServiceMessage<T>> SendPostContentAsync<T>(this HttpClient client, string url, string encryptKey,
            StringContent stringcontent)
        {
            var servicemessage = await client.PostAsync(url, stringcontent);
            servicemessage.EnsureSuccessStatusCode();
            var content = await servicemessage.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ServiceMessage<T>>(content.GetDesEncrypContent(encryptKey));
            return result;
        }
    }
}
