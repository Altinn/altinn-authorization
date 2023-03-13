namespace Altinn.Platform.Authorization.Constants
{
    /// <summary>
    /// Constants related to authorization.
    /// </summary>
    public static class AuthzConstants
    {
        /// <summary>
        /// Policy tag for authorizing designer access
        /// </summary>
        public const string POLICY_STUDIO_DESIGNER = "StudioDesignerAccess";

        /// <summary>
        /// Policy tag for authorizing Altinn.Platform.Authorization API access from AltinnII Authorization
        /// </summary>
        public const string ALTINNII_AUTHORIZATION = "AltinnIIAuthorizationAccess";

        /// <summary>
        /// Policy tag for authorizing Altinn.Platform.Authorization API access from the DelegationEvent Azure function
        /// </summary>
        public const string DELEGATIONEVENT_FUNCTION_AUTHORIZATION = "DelegationEventFunctionAccess";
    }
}
