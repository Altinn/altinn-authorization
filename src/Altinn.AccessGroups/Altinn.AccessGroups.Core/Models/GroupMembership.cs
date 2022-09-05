namespace Altinn.AccessGroups.Core.Models
{
    public class GroupMembership
    {
        public int? CoveredByUserId { get; set; }

        public int? CoveredByPartyId { get; set; }

        public int OfferedByPartyId { get; set; }

        public int DelegationId { get; set; }

        //public GroupMembership(int? coveredByUserId, int? coveredByPartyId, int offeredByPartyId, string groupId)
        //{
        //    CoveredByUserId = coveredByUserId;
        //    CoveredByPartyId = coveredByPartyId;
        //    OfferedByPartyId = offeredByPartyId;
        //    GroupId = groupId;
        //}
    }
}
