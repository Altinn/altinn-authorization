using Altinn.ResourceRegistry.Core;
using Altinn.ResourceRegistry.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ResourceRegistry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {

        private IResourceRegistry _resourceRegistry;

        public ResourceController(IResourceRegistry resourceRegistry)
        {
            _resourceRegistry = resourceRegistry;
        }

        [HttpGet("{id}")]
        public async Task<ServiceResource> Get(string id)
        {
            return await _resourceRegistry.GetResource(id);
        }

        [HttpPost]
        public async void Post(ServiceResource serviceResource)
        {
            await _resourceRegistry.CreateResource(serviceResource);
        }


        [HttpPut]
        public async void Put(ServiceResource serviceResource)
        {
            await _resourceRegistry.UpdateResource(serviceResource);
        }


        [HttpDelete("{id}")]
        public async void  Delete(string id)
        {
            await _resourceRegistry.Delete(id);
        }
    }
}
