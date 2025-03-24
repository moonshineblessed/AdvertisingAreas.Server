namespace AdvertisingAreas.Server.Models
{
    public class PlatformNode
    {
        public string Region { get; init; }
        public List<string> PlatformNames { get; set; } = new List<string>();
        public PlatformNode? Root { get; set; }

        public PlatformNode(string region)
        {
            Region = region;
        }
        public PlatformNode(string region, PlatformNode? root)
        {
            Root = root;
            Region = region;
        }
    }
}
