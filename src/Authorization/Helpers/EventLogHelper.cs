using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Altinn.Authorization.ABAC.Constants;
using Altinn.Authorization.ABAC.Interface;
using Altinn.Authorization.ABAC.Xacml;
using Altinn.Platform.Authorization.Configuration;
using Altinn.Platform.Authorization.Constants;
using Altinn.Platform.Authorization.Models;
using Altinn.Platform.Authorization.Services.Interfaces;
using Microsoft.FeatureManagement;

namespace Altinn.Platform.Authorization.Helpers
{
    /// <summary>
    /// Helper class for event logging
    /// </summary>
    public static class EventLogHelper
    {
        /// <summary>
        /// Creates an authorization event
        /// </summary>
        /// <param name="featureManager">handler for feature manager service</param>
        /// <param name="eventLog">handler for eventlog service</param>
        public async static Task CreateAuthorizationEvent(IFeatureManager featureManager, IEventLog eventLog, XacmlContextRequest contextRequest)
        {
            if (await featureManager.IsEnabledAsync(FeatureFlags.AuditLog))
            {
                AuthorizationEvent authorizationEvent = MapAuthorizationEventFromContextRequest(contextRequest);
                eventLog.CreateAuthorizationEvent(authorizationEvent);
            }
        }

        /// <summary>
        /// Maps the user, resource information from
        /// </summary>
        /// <param name="contextRequest">the context request</param>
        /// <returns></returns>
        public static AuthorizationEvent MapAuthorizationEventFromContextRequest(XacmlContextRequest contextRequest)
        {
            AuthorizationEvent authorizationEvent = null;
            if (contextRequest != null)
            {
                authorizationEvent = new AuthorizationEvent();
                (string resourceId, string instanceId) = GetResourceAttributes(contextRequest);
                (string userId, string partyId) = GetSubjectInformation(contextRequest);
                authorizationEvent.Resource = resourceId;
                authorizationEvent.SubjectUserId = userId;
                authorizationEvent.InstanceId = instanceId;
                authorizationEvent.SubjectParty = partyId;
                authorizationEvent.Operation = GetActionInformation(contextRequest);
            }

            return authorizationEvent;
        }

        /// <summary>
        /// Returens the policy resource type based on XacmlContextRequest
        /// </summary>
        /// <param name="request">The requestId</param>
        /// <returns></returns>
        public static (string resourceId, string instanceId) GetResourceAttributes(XacmlContextRequest request)
        {
            string resourceid = string.Empty;
            string instanceId = string.Empty;

            foreach (XacmlContextAttributes attr in request.Attributes.Where(attr => attr.Category.OriginalString.Equals(XacmlConstants.MatchAttributeCategory.Resource)))
            {
                foreach (XacmlAttribute xacmlAtr in attr.Attributes)
                {
                    if (xacmlAtr.AttributeId.OriginalString.Equals(AltinnXacmlConstants.MatchAttributeIdentifiers.ResourceRegistry))
                    {
                        resourceid = xacmlAtr.AttributeValues.First().Value;
                    }

                    if (xacmlAtr.AttributeId.OriginalString.Equals(AltinnXacmlConstants.MatchAttributeIdentifiers.ResourceRegistry))
                    {
                        instanceId = xacmlAtr.AttributeValues.First().Value;
                    }
                }
            }

            return (resourceid, instanceId);
        }

        /// <summary>
        /// Returens the policy resource type based on XacmlContextRequest
        /// </summary>
        /// <param name="request">The requestId</param>
        /// <returns></returns>
        public static (string userId, string partyId) GetSubjectInformation(XacmlContextRequest request)
        {
            string userId = string.Empty;
            string partyId = string.Empty;

            foreach (XacmlContextAttributes attr in request.Attributes.Where(attr => attr.Category.OriginalString.Equals(XacmlConstants.MatchAttributeCategory.Subject)))
            {
                foreach (XacmlAttribute xacmlAtr in attr.Attributes)
                {
                    if (xacmlAtr.AttributeId.OriginalString.Equals(AltinnXacmlConstants.MatchAttributeIdentifiers.UserAttribute))
                    {
                        userId = xacmlAtr.AttributeValues.First().Value;
                    }

                    if (xacmlAtr.AttributeId.OriginalString.Equals(AltinnXacmlConstants.MatchAttributeIdentifiers.PartyAttribute))
                    {
                        partyId = xacmlAtr.AttributeValues.First().Value;
                    }
                }
            }

            return (userId, partyId);
        }

        /// <summary>
        /// Returens the policy resource type based on XacmlContextRequest
        /// </summary>
        /// <param name="request">The requestId</param>
        /// <returns></returns>
        public static string GetActionInformation(XacmlContextRequest request)
        {
            string actionId = string.Empty;

            foreach (XacmlContextAttributes attr in request.Attributes.Where(attr => attr.Category.OriginalString.Equals(XacmlConstants.MatchAttributeCategory.Action)))
            {
                foreach (XacmlAttribute xacmlAtr in attr.Attributes)
                {
                    if (xacmlAtr.AttributeId.OriginalString.Equals(XacmlConstants.MatchAttributeIdentifiers.ActionId))
                    {
                        actionId = xacmlAtr.AttributeValues.First().Value;
                    }
                }
            }

            return actionId;
        }

        private static Dictionary<string, ICollection<XacmlAttribute>> GetCategoryAttributes(XacmlContextRequest request, string category)
        {
            Dictionary<string, ICollection<XacmlAttribute>> categoryAttributes = new Dictionary<string, ICollection<XacmlAttribute>>();
            foreach (XacmlContextAttributes attributes in request.Attributes)
            {
                if (attributes.Category.OriginalString.Equals(category))
                {
                    foreach (XacmlAttribute attribute in attributes.Attributes)
                    {
                        if (categoryAttributes.Keys.Contains(attribute.AttributeId.OriginalString))
                        {
                            categoryAttributes[attribute.AttributeId.OriginalString].Add(attribute);
                        }
                        else
                        {
                            ICollection<XacmlAttribute> newCollection = new Collection<XacmlAttribute>();
                            newCollection.Add(attribute);

                            categoryAttributes.Add(attribute.AttributeId.OriginalString, newCollection);
                        }
                    }
                }
            }

            return categoryAttributes;
        }
    }
}
