using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Altinn.Platform.Authorization.Models
{
    /// <summary>
    /// Model for a list of OED role assignment
    /// </summary>
    public class OedRoleAssignments
    {
        /// <summary>
        /// The list of OED role assignments
        /// </summary>
        public List<OedRoleAssignment> RoleAssignments { get; set; }
    }
}