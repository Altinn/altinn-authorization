namespace Altinn.ResourceRegistry.Models
{
    public class ServiceResource
    {
        public string identifier { get; set; }

        public Dictionary<string, string> Description { get; set; }

        public Dictionary<string, string> Title { get; set; }

    }
}
