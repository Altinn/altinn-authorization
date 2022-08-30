namespace Altinn.AccessGroups.Integrations
{
    /// <summary>
    /// Represents settings needed by the application for various purposes...
    /// </summary>
    public class SBLBridgeSettings
    {
        /// <summary>
        /// Gets or sets the bridge authorization api endpoint
        /// </summary>
        public string AuthorizationApiEndpoint { get; set; }

        /// <summary>
        /// Gets the bridge api endpoint from kubernetes environment variables and appsettings if environment variable is not set
        /// </summary>
        public string GetAuthorizationApiEndpoint
        {
            get
            {
                return Environment.GetEnvironmentVariable("SBLBridgeSettings__AuthorizationApiEndpoint") ?? AuthorizationApiEndpoint;
            }
        }
    }
}
