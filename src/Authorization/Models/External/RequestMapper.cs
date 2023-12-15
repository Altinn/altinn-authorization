using Altinn.Authorization.ABAC.Xacml.JsonProfile;

namespace Altinn.Platform.Authorization.Models.External
{
    /// <summary>
    /// A class that hold access managment mapping
    /// </summary>
    public class RequestMapper : AutoMapper.Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMapper"/> class.
        /// </summary>
        public RequestMapper()
        {
            AllowNullCollections = true;
            CreateMap<XacmlJsonAttributeAssignmentExternal, XacmlJsonAttributeAssignment>();
            CreateMap<XacmlJsonAttributeExternal,XacmlJsonAttribute>();
            CreateMap<XacmlJsonCategoryExternal, XacmlJsonCategory>();
            CreateMap<XacmlJsonIdReferenceExternal, XacmlJsonIdReference>();
            CreateMap<XacmlJsonMissingAttributeDetailExternal, XacmlJsonMissingAttributeDetail>();
            CreateMap<XacmlJsonMultiRequestsExternal, XacmlJsonMultiRequests>();
            CreateMap<XacmlJsonObligationOrAdviceExternal, XacmlJsonObligationOrAdvice>();
            CreateMap<XacmlJsonPolicyIdentifierListExternal, XacmlJsonPolicyIdentifierList>();
            CreateMap<XacmlJsonRequestExternal, XacmlJsonRequest>();
            CreateMap<XacmlJsonRequestReferenceExternal, XacmlJsonRequestReference>();
            CreateMap<XacmlJsonRequestRootExternal, XacmlJsonRequestRoot>();
            CreateMap<XacmlJsonRequestExternal, XacmlJsonRequest>();
            CreateMap<XacmlJsonResponseExternal, XacmlJsonResponse>();
            CreateMap<XacmlJsonResultExternal, XacmlJsonResult>();
            CreateMap<XacmlJsonStatusCodeExternal, XacmlJsonStatusCode>();
            CreateMap<XacmlJsonStatusExternal, XacmlJsonStatus>();
        }
    }
}
