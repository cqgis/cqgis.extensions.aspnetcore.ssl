using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using cqgis.extensions;
using cqgis.extensions.data;
using Newtonsoft.Json;
using ssl.test.IntegrationTest.Base;
using webapisample.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace ssl.test.IntegrationTest
{
    [Collection(nameof(ServiceTestCollection))]
    public class TestController_Test : ControllerTestBase
    {

        private readonly ITestOutputHelper _output;
        private readonly ServiceApiFixture _fixture;

        public TestController_Test(ITestOutputHelper output, ServiceApiFixture fixture) : base(nameof(TestController), fixture.Client)
        {
            _output = output;
            _fixture = fixture;

          
            


        }


        [Fact]
        public async Task Test_Get_SayHello()
        {
            var url = this.GetUrl("hellozhya");
            _output.WriteLine(url);

            var _newclient = this._fixture.NewClient;
            var encryptkey = "!23";

            //token用公钥加密
            _newclient.DefaultRequestHeaders.Add("ssltoken", "token123".TokenEncrypt(encryptkey));

            var temp = await _newclient.GetAsync(url);
            temp.EnsureSuccessStatusCode();
            var message = await temp.Content.ReadAsStringAsync();


            var infos = message.Split(".");
            infos.Length.ShouldBe(2);

            var content = infos[0];
            var signa = infos[1];

            content.ShouldNotNullOrEmpty();
            signa.ShouldNotNullOrEmpty();


            _output.WriteLine(content);
            _output.WriteLine(signa);


            content.MD5().Verify(signa).ShouldBe(true);
            var value = content.DecryptDes(encryptkey);
            _output.WriteLine(value);

            var valueobj = JsonConvert.DeserializeObject<ServiceMessage<string>>(value);
            valueobj.ShouldNotNull();
            valueobj.Success.ShouldBe(true);
            valueobj.Data.ShouldBe("hellozhya");

            _newclient.Dispose();
        }



        [Fact]
        public async Task Test_Post_TestInfo_UseClientExtension()
        {
            var url = this.GetUrl();
            _output.WriteLine(url);

            var _newclient = this._fixture.NewClient;

            var encryptkey = "!23";
            //token用公钥加密
            _newclient.DefaultRequestHeaders.Add("ssltoken", "token123".TokenEncrypt(encryptkey));

            var tempcontent = new
            {
                UserName = "zhya",
                Pwd = "123456",
            };
             
            var valueobj = await _newclient.SendPostAsync<TestInfo>(url,encryptkey, tempcontent);

            valueobj.ShouldNotNull();
            valueobj.Success.ShouldBe(true);
            valueobj.Data.ShouldNotNull();
            valueobj.Data.UserName.ShouldBe("zhya");
            valueobj.Data.Pwd.ShouldBe("123456");

            _newclient.Dispose();
        }


        [Fact]
        public async Task Test_Post_TestInfo()
        {
            var url = this.GetUrl();
            _output.WriteLine(url);

            var _newclient = this._fixture.NewClient;

            var encryptkey = "!23";
            //token用公钥加密
            _newclient.DefaultRequestHeaders.Add("ssltoken", "token123".TokenEncrypt(encryptkey));

            var tempcontent = JsonConvert.SerializeObject(new
            {
                UserName = "zhya",
                Pwd = "123",
            });

            var stringcontent = new StringContent(tempcontent.ContentEncrypt(encryptkey), Encoding.UTF8, "application/json");

            var servicemessage = await _newclient.PostAsync(url, stringcontent);
            servicemessage.EnsureSuccessStatusCode();
            var message = await servicemessage.Content.ReadAsStringAsync();


            var infos = message.Split(".");
            infos.Length.ShouldBe(2);

            var content = infos[0];
            var signa = infos[1];

            content.ShouldNotNullOrEmpty();
            signa.ShouldNotNullOrEmpty();


            _output.WriteLine(content);
            _output.WriteLine(signa);


            content.MD5().Verify(signa).ShouldBe(true);
            var value = content.DecryptDes(encryptkey);
            _output.WriteLine(value);

            var valueobj = JsonConvert.DeserializeObject<ServiceMessage<TestInfo>>(value);
            valueobj.ShouldNotNull();
            valueobj.Success.ShouldBe(true);
            valueobj.Data.ShouldNotNull();
            valueobj.Data.UserName.ShouldBe("zhya");
            valueobj.Data.Pwd.ShouldBe("123");

            _newclient.Dispose();
        }

        public class TestInfo
        {
            public string UserName { get; set; }

            public string Pwd { get; set; }

        }
    }
}
