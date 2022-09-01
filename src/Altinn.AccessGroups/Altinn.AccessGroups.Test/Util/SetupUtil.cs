using Altinn.AccessGroups.Controllers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Altinn.AccessGroups.Core;
using Altinn.AccessGroups.Test.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Altinn.AccessGroups.Interfaces;

namespace Altinn.AccessGroups.Test.Util
{
    public static class SetupUtil
    {
        public static HttpClient GetTestClient(
            CustomWebApplicationFactory<AccessGroupController> customFactory)
        {
            WebApplicationFactory<AccessGroupController> factory = customFactory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IAccessGroupsRepository, AccessGroupRepositoryMock>();
                    services.AddSingleton<IAccessGroup, AccessGroupServiceMock>();
                });
            });
            factory.Server.AllowSynchronousIO = true;
            return factory.CreateClient();
        }
    }
}
