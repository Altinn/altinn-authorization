using Altinn.ResourceRegistry.Core;
using Altinn.ResourceRegistry.Core.Models;
using Altinn.ResourceRegistry.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ResourceRegistry.Controllers
{
    [Route("ResourceRegistry/api/[controller]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private IResourceRegistry _resourceRegistry;
        private readonly IObjectModelValidator _objectModelValidator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ResourceController(IResourceRegistry resourceRegistry, IObjectModelValidator objectModelValidator, IHttpContextAccessor httpContextAccessor)
        {
            _resourceRegistry = resourceRegistry;
            _objectModelValidator = objectModelValidator;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("{id}")]
        public async Task<ServiceResource> Get(string id)
        {
            return await _resourceRegistry.GetResource(id);
        }

        [SuppressModelStateInvalidFilter] 
        [HttpPost]
        public async Task<ActionResult> Post([ValidateNever] ServiceResource serviceResource)
        {
            if(serviceResource.IsComplete.HasValue && serviceResource.IsComplete.Value)
            {
                if (!ModelState.IsValid)
                {
                    return ValidationProblem(ModelState);
                }
            }

            await _resourceRegistry.CreateResource(serviceResource);

            return Created("/ResourceRegistry/api/" + serviceResource.Identifier, null);
        }

        [SuppressModelStateInvalidFilter]
        [HttpPut]
        public async Task<ActionResult> Put(ServiceResource serviceResource)
        {
            if (serviceResource.IsComplete.HasValue && serviceResource.IsComplete.Value)
            {
                if (!ModelState.IsValid)
                {
                    return ValidationProblem(ModelState);
                }
            }

             await _resourceRegistry.UpdateResource(serviceResource);

            return Ok();
        }


        [HttpDelete("{id}")]
        public async void Delete(string id)
        {
            await _resourceRegistry.Delete(id);
        }

        [HttpGet("Search")]
        public async Task<List<ServiceResource>> Search([FromQuery] ResourceSearch search)
        {
            return await _resourceRegistry.Search(search);
        }
     }

    public class SuppressModelStateInvalidFilterAttribute : Attribute, IActionModelConvention
    {
        private const string FilterTypeName = "ModelStateInvalidFilterFactory";

        public void Apply(ActionModel action)
        {
            for (var i = 0; i < action.Filters.Count; i++)
            {
                if (action.Filters[i].GetType().Name == FilterTypeName)
                {
                    action.Filters.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
