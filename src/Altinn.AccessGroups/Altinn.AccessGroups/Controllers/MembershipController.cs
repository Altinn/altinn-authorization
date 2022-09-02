using Altinn.AccessGroups.Core.Models;
using Altinn.AccessGroups.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Altinn.AccessGroups.Controllers
{
    [Route("authorization/api/v1/[controller]")]
    [ApiController]
    public class MembershipController : ControllerBase
    {
        private readonly IMemberships _membership;

        public MembershipController(IMemberships membership)
        {
            _membership = membership;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("authorization/api/v1/[controller]/ListGroupMemberships")]
        public async Task<ActionResult> ListGroupMemberships([FromBody] AccessGroupSearch search)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<AccessGroup> result = await _membership.ListGroupMemberships(search);

            return Ok(result);
        }

        [HttpPost]
        [Route("authorization/api/v1/[controller]/AddMembership")]
        public async Task<ActionResult> AddMembership([FromBody] GroupMembership input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            GroupMembership result = await _membership.AddMembership(input);

            return Ok(result);
        }

        [HttpPost]
        [Route("authorization/api/v1/[controller]/RevokeMembership")]
        public async Task<ActionResult> RevokeMembership([FromBody] GroupMembership input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool result = await _membership.RevokeMembership(input);

            return Ok(result);
        }
    }
}
