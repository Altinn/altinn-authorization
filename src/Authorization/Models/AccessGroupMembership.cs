namespace Altinn.Platform.Authorization.Models
{
    /// <summary>
    /// Model for Access group membership
    /// </summary>
    public class AccessGroupMembership
    {
        /// <summary>
        /// The membership id
        /// </summary>
        public string MembershipId { get; set; }

        /// <summary>
        /// The access group code 
        /// </summary>
        public string AccessGroupCode { get; set; }

        /// <summary>
        /// The party that offers membership
        /// </summary>
        public int OfferedByPartyId { get; set; }

        /// <summary>
        /// The Member party ID . A person or organization
        /// </summary>
        public int? MemberPartyId { get; set;  }

        /// <summary>
        /// The original member partyID. Relevant when membership is inherited.
        /// </summary>
        public int? OriginalMemberPartyIdPartyId { get; set; }

        /// <summary>
        /// Member
        /// </summary>
        public int? MemberPartyIdUserId { get; set; }
    }
}
