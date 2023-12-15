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
            CreateMap<XacmlJsonRequest, XacmlJsonRequestExternal>();
        }
    }
}
