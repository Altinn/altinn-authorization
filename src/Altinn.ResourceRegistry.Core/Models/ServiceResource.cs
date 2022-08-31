namespace Altinn.ResourceRegistry.Models
{
    public class ServiceResource
    {
        public string Identifier { get; set; }

        /// <summary>
        /// The title of service
        /// </summary>
        public Dictionary<string, string> Title { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public Dictionary<string, string> Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Homepage { get; set; }    

        /// <summary>
        /// 
        /// </summary>
        public string Status { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public DateTime ValidFrom { get; set; } 
    }
}
