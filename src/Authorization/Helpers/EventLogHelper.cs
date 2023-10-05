using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Altinn.Authorization.ABAC.Constants;
using Altinn.Authorization.ABAC.Interface;
using Altinn.Authorization.ABAC.Xacml;
using Altinn.Platform.Authorization.Configuration;
using Altinn.Platform.Authorization.Constants;
using Altinn.Platform.Authorization.Models;
using Altinn.Platform.Authorization.Models.EventLog;
using Altinn.Platform.Authorization.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents.SystemFunctions;
using Microsoft.FeatureManagement;

namespace Altinn.Platform.Authorization.Helpers
{
    /// <summary>
    /// Helper class for event logging
    /// </summary>
    public static class EventLogHelper
    {
        /// <summary>
        /// Maps the user, resource information from
        /// </summary>
        /// <param name="contextRequest">the context request</param>
        /// <returns></returns>
        public static AuthorizationEvent MapAuthorizationEventFromContextRequest(XacmlContextRequest contextRequest, HttpContext context, XacmlContextResponse contextRespsonse, DateTime currentDateTime)
        {
            AuthorizationEvent authorizationEvent = null;
            if (contextRequest != null)
            {
                authorizationEvent = new AuthorizationEvent();
                (string resource, string instanceId, int? resourcePartyId) = GetResourceAttributes(contextRequest);
                (int? userId, int? partyId, string org, int? orgNumber) = GetSubjectInformation(contextRequest);
                authorizationEvent.Created = currentDateTime;
                authorizationEvent.Resource = resource;
                authorizationEvent.SubjectUserId = userId;
                authorizationEvent.SubjectOrgCode = org;
                authorizationEvent.SubjectOrgNumber = orgNumber;
                authorizationEvent.InstanceId = instanceId;
                authorizationEvent.SubjectParty = partyId;
                authorizationEvent.ResourcePartyId = resourcePartyId;
                authorizationEvent.Operation = GetActionInformation(contextRequest);
                authorizationEvent.TimeToDelete = authorizationEvent.Created.AddYears(3);
                authorizationEvent.IpAdress = GetClientIpAddress(context);
                authorizationEvent.ContextRequestJson = JsonSerializer.Serialize(contextRequest);
                authorizationEvent.Decision = contextRespsonse.Results?.FirstOrDefault()?.Decision.ToString();
            }

            return authorizationEvent;
        }

        /// <summary>
        /// Returens the policy resource type based on XacmlContextRequest
        /// </summary>
        /// <param name="request">The requestId</param>
        /// <returns></returns>
        public static (string resource, string instanceId, int? resourcePartyId) GetResourceAttributes(XacmlContextRequest request)
        {
            string resource = string.Empty;
            string instanceId = string.Empty;
            int? resourcePartyId = null;
            string org = string.Empty;
            string app = string.Empty;

            foreach (XacmlContextAttributes attr in request.Attributes.Where(attr => attr.Category.OriginalString.Equals(XacmlConstants.MatchAttributeCategory.Resource)))
            {
                foreach (XacmlAttribute xacmlAtr in attr.Attributes)
                {
                    if (xacmlAtr.AttributeId.OriginalString.Equals(AltinnXacmlConstants.MatchAttributeIdentifiers.ResourceRegistry))
                    {
                        resource = xacmlAtr.AttributeValues.First().Value;
                    }
                    
                    if (xacmlAtr.AttributeId.OriginalString.Equals(AltinnXacmlConstants.MatchAttributeIdentifiers.AppAttribute))
                    {
                        app = xacmlAtr.AttributeValues.First().Value;
                    }

                    if (xacmlAtr.AttributeId.OriginalString.Equals(AltinnXacmlConstants.MatchAttributeIdentifiers.OrgAttribute))
                    {
                        org = xacmlAtr.AttributeValues.First().Value;
                    }

                    if (xacmlAtr.AttributeId.OriginalString.Equals(AltinnXacmlConstants.MatchAttributeIdentifiers.InstanceAttribute))
                    {
                        instanceId = xacmlAtr.AttributeValues.First().Value;
                    }

                    if (xacmlAtr.AttributeId.OriginalString.Equals(AltinnXacmlConstants.MatchAttributeIdentifiers.PartyAttribute))
                    {
                        resourcePartyId = Convert.ToInt32(xacmlAtr.AttributeValues.First().Value);
                    }
                }
            }

            resource = string.IsNullOrEmpty(resource) ? $"app_{org}_{app}" : resource;
            return (resource, instanceId, resourcePartyId);
        }

        /// <summary>
        /// Returens the policy resource type based on XacmlContextRequest
        /// </summary>
        /// <param name="request">The requestId</param>
        /// <returns></returns>
        public static (int? userId, int? partyId, string org, int? orgNumber) GetSubjectInformation(XacmlContextRequest request)
        {
            int? userId = null;
            int? partyId = null;
            string org = string.Empty;
            int? orgNumber = null;

            foreach (XacmlContextAttributes attr in request.Attributes.Where(attr => attr.Category.OriginalString.Equals(XacmlConstants.MatchAttributeCategory.Subject)))
            {
                foreach (XacmlAttribute xacmlAtr in attr.Attributes)
                {
                    if (xacmlAtr.AttributeId.OriginalString.Equals(AltinnXacmlConstants.MatchAttributeIdentifiers.UserAttribute))
                    {
                        userId = Convert.ToInt32(xacmlAtr.AttributeValues.First().Value);
                    }

                    if (xacmlAtr.AttributeId.OriginalString.Equals(AltinnXacmlConstants.MatchAttributeIdentifiers.PartyAttribute))
                    {
                        partyId = Convert.ToInt32(xacmlAtr.AttributeValues.First().Value);
                    }

                    if (xacmlAtr.AttributeId.OriginalString.Equals(AltinnXacmlConstants.MatchAttributeIdentifiers.OrgAttribute))
                    {
                        org = xacmlAtr.AttributeValues.First().Value;
                    }

                    if (xacmlAtr.AttributeId.OriginalString.Equals(AltinnXacmlConstants.MatchAttributeIdentifiers.OrgNumberAttribute))
                    {
                        orgNumber = Convert.ToInt32(xacmlAtr.AttributeValues.First().Value);
                    }
                }
            }

            return (userId, partyId, org, orgNumber);
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

        /// <summary>
        /// Get the client ip address
        /// </summary>
        /// <param name="context">the http request context</param>
        /// <returns></returns>
        public static string GetClientIpAddress(HttpContext context)
        {
            // Try to get the client IP address from the X-Real-IP header
            var clientIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();

            // If the X-Real-IP header is not present, fall back to the RemoteIpAddress property
            if (string.IsNullOrEmpty(clientIp))
            {
                clientIp = context.Connection.RemoteIpAddress?.ToString();
            }

            return clientIp;
        }
    }
}
