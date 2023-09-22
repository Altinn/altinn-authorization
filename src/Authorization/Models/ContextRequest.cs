using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Altinn.Platform.Authorization.Models
{
    /// <summary>
    /// This model describes an authorization event. An authorization event is an action triggered when a user requests access to an operation
    /// </summary>
    public class ContextRequest
    {
        /// <summary>
        /// Return policy id list
        /// </summary>
        public bool ReturnPolicyIdList { get; set; }

        /// <summary>
        /// Subject of request
        /// </summary>
        public List<AccessSubject> AccessSubject { get; set; }

        /// <summary>
        /// Action of the request
        /// </summary>
        public List<Action> Action { get; set; }

        /// <summary>
        /// Resources list
        /// </summary>
        public List<Resource> Resources { get; set; }
    }

    /// <summary>
    /// Subject of the request
    /// </summary>
    public class AccessSubject
    {
        /// <summary>
        /// subject attributes
        /// </summary>
        public List<Attribute> Attribute { get; set; }
    }

    /// <summary>
    /// Action attributes of the request
    /// </summary>
    public class Action
    {
        /// <summary>
        /// List of action attributes
        /// </summary>
        public List<Attribute> Attribute { get; set; }
    }

    /// <summary>
    /// Resource in the request
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// List of resource attributes
        /// </summary>
        public List<Attribute> Attribute { get; set; }
    }

    /// <summary>
    /// Attribute
    /// </summary>
    public class Attribute
    {
        /// <summary>
        /// Id of the attribute
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Value of the attribute
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Data type of the attribute
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// Include the attribute in result
        /// </summary>
        public bool IncludeInResult { get; set; }
    }
}
