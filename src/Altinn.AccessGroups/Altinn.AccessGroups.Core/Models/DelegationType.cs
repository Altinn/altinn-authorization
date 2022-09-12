namespace Altinn.AccessGroups.Core.Models
{
    public enum DelegationType
    {
        /// <summary>
        /// Delegated by user
        /// </summary>
        brukerdelegering = 0,

        /// <summary>
        /// Delegated by a client
        /// </summary>
        klientdelegering = 1,

        /// <summary>
        /// Delegated by a service
        /// </summary>
        tjenestedelegering = 2
    }
}
