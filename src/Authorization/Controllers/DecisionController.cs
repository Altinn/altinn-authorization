using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using Altinn.Authorization.ABAC;
using Altinn.Authorization.ABAC.Interface;
using Altinn.Authorization.ABAC.Utils;
using Altinn.Authorization.ABAC.Xacml;
using Altinn.Authorization.ABAC.Xacml.JsonProfile;
using Altinn.Platform.Authorization.Helpers;
using Altinn.Platform.Authorization.ModelBinding;
using Altinn.Platform.Authorization.Models;
using Altinn.Platform.Authorization.Repositories.Interface;
using Altinn.Platform.Authorization.Services.Interface;
using Altinn.Platform.Authorization.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Newtonsoft.Json;

namespace Altinn.Platform.Authorization.Controllers
{
    /// <summary>
    /// This is the controller responsible for Policy Enformcent Point endpoint.
    /// It returns a Xacml Context Reponse based on a Context Request
    /// </summary>
    [Route("authorization/api/v1/[controller]")]
    [ApiController]
    public class DecisionController : ControllerBase
    {
        private readonly PolicyDecisionPoint _pdp;
        private readonly IPolicyRetrievalPoint _prp;
        private readonly IContextHandler _contextHandler;
        private readonly IDelegationContextHandler _delegationContextHandler;
        private readonly IDelegationMetadataRepository _delegationRepository;
        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IEventLog _eventLog;
        private readonly IFeatureManager _featureManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="DecisionController"/> class.
        /// </summary>
        /// <param name="contextHandler">The Context handler</param>
        /// <param name="delegationContextHandler">The delegation context handler</param>
        /// <param name="policyRetrievalPoint">The policy Retrieval point</param>
        /// <param name="delegationRepository">The delegation repository</param>
        /// <param name="logger">the logger.</param>
        /// <param name="memoryCache">memory cache</param>
        /// <param name="eventLog">the authorization event logger</param>
        /// <param name="featureManager">the feature manager</param>
        public DecisionController(IContextHandler contextHandler, IDelegationContextHandler delegationContextHandler, IPolicyRetrievalPoint policyRetrievalPoint, IDelegationMetadataRepository delegationRepository, ILogger<DecisionController> logger, IMemoryCache memoryCache, IEventLog eventLog, IFeatureManager featureManager)
        {
            _pdp = new PolicyDecisionPoint();
            _prp = policyRetrievalPoint;
            _contextHandler = contextHandler;
            _delegationContextHandler = delegationContextHandler;
            _delegationRepository = delegationRepository;
            _logger = logger;
            _memoryCache = memoryCache;
            _eventLog = eventLog;
            _featureManager = featureManager;
        }

        /// <summary>
        /// Decision Point endpoint to authorize Xacml Context Requests
        /// </summary>
        /// <param name="model">A Generic model</param>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] XacmlRequestApiModel model)
        {
            try
            {
                if (Request.ContentType.Contains("application/json"))
                {
                    return await AuthorizeJsonRequest(model);
                }
                else
                {
                    return await AuthorizeXmlRequest(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "// DecisionController // Decision // Unexpected Exception");

                XacmlContextResult result = new XacmlContextResult(XacmlContextDecision.Indeterminate)
                {
                    Status = new XacmlContextStatus(XacmlContextStatusCode.SyntaxError)
                };

                XacmlContextResponse xacmlContextResponse = new XacmlContextResponse(result);

                if (Request.ContentType.Contains("application/json"))
                {
                    XacmlJsonResponse jsonResult = XacmlJsonXmlConverter.ConvertResponse(xacmlContextResponse);
                    return Ok(jsonResult);
                }
                else
                {
                    return CreateResponse(xacmlContextResponse);
                }
            }
        }

        private async Task<XacmlJsonResponse> Authorize(XacmlJsonRequest decisionRequest)
        {
            bool logEvent = true;
            if (decisionRequest.MultiRequests == null || decisionRequest.MultiRequests.RequestReference == null
                || decisionRequest.MultiRequests.RequestReference.Count < 2)
            {
                XacmlContextRequest request = XacmlJsonXmlConverter.ConvertRequest(decisionRequest);
                XacmlContextResponse xmlResponse = await Authorize(request, logEvent);
                return XacmlJsonXmlConverter.ConvertResponse(xmlResponse);
            }
            else
            {
                logEvent = false;
                XacmlJsonResponse multiResponse = new XacmlJsonResponse();
                foreach (XacmlJsonRequestReference xacmlJsonRequestReference in decisionRequest.MultiRequests.RequestReference)
                {
                    XacmlJsonRequest jsonMultiRequestPart = new XacmlJsonRequest();

                    foreach (string refer in xacmlJsonRequestReference.ReferenceId)
                    {
                        IEnumerable<XacmlJsonCategory> resourceCategoriesPart = decisionRequest.Resource.Where(i => i.Id.Equals(refer));

                        if (resourceCategoriesPart != null && resourceCategoriesPart.Count() > 0)
                        {
                            if (jsonMultiRequestPart.Resource == null)
                            {
                                jsonMultiRequestPart.Resource = new List<XacmlJsonCategory>();
                            }

                            jsonMultiRequestPart.Resource.AddRange(resourceCategoriesPart);
                        }

                        IEnumerable<XacmlJsonCategory> subjectCategoriesPart = decisionRequest.AccessSubject.Where(i => i.Id.Equals(refer));

                        if (subjectCategoriesPart != null && subjectCategoriesPart.Count() > 0)
                        {
                            if (jsonMultiRequestPart.AccessSubject == null)
                            {
                                jsonMultiRequestPart.AccessSubject = new List<XacmlJsonCategory>();
                            }

                            jsonMultiRequestPart.AccessSubject.AddRange(subjectCategoriesPart);
                        }

                        IEnumerable<XacmlJsonCategory> actionCategoriesPart = decisionRequest.Action.Where(i => i.Id.Equals(refer));

                        if (actionCategoriesPart != null && actionCategoriesPart.Count() > 0)
                        {
                            if (jsonMultiRequestPart.Action == null)
                            {
                                jsonMultiRequestPart.Action = new List<XacmlJsonCategory>();
                            }

                            jsonMultiRequestPart.Action.AddRange(actionCategoriesPart);
                        }
                    }

                    XacmlContextResponse partResponse = await Authorize(XacmlJsonXmlConverter.ConvertRequest(jsonMultiRequestPart), logEvent);
                    XacmlJsonResponse xacmlJsonResponsePart = XacmlJsonXmlConverter.ConvertResponse(partResponse);

                    if (multiResponse.Response == null)
                    {
                        multiResponse.Response = new List<XacmlJsonResult>();
                    }

                    multiResponse.Response.Add(xacmlJsonResponsePart.Response.First());
                }

                return multiResponse;
            }
        }

        private async Task<ActionResult> AuthorizeXmlRequest(XacmlRequestApiModel model)
        {
            XacmlContextRequest request;
            using (XmlReader reader = XmlReader.Create(new StringReader(model.BodyContent)))
            {
                request = XacmlParser.ReadContextRequest(reader);
            }

            XacmlContextResponse xacmlContextResponse = await Authorize(request, true);
            return CreateResponse(xacmlContextResponse);
        }

        private async Task<ActionResult> AuthorizeJsonRequest(XacmlRequestApiModel model)
        {
            XacmlJsonRequestRoot jsonRequest = (XacmlJsonRequestRoot)JsonConvert.DeserializeObject(model.BodyContent, typeof(XacmlJsonRequestRoot));

            XacmlJsonResponse jsonResponse = await Authorize(jsonRequest.Request);

            return Ok(jsonResponse);
        }

        private ActionResult CreateResponse(XacmlContextResponse xacmlContextResponse)
        {
            StringBuilder builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(builder))
            {
                XacmlSerializer.WriteContextResponse(writer, xacmlContextResponse);
            }

            string xml = builder.ToString();

            return Content(xml);
        }

        private async Task<XacmlContextResponse> Authorize(XacmlContextRequest decisionRequest, bool logEvent = true)
        {
            decisionRequest = await this._contextHandler.Enrich(decisionRequest);

            ////_logger.LogInformation($"// DecisionController // Authorize // Roles // Enriched request: {JsonConvert.SerializeObject(decisionRequest)}.");
            XacmlPolicy policy = await this._prp.GetPolicyAsync(decisionRequest);

            XacmlContextResponse rolesContextResponse = _pdp.Authorize(decisionRequest, policy);
            ////_logger.LogInformation($"// DecisionController // Authorize // Roles // XACML ContextResponse: {JsonConvert.SerializeObject(rolesContextResponse)}.");

            XacmlContextResult roleResult = rolesContextResponse.Results.First();
            if (roleResult.Decision.Equals(XacmlContextDecision.NotApplicable))
            {
                try
                {
                    XacmlContextResponse delegationContextResponse = await AuthorizeBasedOnDelegations(decisionRequest, policy);
                    XacmlContextResult delegationResult = delegationContextResponse.Results.First();
                    if (delegationResult.Decision.Equals(XacmlContextDecision.Permit))
                    {
                        if (logEvent)
                        {
                            _eventLog.CreateAuthorizationEvent(_featureManager, decisionRequest, HttpContext, delegationContextResponse);
                        }

                        return delegationContextResponse;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "// DecisionController // Authorize // Delegation // Unexpected Exception");
                }
            }

            if (logEvent)
            {
                _eventLog.CreateAuthorizationEvent(_featureManager, decisionRequest, HttpContext, rolesContextResponse);
            }
            
            return rolesContextResponse;
        }

        private async Task<XacmlContextResponse> AuthorizeBasedOnDelegations(XacmlContextRequest decisionRequest, XacmlPolicy appPolicy)
        {
            XacmlContextResponse delegationContextResponse = new XacmlContextResponse(new XacmlContextResult(XacmlContextDecision.NotApplicable)
            {
                Status = new XacmlContextStatus(XacmlContextStatusCode.Success)
            });

            XacmlResourceAttributes resourceAttributes = _delegationContextHandler.GetResourceAttributes(decisionRequest);
            int subjectUserId = _delegationContextHandler.GetSubjectUserId(decisionRequest);

            if (resourceAttributes == null ||
                string.IsNullOrEmpty(resourceAttributes.OrgValue) ||
                string.IsNullOrEmpty(resourceAttributes.AppValue) ||
                subjectUserId == 0 ||
                !int.TryParse(resourceAttributes.ResourcePartyValue, out int reporteePartyId))
            {
                // Not able to continue authorization based on delegations because of incomplete decision request
                string request = JsonConvert.SerializeObject(decisionRequest);
                _logger.LogWarning("// DecisionController // Authorize // Delegations // Incomplete request: {request}", request);
                return new XacmlContextResponse(new XacmlContextResult(XacmlContextDecision.Indeterminate)
                {
                    Status = new XacmlContextStatus(XacmlContextStatusCode.Success)
                });
            }

            List<string> appIds = new List<string> { $"{resourceAttributes.OrgValue}/{resourceAttributes.AppValue}" };
            List<int> offeredByPartyIds = new List<int> { reporteePartyId };
            List<int> coveredByUserIds = new List<int> { subjectUserId };

            // 1. Direct user delegations
            List<DelegationChange> delegations = await GetAllCachedDelegationChanges(offeredByPartyIds, appIds, coveredByUserIds: coveredByUserIds);
            if (delegations.Any())
            {
                delegationContextResponse = await AuthorizeBasedOnDelegations(decisionRequest, delegations, appPolicy);

                if (delegationContextResponse.Results.Any(r => r.Decision == XacmlContextDecision.Permit))
                {
                    return delegationContextResponse;
                }
            }

            // 2. Direct user delegations from mainunit
            List<MainUnit> mainunits = await _delegationContextHandler.GetMainUnits(reporteePartyId);
            List<int> mainunitPartyIds = mainunits.Where(m => m.PartyId.HasValue).Select(m => m.PartyId.Value).ToList();

            if (mainunitPartyIds.Any())
            {
                offeredByPartyIds.AddRange(mainunitPartyIds);
                delegations = await GetAllCachedDelegationChanges(mainunitPartyIds, appIds, coveredByUserIds: coveredByUserIds);

                if (delegations.Any())
                {
                    delegationContextResponse = await AuthorizeBasedOnDelegations(decisionRequest, delegations, appPolicy);

                    if (delegationContextResponse.Results.Any(r => r.Decision == XacmlContextDecision.Permit))
                    {
                        return delegationContextResponse;
                    }
                }                
            }

            // 3. Direct party delegations to keyrole units
            List<int> keyrolePartyIds = await _delegationContextHandler.GetKeyRolePartyIds(subjectUserId);
            if (keyrolePartyIds.Any())
            {
                delegations = await GetAllCachedDelegationChanges(offeredByPartyIds, appIds, coveredByPartyIds: keyrolePartyIds);

                if (delegations.Any())
                {
                    _delegationContextHandler.Enrich(decisionRequest, keyrolePartyIds);
                    delegationContextResponse = await AuthorizeBasedOnDelegations(decisionRequest, delegations, appPolicy);
                }
            }

            return delegationContextResponse;
        }

        private async Task<List<DelegationChange>> GetAllCachedDelegationChanges(List<int> offeredByPartyIds, List<string> altinnAppIds = null, List<int> coveredByPartyIds = null, List<int> coveredByUserIds = null)
        {
            List<DelegationChange> delegationChanges;
            string cacheKey = GetDlgChangeCacheKey(offeredByPartyIds, altinnAppIds, coveredByPartyIds, coveredByUserIds);
            if (!_memoryCache.TryGetValue(cacheKey, out delegationChanges))
            {
                // Key not in cache, so get data.
                delegationChanges = await _delegationRepository.GetAllCurrentDelegationChanges(offeredByPartyIds, altinnAppIds, coveredByPartyIds, coveredByUserIds);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
               .SetPriority(CacheItemPriority.High)
               .SetAbsoluteExpiration(new TimeSpan(0, 0, 5, 0));

                _memoryCache.Set(cacheKey, delegationChanges, cacheEntryOptions);
            }

            return delegationChanges;
        }

        private string GetDlgChangeCacheKey(List<int> offeredByPartyIds, List<string> altinnAppIds = null, List<int> coveredByPartyIds = null, List<int> coveredByUserIds = null)
        {
            string cacheKey = null;
            foreach (int id in offeredByPartyIds ?? Enumerable.Empty<int>())
            {
                cacheKey += $"o:{id};";
            }

            foreach (string id in altinnAppIds ?? Enumerable.Empty<string>())
            {
                cacheKey += $"a:{id};";
            }

            foreach (int id in coveredByPartyIds ?? Enumerable.Empty<int>())
            {
                cacheKey += $"p:{id};";
            }

            foreach (int id in coveredByUserIds ?? Enumerable.Empty<int>())
            {
                cacheKey += $"u:{id};";
            }

            return cacheKey;
        }

        private async Task<XacmlContextResponse> AuthorizeBasedOnDelegations(XacmlContextRequest decisionRequest, List<DelegationChange> delegations, XacmlPolicy appPolicy)
        {
            XacmlContextResponse delegationContextResponse = new XacmlContextResponse(new XacmlContextResult(XacmlContextDecision.NotApplicable)
            {
                Status = new XacmlContextStatus(XacmlContextStatusCode.Success)
            });

            foreach (DelegationChange delegation in delegations.Where(d => d.DelegationChangeType != DelegationChangeType.RevokeLast))
            {
                XacmlPolicy delegationPolicy = await _prp.GetPolicyVersionAsync(delegation.BlobStoragePolicyPath, delegation.BlobStorageVersionId);
                foreach (XacmlObligationExpression obligationExpression in appPolicy.ObligationExpressions)
                {
                    delegationPolicy.ObligationExpressions.Add(obligationExpression);
                }

                delegationContextResponse = _pdp.Authorize(decisionRequest, delegationPolicy);

                string response = JsonConvert.SerializeObject(delegationContextResponse);
                _logger.LogInformation("// DecisionController // Authorize // Delegations // XACML ContextResponse\n{response}", response);

                if (delegationContextResponse.Results.Any(r => r.Decision == XacmlContextDecision.Permit))
                {
                    return delegationContextResponse;
                }
            }

            return delegationContextResponse;
        }
    }
}
