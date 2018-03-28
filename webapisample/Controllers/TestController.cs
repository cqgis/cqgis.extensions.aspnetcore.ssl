using System;
using cqgis.extensions.data;
using Microsoft.AspNetCore.Mvc;

namespace webapisample.Controllers
{
    [Route("api/[controller]")]
    public class TestController
    {
        public TestController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected  IServiceProvider _serviceProvider { get; }

        
        [HttpGet("{value}")]
        public ServiceMessage<string> Get(string value)
        {
            return new ServiceMessage<string>(value);
        }


        [HttpPost]
        public ServiceMessage<TestInfo> Post([FromBody]TestInfo info)
        {
            return new ServiceMessage<TestInfo>(info);
        }

        public class TestInfo
        {
            public string UserName { get; set; }

            public string Pwd { get; set; }

        }

    }








}


