using System;
using System.Text.Json.Serialization;

namespace Altinn.Platform.Authorization.Models
{
    /// <summary>
    /// This model describes a delegation change as stored in the Authorization postgre DelegationChanges table.
    /// </summary>
    public class DelegationChange
    {
        /// <summary>
        /// Gets or sets the delegation change id
        /// </summary>
        [JsonPropertyName("delegationchangeid")]
        public int DelegationChangeId { get; set; }

        /// <summary>
        /// Gets or sets the delegation change type
        /// </summary>
        [JsonPropertyName("delegationchangetype")]
        public DelegationChangeType DelegationChangeType { get; set; }

        /// <summary>
        /// Gets or sets the altinnappid. E.g. skd/skattemelding
        /// </summary>
        [JsonPropertyName("altinnappid")]
        public string AltinnAppId { get; set; }

        /// <summary>
        /// Gets or sets the offeredbypartyid, refering to the party id of the user or organization offering the delegation.
        /// </summary>
        [JsonPropertyName("offeredbypartyid")]
        public int OfferedByPartyId { get; set; }

        /// <summary>
        /// Gets or sets the coveredbypartyid, refering to the party id of the organization having received the delegation. Otherwise Null if the recipient is a user.
        /// </summary>
        [JsonPropertyName("coveredbypartyid")]
        public int? CoveredByPartyId { get; set; }

        /// <summary>
        /// Gets or sets the coveredbyuserid, refering to the user id of the user having received the delegation. Otherwise Null if the recipient is an organization.
        /// </summary>
        [JsonPropertyName("coveredbyuserid")]
        public int? CoveredByUserId { get; set; }

        /// <summary>
        /// Gets or sets the user id of the user that performed the delegation change (either added or removed rules to the policy, or deleted it entirely).
        /// </summary>
        [JsonPropertyName("performedbyuserid")]
        public int PerformedByUserId { get; set; }

        /// <summary>
        /// Gets or sets blobstoragepolicypath.
        /// </summary>
        [JsonPropertyName("blobstoragepolicypath")]
        public string BlobStoragePolicyPath { get; set; }

        /// <summary>
        /// Gets or sets the blobstorage versionid
        /// </summary>
        [JsonPropertyName("blobstorageversionid")]
        public string BlobStorageVersionId { get; set; }

        /// <summary>
        /// Gets or sets the created date and timestamp for the delegation change
        /// </summary>
        [JsonPropertyName("created")]
        public DateTime Created { get; set; }
    }
}
