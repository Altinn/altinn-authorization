using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Altinn.Authorization.ABAC.Constants;
using Altinn.Authorization.ABAC.Interface;
using Altinn.Authorization.ABAC.Xacml;
using Altinn.Platform.Authorization.Configuration;
using Altinn.Platform.Authorization.Constants;
using Altinn.Platform.Authorization.Helpers;
using Altinn.Platform.Authorization.Models;
using Altinn.Platform.Authorization.Repositories.Interface;
using Altinn.Platform.Authorization.Services.Interface;
using Altinn.Platform.Authorization.Services.Interfaces;
using Altinn.Platform.Register.Models;
using Altinn.Platform.Storage.Interface.Models;
using Authorization.Platform.Authorization.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Altinn.Platform.Authorization.Services.Implementation
{
    /// <summary>
    /// The context handler is responsible for updating a context request
    /// From XACML standard
    ///
    /// Context Handler
    /// The system entity that converts decision requests in the native request format to the XACML canonical form, coordinates with Policy
    /// Information Points to add attribute values to the request context, and converts authorization decisions in the XACML canonical form to
    /// the native response format
    /// </summary>
    public class ContextHandler : IContextHandler
    {
        private readonly IInstanceMetadataRepository _policyInformationRepository;
        private readonly IRoles _rolesWrapper;
        private readonly IParties _partiesWrapper;
        private readonly IMemoryCache _memoryCache;
        private readonly GeneralSettings _generalSettings;
        private readonly IRegisterService _registerService;
        private readonly IPolicyRetrievalPoint _prp;
        private readonly IOedRoleAssignmentWrapper _oedRoleAssignmentWrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextHandler"/> class
        /// </summary>
        /// <param name="policyInformationRepository">the policy information repository handler</param>
        /// <param name="rolesWrapper">the roles handler</param>
        /// <param name="partiesWrapper">the party information handler</param>
        /// <param name="memoryCache">The cache handler </param>
        /// <param name="settings">The app settings</param>
        /// <param name="registerService">Register service</param>
        /// <param name="policyRetrievalPoint">the Policy Retrieval Point</param>
        /// <param name="oedRoleAssignmentWrapper">the OED Role Assignment Wrapper</param>
        public ContextHandler(
            IInstanceMetadataRepository policyInformationRepository, IRoles rolesWrapper, IParties partiesWrapper, IMemoryCache memoryCache, IOptions<GeneralSettings> settings, IRegisterService registerService, IPolicyRetrievalPoint policyRetrievalPoint, IOedRoleAssignmentWrapper oedRoleAssignmentWrapper)
        {
            _policyInformationRepository = policyInformationRepository;
            _rolesWrapper = rolesWrapper;
            _partiesWrapper = partiesWrapper;
            _memoryCache = memoryCache;
            _generalSettings = settings.Value;
            _registerService = registerService;
            _prp = policyRetrievalPoint;
            _oedRoleAssignmentWrapper = oedRoleAssignmentWrapper;
        }

        /// <summary>
        /// Ads needed information to the Context Request.
        /// </summary>
        /// <param name="request">The original Xacml Context Request</param>
        /// <returns>The enriched XacmlContextRequest</returns>
        public async Task<XacmlContextRequest> Enrich(XacmlContextRequest request)
        {
            await EnrichResourceAttributes(request);
            return await Task.FromResult(request);
        }

        /// <summary>
        /// Enriches the resource attribute collection with additional attributes retrieved based on the instance on the request
        /// </summary>
        /// <param name="request">The original Xacml Context Request</param>
        protected async Task EnrichResourceAttributes(XacmlContextRequest request)
        {
            XacmlContextAttributes resourceContextAttributes = request.GetResourceAttributes();
            XacmlResourceAttributes resourceAttributes = GetResourceAttributeValues(resourceContextAttributes);
            await EnrichResourceParty(resourceAttributes);

            bool resourceAttributeComplete = IsResourceComplete(resourceAttributes);

            if (!resourceAttributeComplete && !string.IsNullOrEmpty(resourceAttributes.InstanceValue))
            {
                Instance instanceData = null;
                if (!_generalSettings.UseStorageApiForInstanceAuthInfo)
                {
                    instanceData = await _policyInformationRepository.GetInstance(resourceAttributes.InstanceValue);
                }
                else
                {
                    instanceData = new();
                    AuthInfo authInfo = await _policyInformationRepository.GetAuthInfo(resourceAttributes.InstanceValue);
                    instanceData.Process = authInfo.Process;
                    instanceData.AppId = authInfo.AppId;
                    instanceData.Org = instanceData.AppId.Split('/')[0];
                }

                if (instanceData != null)
                {
                    AddIfValueDoesNotExist(resourceContextAttributes, XacmlRequestAttribute.OrgAttribute, resourceAttributes.OrgValue, instanceData.Org);
                    string app = instanceData.AppId.Split("/")[1];
                    AddIfValueDoesNotExist(resourceContextAttributes, XacmlRequestAttribute.AppAttribute, resourceAttributes.AppValue, app);
                    if (instanceData.Process?.CurrentTask != null)
                    {
                        AddIfValueDoesNotExist(resourceContextAttributes, XacmlRequestAttribute.TaskAttribute, resourceAttributes.TaskValue, instanceData.Process.CurrentTask.ElementId);
                    }
                    else if (instanceData.Process?.EndEvent != null)
                    {
                        AddIfValueDoesNotExist(resourceContextAttributes, XacmlRequestAttribute.EndEventAttribute, null, instanceData.Process.EndEvent);
                    }

                    string partyId = resourceAttributes.InstanceValue.Split('/')[0];
                    AddIfValueDoesNotExist(resourceContextAttributes, XacmlRequestAttribute.PartyAttribute, resourceAttributes.ResourcePartyValue, partyId);
                    resourceAttributes.ResourcePartyValue = partyId;
                }
            }

            await EnrichSubjectAttributes(request, resourceAttributes.ResourcePartyValue, resourceAttributes.ResourceSsnValue);
        }

        private static bool IsResourceComplete(XacmlResourceAttributes resourceAttributes)
        {
            bool resourceAttributeComplete = false;
            if (!string.IsNullOrEmpty(resourceAttributes.OrgValue) &&
                !string.IsNullOrEmpty(resourceAttributes.AppValue) &&
                !string.IsNullOrEmpty(resourceAttributes.InstanceValue) &&
                !string.IsNullOrEmpty(resourceAttributes.ResourcePartyValue) &&
                !string.IsNullOrEmpty(resourceAttributes.TaskValue))
            {
                // The resource attributes are complete
                resourceAttributeComplete = true;
            }
            else if (!string.IsNullOrEmpty(resourceAttributes.OrgValue) &&
                !string.IsNullOrEmpty(resourceAttributes.AppValue) &&
                string.IsNullOrEmpty(resourceAttributes.InstanceValue) &&
                !string.IsNullOrEmpty(resourceAttributes.ResourcePartyValue) &&
                string.IsNullOrEmpty(resourceAttributes.TaskValue))
            {
                // The resource attributes are complete
                resourceAttributeComplete = true;
            }
            else if (!string.IsNullOrEmpty(resourceAttributes.OrgValue) &&
            !string.IsNullOrEmpty(resourceAttributes.AppValue) &&
            !string.IsNullOrEmpty(resourceAttributes.InstanceValue) &&
            !string.IsNullOrEmpty(resourceAttributes.ResourcePartyValue) &&
            !string.IsNullOrEmpty(resourceAttributes.AppResourceValue) &&
            resourceAttributes.AppResourceValue.Equals("events"))
            {
                // The resource attributes are complete
                resourceAttributeComplete = true;
            }
            else if (!string.IsNullOrEmpty(resourceAttributes.ResourceRegistryId) &&
           !string.IsNullOrEmpty(resourceAttributes.ResourcePartyValue))
            {
                // The resource attributes are complete
                resourceAttributeComplete = true;
            }

            return resourceAttributeComplete;
        }

        /// <summary>
        /// Method that adds information about the resource party 
        /// </summary>
        /// <returns></returns>
        protected async Task EnrichResourceParty(XacmlResourceAttributes resourceAttributes)
        {
            if (string.IsNullOrEmpty(resourceAttributes.ResourcePartyValue) && !string.IsNullOrEmpty(resourceAttributes.OrganizationNumber))
            {
                int partyId = await _registerService.PartyLookup(resourceAttributes.OrganizationNumber, null);
                if (partyId != 0)
                {
                    resourceAttributes.ResourcePartyValue = partyId.ToString();
                }
            }
        }

        /// <summary>
        /// Maps the XacmlContextAttributes for the Xacml Resource category to the Altinn XacmlResourceAttributes model
        /// </summary>
        /// <param name="resourceContextAttributes">XacmlContextAttributes for mapping of resource attribute values</param>
        /// <returns>XacmlResourceAttributes</returns>
        protected XacmlResourceAttributes GetResourceAttributeValues(XacmlContextAttributes resourceContextAttributes)
        {
            XacmlResourceAttributes resourceAttributes = new XacmlResourceAttributes();

            foreach (XacmlAttribute attribute in resourceContextAttributes.Attributes)
            {
                if (attribute.AttributeId.OriginalString.Equals(XacmlRequestAttribute.OrgAttribute))
                {
                    resourceAttributes.OrgValue = attribute.AttributeValues.First().Value;
                }

                if (attribute.AttributeId.OriginalString.Equals(XacmlRequestAttribute.AppAttribute))
                {
                    resourceAttributes.AppValue = attribute.AttributeValues.First().Value;
                }

                if (attribute.AttributeId.OriginalString.Equals(XacmlRequestAttribute.InstanceAttribute))
                {
                    resourceAttributes.InstanceValue = attribute.AttributeValues.First().Value;
                }

                if (attribute.AttributeId.OriginalString.Equals(XacmlRequestAttribute.PartyAttribute))
                {
                    resourceAttributes.ResourcePartyValue = attribute.AttributeValues.First().Value;
                }

                if (attribute.AttributeId.OriginalString.Equals(XacmlRequestAttribute.SsnAttribute))
                {
                    resourceAttributes.ResourceSsnValue = attribute.AttributeValues.First().Value;
                }

                if (attribute.AttributeId.OriginalString.Equals(XacmlRequestAttribute.TaskAttribute))
                {
                    resourceAttributes.TaskValue = attribute.AttributeValues.First().Value;
                }

                if (attribute.AttributeId.OriginalString.Equals(XacmlRequestAttribute.AppResourceAttribute))
                {
                    resourceAttributes.AppResourceValue = attribute.AttributeValues.First().Value;
                }

                if (attribute.AttributeId.OriginalString.Equals(XacmlRequestAttribute.ResourceRegistryAttribute))
                {
                    resourceAttributes.ResourceRegistryId = attribute.AttributeValues.First().Value;
                }

                if (attribute.AttributeId.OriginalString.Equals(XacmlRequestAttribute.OrganizationNumberAttribute))
                {
                    resourceAttributes.OrganizationNumber = attribute.AttributeValues.First().Value;
                }
            }

            return resourceAttributes;
        }

        /// <summary>
        /// Add a XacmlAttribute to the resourceAttributes collection, if the existing value is empty
        /// </summary>
        /// <param name="resourceAttributes">The collection of resource attribues</param>
        /// <param name="attributeId">The attribute id</param>
        /// <param name="attributeValue">The existing attribute value</param>
        /// <param name="newAttributeValue">The new attribute value</param>
        protected void AddIfValueDoesNotExist(XacmlContextAttributes resourceAttributes, string attributeId, string attributeValue, string newAttributeValue)
        {
            if (string.IsNullOrEmpty(attributeValue))
            {
                resourceAttributes.Attributes.Add(GetAttribute(attributeId, newAttributeValue));
            }
        }

        /// <summary>
        /// Gets a XacmlAttribute model for the specified attribute id and value
        /// </summary>
        /// <param name="attributeId">The attribute id</param>
        /// <param name="attributeValue">The attribute value</param>
        /// <returns>XacmlAttribute</returns>
        protected XacmlAttribute GetAttribute(string attributeId, string attributeValue)
        {
            XacmlAttribute attribute = new XacmlAttribute(new Uri(attributeId), false);
            if (attributeId.Equals(XacmlRequestAttribute.PartyAttribute))
            {
                // When Party attribute is missing from input it is good to return it so PEP can get this information
                attribute.IncludeInResult = true;
            }

            attribute.AttributeValues.Add(new XacmlAttributeValue(new Uri(XacmlConstants.DataTypes.XMLString), attributeValue));
            return attribute;
        }

        /// <summary>
        /// Enriches the XacmlContextRequest with the Roles the subject user has for the resource reportee
        /// </summary>
        /// <param name="request">The original Xacml Context Request</param>
        /// <param name="resourceParty">The resource reportee party id</param>
        /// <param name="resourceSsn">The resource reportee's ssn</param>
        protected async Task EnrichSubjectAttributes(XacmlContextRequest request, string resourceParty, string resourceSsn)
        {
            // If there is no resource party then it is impossible to enrich roles
            if (string.IsNullOrEmpty(resourceParty) && string.IsNullOrEmpty(resourceSsn))
            {
                return;
            }

            XacmlPolicy xacmlPolicy = await _prp.GetPolicyAsync(request);
            List<string> policySubjectAttributes = PolicyHelper.GetSubjectAttributeIds(xacmlPolicy);

            XacmlContextAttributes requestSubjectContextAttributes = request.GetSubjectAttributes();

            int subjectUserId = 0;
            string subjectSsn = string.Empty;
            int resourcePartyId = string.IsNullOrEmpty(resourceParty) ? 0 : Convert.ToInt32(resourceParty);

            foreach (XacmlAttribute xacmlAttribute in requestSubjectContextAttributes.Attributes)
            {
                if (xacmlAttribute.AttributeId.OriginalString.Equals(XacmlRequestAttribute.UserAttribute))
                {
                    subjectUserId = Convert.ToInt32(xacmlAttribute.AttributeValues.First().Value);
                }

                if (xacmlAttribute.AttributeId.OriginalString.Equals(XacmlRequestAttribute.SsnAttribute))
                {
                    subjectSsn = xacmlAttribute.AttributeValues.First().Value;
                }
            }

            if (policySubjectAttributes.Contains(AltinnXacmlConstants.MatchAttributeIdentifiers.OEDRoleAttribute))
            {
                if (subjectSsn == string.Empty)
                {
                    // slå opp ssn hvis den ikke finnes allerede
                }

                List<OedRoleAssignment> oedRoleAssignments = await GetOedRoleAssignments(resourceSsn, subjectSsn);
                requestSubjectContextAttributes.Attributes.Add(GetAccessGroupsAttribute(oedRoleAssignments));
            }

            // if (subjectUserId == 0)
            // {
            //    return;
            // }

            if (resourcePartyId != 0)
            {
                List<Role> roleList = await GetRoles(subjectUserId, resourcePartyId);
                requestSubjectContextAttributes.Attributes.Add(GetRoleAttribute(roleList));
            }
        }

        /// <summary>
        /// Gets a XacmlAttribute model for the list of access groups membership
        /// </summary>
        /// <param name="oedRoleAssignments">The list of oedRoleAssignments</param>
        /// <returns>XacmlAttribute</returns>
        protected XacmlAttribute GetAccessGroupsAttribute(List<OedRoleAssignment> oedRoleAssignments)
        {
            XacmlAttribute attribute = new XacmlAttribute(new Uri(XacmlRequestAttribute.OedRoleAttribute), false);
            foreach (OedRoleAssignment oedRoleAssignment in oedRoleAssignments)
            {
                attribute.AttributeValues.Add(new XacmlAttributeValue(new Uri(XacmlConstants.DataTypes.XMLString), oedRoleAssignment.OedRoleCode));
            }

            return attribute;
        }

        /// <summary>
        /// Gets a XacmlAttribute model for the list of roletype codes
        /// </summary>
        /// <param name="roles">The list of roletype codes</param>
        /// <returns>XacmlAttribute</returns>
        protected XacmlAttribute GetRoleAttribute(List<Role> roles)
        {
            XacmlAttribute attribute = new XacmlAttribute(new Uri(XacmlRequestAttribute.RoleAttribute), false);
            foreach (Role role in roles)
            {
                attribute.AttributeValues.Add(new XacmlAttributeValue(new Uri(XacmlConstants.DataTypes.XMLString), role.Value));
            }

            return attribute;
        }

        /// <summary>
        /// Gets a XacmlAttribute model for a list of party ids
        /// </summary>
        /// <param name="partyIds">The list of party ids</param>
        /// <returns>XacmlAttribute</returns>
        protected XacmlAttribute GetPartyIdsAttribute(List<int> partyIds)
        {
            XacmlAttribute attribute = new XacmlAttribute(new Uri(XacmlRequestAttribute.PartyAttribute), false);
            foreach (int partyId in partyIds)
            {
                attribute.AttributeValues.Add(new XacmlAttributeValue(new Uri(XacmlConstants.DataTypes.XMLString), partyId.ToString()));
            }

            return attribute;
        }

        /// <summary>
        /// Gets the list of roletype codes the subject user has for the resource reportee
        /// </summary>
        /// <param name="subjectUserId">The user id of the subject</param>
        /// <param name="resourcePartyId">The party id of the reportee</param>
        /// <returns>List of roles</returns>
        protected async Task<List<Role>> GetRoles(int subjectUserId, int resourcePartyId)
        {
            string cacheKey = GetCacheKey(subjectUserId, resourcePartyId);
           
            if (!_memoryCache.TryGetValue(cacheKey, out List<Role> roles))
            {
                // Key not in cache, so get data.
                roles = await _rolesWrapper.GetDecisionPointRolesForUser(subjectUserId, resourcePartyId) ?? new List<Role>();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
               .SetPriority(CacheItemPriority.High)
               .SetAbsoluteExpiration(new TimeSpan(0, _generalSettings.RoleCacheTimeout, 0));

                _memoryCache.Set(cacheKey, roles, cacheEntryOptions);
            }

            return roles;
        }

        /// <summary>
        /// Gets the list of mainunits for a subunit
        /// </summary>
        /// <param name="subUnitPartyId">The subunit partyId to check and retrieve mainunits for</param>
        /// <returns>List of mainunits</returns>
        protected async Task<List<MainUnit>> GetMainUnits(int subUnitPartyId)
        {
            string cacheKey = $"GetMainUnitsFor:{subUnitPartyId}";

            if (!_memoryCache.TryGetValue(cacheKey, out List<MainUnit> mainUnits))
            {
                // Key not in cache, so get data.
                mainUnits = await _partiesWrapper.GetMainUnits(new MainUnitQuery { PartyIds = new List<int> { subUnitPartyId } });

                var cacheEntryOptions = new MemoryCacheEntryOptions()
               .SetPriority(CacheItemPriority.High)
               .SetAbsoluteExpiration(new TimeSpan(0, _generalSettings.MainUnitCacheTimeout, 0));

                _memoryCache.Set(cacheKey, mainUnits, cacheEntryOptions);
            }

            return mainUnits;
        }

        /// <summary>
        /// Gets the list of keyrole unit partyIds for a user
        /// </summary>
        /// <param name="subjectUserId">The userid to retrieve keyrole unit for</param>
        /// <returns>List of partyIds for units where user has keyrole</returns>
        protected async Task<List<int>> GetKeyRolePartyIds(int subjectUserId)
        {
            string cacheKey = $"GetKeyRolePartyIdsFor:{subjectUserId}";

            if (!_memoryCache.TryGetValue(cacheKey, out List<int> keyrolePartyIds))
            {
                // Key not in cache, so get data.
                keyrolePartyIds = await _partiesWrapper.GetKeyRoleParties(subjectUserId);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
               .SetPriority(CacheItemPriority.High)
               .SetAbsoluteExpiration(new TimeSpan(0, _generalSettings.MainUnitCacheTimeout, 0));

                _memoryCache.Set(cacheKey, keyrolePartyIds, cacheEntryOptions);
            }

            return keyrolePartyIds;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="from">the deceased party</param>
        /// <param name="to">the not deceased party</param>
        /// <returns>list of OED Role Assignments</returns>
        protected async Task<List<OedRoleAssignment>> GetOedRoleAssignments(string from, string to)
        {
            string cacheKey = GetOedRoleassignmentCacheKey(from, to);

            if (!_memoryCache.TryGetValue(cacheKey, out List<OedRoleAssignment> oedRoles))
            {
                // Key not in cache, so get dat
                oedRoles = await _oedRoleAssignmentWrapper.GetOedRoleAssignments(from, to);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
               .SetPriority(CacheItemPriority.High)
               .SetAbsoluteExpiration(new TimeSpan(0, _generalSettings.RoleCacheTimeout, 0));

                _memoryCache.Set(cacheKey, oedRoles, cacheEntryOptions);
            }

            return oedRoles;
        }

        private string GetCacheKey(int userId, int partyId)
        {
            return "rolelist_" + userId + "_" + partyId;
        }

        private string GetOedRoleassignmentCacheKey(string from, string to)
        {
            return $"oed{from}_{to}";
        }
    }
}
