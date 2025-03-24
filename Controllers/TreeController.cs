using AdvertisingAreas.Server.Contracts;
using AdvertisingAreas.Server.Models;
using AdvertisingAreas.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdvertisingAreas.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TreeController : Controller
    {
        private Tree _tree;
        private readonly ITreeService _treeService;
        private readonly IConfiguration _configuration;
        IWebHostEnvironment _appEnvironment;

        public TreeController(Tree tree, ITreeService treeService, 
                                IWebHostEnvironment appEnvironment, 
                                IConfiguration configuration)
        {
            _tree = tree;
            _treeService = treeService;
            _appEnvironment = appEnvironment;
            _configuration = configuration;
        }


        /// <summary>
        /// Загружает текстовый файл на сервер, обрабатывает его содержимое и обновляет структуру дерева.
        /// </summary>
        /// <param name="file">Файл для загрузки. Должен быть в формате .txt.</param>
        /// <returns>Возвращает статус 200 с объектом дерева, если файл успешно загружен и обработан. В случае ошибки возвращает ошибку.</returns>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            
            var relativePath = _appEnvironment.ContentRootPath + _configuration.GetSection("FileStoragePath").Value;
            
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не выбран или пуст.");
            }
            if (Path.GetExtension(file.FileName).ToLower() != ".txt")
            {
                return BadRequest("Неверный формат файла. Требуется файл с расширением .txt.");
            }

            try
            {
                using (var fileStream = new FileStream(relativePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                _treeService.CreateTree(await _treeService.FileConverter(relativePath),
                                        _tree);

                return Ok(_tree);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка обработки файла: {ex.Message}");
            }
        }

        /// <summary>
        /// Получает список платформ для указанной локации.
        /// </summary>
        /// <param name="location">Локация, для которой необходимо получить платформы. Прописывается только искомый регион без родительских </param>
        /// <returns>Возвращает список платформ, если они найдены, иначе возвращает ошибку или статус "не найдено".</returns>
        [HttpGet("platforms")]
        public IActionResult GetPlatforms(string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                return BadRequest("Не указана локация.");
            }


            if (_tree.Platforms.Count <= 1)
            {
                return NotFound("Дерево платформ не инициализировано.");
            }

            var platform = _treeService.FindPlatformNames(location, _tree);

            return Ok(platform);
        }

        /// <summary>
        /// Создает новую платформу под указанной локацией в структуре дерева.
        /// </summary>
        /// <param name="location">Локация, под которой будет создана новая платформа.</param>
        /// <param name="platformName">Название новой платформы.</param>
        /// <returns>Возвращает статус 200, если платформа была успешно создана, иначе возвращает ошибку.</returns>
        [HttpPost("platdorms")]
        public IActionResult CreatePlatform([FromBody] CreateSubTreeRequest request)
        {
            var relativePath = _appEnvironment.ContentRootPath + _configuration.GetSection("FileStoragePath").Value;
            if (string.IsNullOrEmpty(request.Location) || 
                string.IsNullOrEmpty(request.PlatformName))
            {
                return BadRequest("Все поля должны быть заполнены");
            }
            try
            {
                _treeService.AddSubTree(request.Location, request.PlatformName, _tree, relativePath);
                return Ok();
            }
            catch(Exception ex) 
            {
                return NotFound($"Возникла ошибка в добавлении платформы: {ex}");
            }
        }
    }
}
