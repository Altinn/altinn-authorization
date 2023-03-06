using System.Threading.Tasks;
using Altinn.Authorization.ABAC.Interface;
using Altinn.Authorization.ABAC.Xacml;

namespace Altinn.Platform.Authorization.Services.Interface;

/// <summary>
/// This handles any additional dynamic enrichment that might be required based on subject attributes in the policy
/// </summary>
public interface IPolicyContextHandler
{
    /// <summary>
    /// Enriches the request with attributes dynamically based on attributes in the policy
    /// </summary>
    /// <param name="request">The request to be enriched</param>
    /// <param name="policy">The policy in question</param>
    /// <returns></returns>
    public Task<XacmlContextRequest> Enrich(XacmlContextRequest request, XacmlPolicy policy);
}