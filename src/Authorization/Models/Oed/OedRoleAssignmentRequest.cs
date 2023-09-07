namespace Altinn.Platform.Authorization.Models
{
    /// <summary>
    /// Model for Access group membership
    /// </summary>
    public class OedRoleAssignmentRequest
    {
        /// <summary>
        /// The OED role code 
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// The OED role code 
        /// </summary>
        public string To { get; set; }
    }
}