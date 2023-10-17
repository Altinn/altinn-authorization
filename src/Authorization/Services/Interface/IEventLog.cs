﻿using System.Threading.Tasks;
using Altinn.Authorization.ABAC.Xacml;
using Altinn.Platform.Authorization.Models.EventLog;
using Microsoft.AspNetCore.Http;
using Microsoft.FeatureManagement;

namespace Altinn.Platform.Authorization.Services.Interfaces
{
    /// <summary>
    /// Defines event log interface to queue an authentication event to a storage queue
    /// </summary>
    public interface IEventLog
    {
        /// <summary>
        /// Creates an authorization event in storage queue
        /// </summary>
        /// <param name="authorizationEvent">authorization event</param>
        public void CreateAuthorizationEvent(AuthorizationEvent authorizationEvent);

        /// <summary>
        /// Creates an authorization event in storage queue
        /// </summary>
        /// <param name="featureManager">the handler to manage feature management</param>
        /// <param name="contextRequest">the enriched context request</param>
        /// <param name="context">the http context</param>
        /// <param name="contextResponse">the decision after the request process</param>
        public Task CreateAuthorizationEvent(IFeatureManager featureManager, XacmlContextRequest contextRequest, HttpContext context, XacmlContextResponse contextResponse);
    }
}
