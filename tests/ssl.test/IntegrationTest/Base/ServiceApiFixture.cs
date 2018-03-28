using System;
using System.Net.Http;
using System.Reflection;
using cqgis.extensions.ssl;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using webapisample;

namespace ssl.test.IntegrationTest.Base
{
    public class ServiceApiFixture : IDisposable
    {
        private readonly TestServer _server;
        public HttpClient Client { get; }

        public HttpClient NewClient => _server.CreateClient();

        public ServiceApiFixture()
        {
            var builder = new WebHostBuilder() 
                .ConfigureServices(InitializeServices) 
                .UseStartup<Startup>();
             
            _server = new TestServer(builder);

            var s= _server.Features.Get<IEncryptManager>();

           
            Client = _server.CreateClient();
            Client.BaseAddress = new Uri("http://localhost");
        }



        public void Dispose()
        {
            Client.Dispose();
            _server.Dispose();
        }

        protected virtual void InitializeServices(IServiceCollection services)
        {
            var startupAssembly = typeof(Startup).GetTypeInfo().Assembly;
            
            var manager = new ApplicationPartManager();
            manager.ApplicationParts.Add(new AssemblyPart(startupAssembly));

            manager.FeatureProviders.Add(new ControllerFeatureProvider());
            manager.FeatureProviders.Add(new ViewComponentFeatureProvider());

            services.AddSingleton(manager);
        }


    }


   
}
