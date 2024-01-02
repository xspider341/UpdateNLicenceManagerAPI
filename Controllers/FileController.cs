using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using UpdateNLicenceManagerAPI.Models;

namespace UpdateNLicenceManagerAPI.Controllers
{
    public class FileTransferOptions
    {
        public string Path { get; set; }
    }


    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly FileTransferOptions _fileOptions;
        public FileController(FileTransferOptions fileOptions)
        {
            _fileOptions = fileOptions;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Upload a file.");

            /*var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var path = Path.Combine(desktopPath, _fileOptions.Path);*/

            var path = Path.Combine(_fileOptions.Path, file.FileName);

            if (!Directory.Exists(_fileOptions.Path))
                Directory.CreateDirectory(_fileOptions.Path);

            using (var stream = new FileStream(_fileOptions.Path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileMetaData = new FileMetaData
            {
                FileName = file.FileName,
                Version = "1.0", //TODO Sürümü burada ayarlanacak, hangi sürüm verileceği otomatize edilebilir.
                UploadDate = DateTime.UtcNow
            };

            // Veritabanına kaydet //TODO veri tabanına upload edilen datanın meta dataları kaydedilecek.
            // Örnek: _dbContext.FileMetaDatas.Add(fileMetaData);
            // await _dbContext.SaveChangesAsync();


            return Ok(new { file.FileName, path });
        }

        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), _fileOptions.Path); //path static veya 
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        private string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(path, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }

    }
}