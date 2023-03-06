using System;
using System.Linq;
using System.Threading.Tasks;
using Altinn.Authorization.ABAC.Interface;
using Altinn.Authorization.ABAC.Xacml;
using Altinn.Platform.Authorization.Constants;

namespace Altinn.Platform.Authorization.Services.Implementation;

/// <summary>
/// Context handler for OED ("Digitalt dødsbo") roles. Utilizes OED PIP API
/// </summary>
public class OedSubjectContextHandler : IContextHandler
{
    /// <inheritdoc />
    public async Task<XacmlContextRequest> Enrich(XacmlContextRequest decisionRequest)
    {
        //// TODO! Consider expand XacmlRequestAttribute with "urn:altinn:ssn" or "urn:altinn:nin"

        XacmlContextAttributes subjectContextAttributes = decisionRequest.GetSubjectAttributes();
        XacmlAttribute subjectAttribute = subjectContextAttributes.Attributes.FirstOrDefault(a => a.AttributeId.OriginalString.Equals(XacmlRequestAttribute.UserAttribute));
        int userId = Convert.ToInt32(subjectAttribute?.AttributeValues.FirstOrDefault()?.Value);

        XacmlContextAttributes resourceAttributes = decisionRequest.GetResourceAttributes();
        XacmlAttribute resourceAttribute = resourceAttributes.Attributes.FirstOrDefault(a => a.AttributeId.OriginalString.Equals(XacmlRequestAttribute.PartyAttribute));
        int reporteePartyId = Convert.ToInt32(resourceAttribute?.AttributeValues.FirstOrDefault()?.Value);

        //// TODO! Lookup and cache  SSNs from userId, reporteeId, call PIP-API for OED

        return await Task.FromResult(decisionRequest);
    }
}