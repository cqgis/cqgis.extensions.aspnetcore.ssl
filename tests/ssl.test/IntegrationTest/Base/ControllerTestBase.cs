using System.Net.Http;
using cqgis.extensions;

namespace ssl.test.IntegrationTest.Base
{
    public abstract class ControllerTestBase
    {
        private readonly string baseUrl;
        protected readonly HttpClient _client;

        protected ControllerTestBase(string ControllerName, HttpClient client)
        {
            this.baseUrl = $"api/{ControllerName.Replace("Controller", "")}";
            _client = client;
        }

        protected string GetUrl(string route = "")
        {
            if (route.IsNullorEmpty()) return baseUrl;
            return $"{baseUrl}/{route}";
        }
    }
}