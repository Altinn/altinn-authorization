using Altinn.ResourceRegistry.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ResourceRegistry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        public async Task<ServiceResource> Get(string id)
        {
            ServiceResource resource =  new ServiceResource();
            return resource;
        }
    }
}
