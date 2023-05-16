using System;
using System.Threading.Tasks;
using Altinn.Platform.Storage.Interface.Models;

namespace Altinn.Platform.Authorization.Repositories.Interface
{
    /// <summary>
    /// Interface for operations on instance information
    /// </summary>
    public interface IInstanceMetadataRepository
    {
        /// <summary>
        /// Gets auth info for a process
        /// </summary>
        /// <param name="instanceId">the instance id</param>
        /// <returns>Auth info</returns>
        Task<(ProcessState Process, string AppId)> GetAuthInfo(string instanceId);

        /// <summary>
        /// Gets the information of a given instance
        /// </summary>
        /// <param name="instanceId">the instance id</param>
        /// <param name="instanceOwnerId">the instance owner</param>
        /// <returns></returns>
        Task<Instance> GetInstance(string instanceId, int instanceOwnerId);

        /// <summary>
        /// Gets the information of a given instance
        /// </summary>
        /// <param name="instanceId">the instance id</param>
        /// <returns></returns>
        Task<Instance> GetInstance(string instanceId);

        /// <summary>
        /// Gets the application information of a given instance
        /// </summary>
        /// <param name="app">Application identifier which is unique within an organisation.</param>
        /// <param name="org">Unique identifier of the organisation responsible for the app.</param>
        /// <returns></returns>
        Task<Application> GetApplication(string app, string org);
    }
}
