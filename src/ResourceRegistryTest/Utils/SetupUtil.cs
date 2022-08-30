using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using ResourceRegistry.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ResourceRegistryTest.Utils
{
    public static class SetupUtil
    {
        public static HttpClient GetTestClient(
            CustomWebApplicationFactory<ResourceController> customFactory)
        {
            WebApplicationFactory<ResourceController> factory = customFactory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    
                });
            });
            factory.Server.AllowSynchronousIO = true;
            return factory.CreateClient();
        }

    }
}
