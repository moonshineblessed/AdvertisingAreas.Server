using AdvertisingAreas.Server.Models;

namespace AdvertisingAreas.Server.Services
{
    public class TreeService() : ITreeService
    {

        /// <summary>
        /// Преобразует содержимое файла в список данных для платформ.
        /// </summary>
        /// <param name="filename">Путь к текстовому файлу для обработки.</param>
        /// <returns>Возвращает список объектов NodeInputData, содержащих информацию о платформах.</returns>
        public async Task<List<NodeInputData>> FileConverter(string filename)
        {
            List<NodeInputData> platforms = new();

            StreamReader sr = new(filename);
            string line = await sr.ReadLineAsync();
            while (line != null)
            {
                foreach (var path in line.Split(':')[1].Split(','))
                    platforms.Add(
                        new NodeInputData(
                            line.Split(':')[0],
                            path.Replace('/', ' ').Trim()));

                line = await sr.ReadLineAsync();
            }
            sr.Close();


            return platforms;
        }

        /// <summary>
        /// Создает дерево платформ на основе входных данных.
        /// </summary>
        /// <param name="inputPlatforms">Список данных о платформах, которые необходимо добавить в дерево.</param>
        /// <param name="tree">Объект дерева, в который будут добавлены платформы.</param>
        public void CreateTree(List<NodeInputData> inputPlatforms, Tree tree)
        {

            foreach (var platform in inputPlatforms)
                AddSubTree(platform.Paths, platform.Name, tree);

        }

        /// <summary>
        /// Добавляет поддерево в структуру дерева по указанному пути.
        /// </summary>
        /// <param name="inputString">Путь, по которому будет добавлено поддерево.</param>
        /// <param name="platformName">Название платформы, которое будет добавлено в поддерево.</param>
        /// <param name="tree">Дерево, в которое будет добавлено поддерево.</param>
        /// <param name="path">Опциональный путь для записи в файл.</param>
        public void AddSubTree(string inputString, string platformName, Tree tree, string? path = null)
        {
            var currentNode = tree.Platforms[0];
            var pathSegments = inputString.Split(' ');

            foreach (var segment in pathSegments)
            {
                var existingNode = tree.Platforms.FirstOrDefault(node => node.Region == segment && node.Root == currentNode);
                if (existingNode == null)
                {
                    var newNode = new PlatformNode(segment, currentNode);
                    tree.Add(newNode);
                    currentNode = newNode;
                }
                else
                {
                    currentNode = existingNode;
                }
            }
            currentNode.PlatformNames.Add(platformName);

            if (path is not null)
            {
                using StreamWriter sw = new StreamWriter(path, true);
                sw.Write($"\n{platformName}:{inputString.Replace(' ', '/')}");
            }

        }

        /// <summary>
        /// Находит все платформы по указанному пути в дереве.
        /// </summary>
        /// <param name="path">Путь, по которому нужно найти платформы.</param>
        /// <param name="tree">Дерево, в котором будут искаться платформы.</param>
        /// <returns>Возвращает список платформ, которые соответствуют указанному пути.</returns>
        public List<string> FindPlatformNames(string path, Tree tree)
        {
            List<string> platforms = new();

            var nodes = tree.Platforms.FindAll(node => node.Region == path);
            foreach (var node in nodes)
            {
                var rootNode = node;

                while (rootNode.Root != null)
                {
                    platforms.AddRange(rootNode.PlatformNames);
                    rootNode = rootNode.Root;
                }

            }

            return platforms;
        }

    }

}
