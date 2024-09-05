using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Altinn.Authorization.ABAC.Utils;
using Altinn.Authorization.ABAC.Xacml;
using Altinn.Authorization.Models.Register;
using Altinn.Authorization.Models.ResourceRegistry;
using Altinn.Platform.Authorization.Services.Interface;

namespace Altinn.Platform.Authorization.IntegrationTests.MockServices;

public class ResourceRegistryMock : IResourceRegistry
{
    private readonly JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    public Task<ServiceResource> GetResourceAsync(string resourceId, CancellationToken cancellationToken = default)
    {
        string unitTestFolder = Path.GetDirectoryName(new Uri(typeof(AltinnApps_DecisionTests).Assembly.Location).LocalPath);
        string resourceListPath = Path.Combine(unitTestFolder, "Data", "Json", "ResourceList", "ResourceList.json");
        if (File.Exists(resourceListPath))
        {
            string content = File.ReadAllText(resourceListPath);
            List<ServiceResource> resourceList = JsonSerializer.Deserialize<List<ServiceResource>>(content, options);
            return Task.FromResult(resourceList.Find(r => r.Identifier.Equals(resourceId)));
        }

        return null;
    }

    public async Task<XacmlPolicy> GetResourcePolicyAsync(string resourceId, CancellationToken cancellationToken = default)
    {
        if (File.Exists(Path.Combine(GetResourceRegistryPolicyPath(resourceId), "policy.xml")))
        {
            return await Task.FromResult(ParsePolicy("policy.xml", GetResourceRegistryPolicyPath(resourceId)));
        }
       
        return null;
    }

    public Task<IEnumerable<AccessListResourceMembershipWithActionFilterDto>> GetMembershipsForResourceForParty(PartyUrn partyUrn, ResourceIdUrn resourceIdUrn, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private static string GetResourceRegistryPolicyPath(string resourceId)
    {
        string unitTestFolder = Path.GetDirectoryName(new Uri(typeof(AltinnApps_DecisionTests).Assembly.Location).LocalPath);
        return Path.Combine(unitTestFolder, "..", "..", "..", "Data", "Xacml", "3.0", "ResourceRegistry", resourceId);
    }
    
    public static XacmlPolicy ParsePolicy(string policyDocumentTitle, string policyPath)
    {
        XmlDocument policyDocument = new XmlDocument();

        policyDocument.Load(Path.Combine(policyPath, policyDocumentTitle));
        XacmlPolicy policy;
        using (XmlReader reader = XmlReader.Create(new StringReader(policyDocument.OuterXml)))
        {
            policy = XacmlParser.ParseXacmlPolicy(reader);
        }

        return policy;
    }
}
