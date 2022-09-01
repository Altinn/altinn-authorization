using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Altinn.AccessGroups.Core.Models;
using Altinn.AccessGroups.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace Altinn.AccessGroups.Controllers
{
    [ApiController]
    public class AccessGroupController : ControllerBase
    {
        private IAccessGroup _accessGroup;

        public AccessGroupController(IAccessGroup accessGroup)
        {
            _accessGroup = accessGroup;
        }

        [HttpPost]
        [Route("authorization/api/v1/[controller]/CreateGroup")]
        public async Task<ActionResult> CreateGroup([FromBody] AccessGroup accessGroup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(await _accessGroup.CreateGroup(accessGroup));
        }

        [HttpPost]
        [Route("authorization/api/v1/[controller]/UpdateGroup")]
        public async Task<ActionResult> UpdateGroup([FromBody] AccessGroup accessGroup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool result = await _accessGroup.UpdateGroup(accessGroup);

            return Ok(result);
        }

        [HttpGet]
        [Route("authorization/api/v1/[controller]/ExportAccessGroups")]
        public async Task<ActionResult> ExportAccessGroups()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<AccessGroup> result = await _accessGroup.ExportAccessGroups();

            return Ok(result);
        }

        [HttpPost]
        [Route("authorization/api/v1/[controller]/ImportAccessGroups")]
        public async Task<ActionResult> ImportAccessGroups([FromBody] List<AccessGroup> accessGroups)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool result = await _accessGroup.ImportAccessGroups(accessGroups);

            return Ok(result);
        }

        [HttpPost]
        [Route("authorization/api/v1/[controller]/ImportExternalRelationships")]
        public async Task<ActionResult> ImportExternalRelationships([FromBody] List<ExternalRelationship> externalRelationships)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(await _accessGroup.ImportExternalRelationships(externalRelationships));
        }

        [HttpGet]
        [Route("authorization/api/v1/[controller]/ExportExternalRelationships")]
        public async Task<ActionResult> ExportExternalRelationships()
        {
            return Ok(await _accessGroup.GetExternalRelationships());
        }

        [HttpGet]
        [Route("authorization/api/v1/[controller]/")]
        public string Get()
        {
            
            return "Hello world!";
        }
    }
}
