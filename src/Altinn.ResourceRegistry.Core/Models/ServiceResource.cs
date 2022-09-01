using Altinn.ResourceRegistry.Core.Models;

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
        public Dictionary<string, string> RightsDescription { get; set;  }

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

        /// <summary>
        /// 
        /// </summary>
        public DateTime ValidTo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IsPartOf { get; set; }


        public bool IsPublicService { get; set; }

        public bool IsLimitedToSpesificUsers { get; set; }

        public bool IsUsedByNorwegianCitiens { get; set; }

        public bool IsUsedByNorwgianEnterprices { get; set; }

        public bool IsUsedBySelfIdentified { get; set; }

        public string? ThematicArea { get; set; }

        public List<string>? Keywords { get; set;  }

        public ResourceReference ResourceReference { get; set;  }
    }
}
