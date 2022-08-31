namespace Altinn.AccessGroups.Core.Models
{
    public class AccessGroup
    {
        public AccessGroup(string id)
        {
            AccessGroupId = id;
            Title = new Dictionary<string, string>();
        }

        /// <summary>
        /// The AccessGroupId
        /// </summary>
        public string AccessGroupId { get; set; }

        public Dictionary<string, string> Title { get; set; }
    }
}
