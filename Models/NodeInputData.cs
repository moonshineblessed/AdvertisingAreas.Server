namespace AdvertisingAreas.Server.Models
{
    public class NodeInputData
    {
        public string Name { get; set; }

        public string Paths { get; set; }

        public NodeInputData(string name, string paths)
        {
            Name = name;
            Paths = paths;
        }
    }
}
