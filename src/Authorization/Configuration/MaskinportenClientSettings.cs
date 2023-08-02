using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Altinn.ApiClients.Maskinporten.Config;
using Altinn.ApiClients.Maskinporten.Interfaces;
using Altinn.ApiClients.Maskinporten.Models;

namespace Altinn.Platform.Authorization.Configuration
{
    /// <summary>
    /// General configuration settings
    /// </summary>
    public class MaskinportenClientSettings : IClientDefinition
    {
        /// <summary>
        /// Gets or sets the environment value
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// Gets or sets the ClientId
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the Scope
        /// </summary>
        public string Scope { get; set; }

        /// <inheritdoc/>
        public IMaskinportenSettings ClientSettings { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public Task<ClientSecrets> GetClientSecrets()
        {
            throw new NotImplementedException();
        }
    }
}
