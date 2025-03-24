using AdvertisingAreas.Server.Models;

namespace AdvertisingAreas.Server.Services
{
    public interface ITreeService
    {
        Task<List<NodeInputData>> FileConverter(string filename);
        void CreateTree(List<NodeInputData> inputPlatforms, Tree tree);
        void AddSubTree(string inputString, string platformName, Tree tree, string? path = null);
        List<string> FindPlatformNames(string path, Tree tree);
    }
}
