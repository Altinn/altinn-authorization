using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Altinn.AccessGroups.Controllers
{
    [ApiController]
    [Route("accessgroups/api/v1/[controller]")]
    public class TestController : ControllerBase
    {
        public TestController()
        {
        }

        /// <summary>
        /// Test method
        /// </summary>
        /// <returns>test string</returns>
        [HttpGet]
        public string Get()
        {
            return "Hello world!";
        }
    }    
}
