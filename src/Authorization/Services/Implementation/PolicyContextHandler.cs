using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Altinn.Authorization.ABAC.Interface;
using Altinn.Authorization.ABAC.Xacml;
using Altinn.Platform.Authorization.Extensions;
using Altinn.Platform.Authorization.Services.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Altinn.Platform.Authorization.Services.Implementation;

/// <inheritdoc />
public class PolicyContextHandler : IPolicyContextHandler
{
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="PolicyContextHandler"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to fetch the various subject enrichers</param>
    public PolicyContextHandler(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public async Task<XacmlContextRequest> Enrich(XacmlContextRequest request, XacmlPolicy policy)
    {
        // TODO! This should probably be done in parallell (if >1 enricher?), but
        // needs to operate on copies of the request that are merged at the end
        foreach (Uri subjectAttributeDesignatorId in GetSubjectAttributeDesignatorIds(policy))
        {
            IContextHandler contextHandler = GetSubjectEnricherForAttributeDesignatorId(subjectAttributeDesignatorId);
            request = await contextHandler.Enrich(request);
        }

        // TODO! Further dynamic resource enrichments too?
        return request;
    }

    /// <summary>
    /// This method returns a instance of a context handler for the given attribute designator id.
    /// </summary>
    /// <param name="attributeDesignatorId">The subject attribute designator id</param>
    /// <returns>A context handler able to enrich the request</returns>
    private IContextHandler GetSubjectEnricherForAttributeDesignatorId(Uri attributeDesignatorId)
    {
        return attributeDesignatorId.OriginalString switch
        {
            "urn:oed:rolecode" => serviceProvider.GetRequiredService<OedSubjectContextHandler>(),
            ////"urn:altinn:apiscope" => serviceProvider.GetRequiredService<ApiScopeSubjectContextHandler>(),
            ////"urn:advreg:role" => serviceProvider.GetRequiredService<AdvRegSubjectContextHandler>(),
            ////"urn:aareg:employee" => serviceProvider.GetRequiredService<AaRegSubjectContextHandler>(),

            //// Handle unknown attribute designator ids
            _ => new NullContextHandler()
        };
    }

    private ICollection<Uri> GetSubjectAttributeDesignatorIds(XacmlPolicy policy)
    {
        //// TODO! Consider caching this based on policy.PolicyId
        return (
            from rule in policy.Rules.TakeWhile(x => x.Target != null)
            from anyOf in rule.Target.AnyOf
            from allOf in anyOf.AllOf
            from match in allOf.Matches
            where match.AttributeDesignator.Category.OriginalString ==
                  "urn:oasis:names:tc:xacml:1.0:subject-category:access-subject"
            select match.AttributeDesignator.AttributeId).ToList();
    }
}

/// <inheritdoc />
internal class NullContextHandler : IContextHandler
{
    /// <inheritdoc />
    public Task<XacmlContextRequest> Enrich(XacmlContextRequest decisionRequest)
    {
        return Task.FromResult(decisionRequest);
    }
}
