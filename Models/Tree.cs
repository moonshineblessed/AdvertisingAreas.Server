namespace AdvertisingAreas.Server.Models
{
    public class Tree
    {
        public List<PlatformNode> Platforms { get; set; }

        public Tree()
        {
            Platforms = [new("Мир")];
        }
        public void Add(PlatformNode platforms)
        { 
            Platforms.Add(platforms);
        }
    }
}
