using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Altinn.AccessGroups.Controllers
{
    [ApiController]
    public class TestController : ControllerBase
    {
        public TestController()
        {
        }

        /// <summary>
        /// Test method. Should be deleted?
        /// </summary>
        /// <returns>test string</returns>
        [HttpGet]
        [Route("accessgroups/api/v1/[controller]")]
        public string Get()
        {
            return "Hello world!";
        }
    }    
}
