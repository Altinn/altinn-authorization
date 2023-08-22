using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Altinn.Platform.Authorization.Models;
using Altinn.Platform.Authorization.Services.Interface;
using Altinn.Platform.Register.Models;
using Authorization.Platform.Authorization.Models;

namespace Altinn.Platform.Authorization.IntegrationTests.MockServices
{
    public class OedRoleAssignmentWrapperMock : IOedRoleAssignmentWrapper
    {
        public Task<List<OedRoleAssignment>> GetOedRoleAssignments(string from, string to)
        {
            List<OedRoleAssignment> oedRoleAssignments = new();
            if (from == "10987654321" && to == "12345678910")
            {
                OedRoleAssignment oedRoleAssignment = new OedRoleAssignment
                {
                    Created = DateTime.Now,
                    From = from,
                    To = to,
                    OedRoleCode = "urn:digitaltdodsbo:formuesfullmakt"
                };
                oedRoleAssignments.Add(oedRoleAssignment);
            }

            return Task.FromResult(oedRoleAssignments);
        }
    }
}
