using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Altinn.Authorization.ABAC.Interface;
using Altinn.Authorization.ABAC.Xacml;
using Altinn.Platform.Authorization.Services.Interface;

namespace Altinn.Platform.Authorization.Services.Implementation;

/// <inheritdoc />
public class PolicyContextHandler : IPolicyContextHandler
{
    private readonly ConcurrentDictionary<string, IContextHandler> _contextHandlers = new();

    /// <inheritdoc />
    public async Task<XacmlContextRequest> Enrich(XacmlContextRequest request, XacmlPolicy policy)
    {
        //// TODO! This could probably be done in parallell for each enricher, if
        //// alterations made to XacmlContextRequest can be made thread safe
        foreach (Uri subjectAttributeDesignatorId in GetSubjectAttributeDesignatorIds(policy))
        {
            IContextHandler contextHandler = GetSubjectEnricherForAttributeDesignatorId(subjectAttributeDesignatorId);
            request = await contextHandler.Enrich(request);
        }

        //// TODO! Dynamic resource enrichments too?
        return request;
    }

    private IContextHandler GetSubjectEnricherForAttributeDesignatorId(Uri attributeDesignatorId)
    {
        return attributeDesignatorId.OriginalString switch
        {
            "urn:oed:rolecode" => CreateContextHandler<OedSubjectContextHandler>(attributeDesignatorId.OriginalString),
            _ => CreateContextHandler<NullContextHandler>(attributeDesignatorId.OriginalString)
        };
    }

    private IContextHandler CreateContextHandler<TContextHandler>(string kind)
        where TContextHandler : IContextHandler, new()
    {
        return _contextHandlers.GetOrAdd(kind, (_) => new TContextHandler());
    }

    private ICollection<Uri> GetSubjectAttributeDesignatorIds(XacmlPolicy policy)
    {
        //// TODO! Check if this can benefit from caching
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